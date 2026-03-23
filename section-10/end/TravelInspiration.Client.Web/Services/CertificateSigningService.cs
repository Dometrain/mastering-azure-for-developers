using Azure.Identity;
using Azure.Security.KeyVault.Certificates;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace TravelInspiration.Client.Web.Services;

public class CertificateSigningService : ISigningService
{
    private readonly X509Certificate2 _signingCertificate;

    public CertificateSigningService(IConfiguration configuration)
    {
        var certificateClient = new CertificateClient(new Uri(configuration["KeyVaultUri"] ??
            throw new InvalidOperationException("Missing configuration value KeyVaultUri")),
            new DefaultAzureCredential());

        // Download the full certificate with chain for validation
        var certificateWithPolicy = certificateClient
            .DownloadCertificate("TravelInspirationEncryptionCertificate");
        _signingCertificate = certificateWithPolicy.Value;

        // Validate the certificate trust chain
        ValidateCertificateChain(_signingCertificate);
    }

    public string SignData(string textToSign)
    {
        var plaintextBytes = Encoding.UTF8.GetBytes(textToSign);

        using (var rsa = _signingCertificate.GetRSAPrivateKey() ??
            throw new InvalidOperationException("Certificate does not contain an RSA private key"))
        {
            var signature = rsa.SignData(plaintextBytes,
                HashAlgorithmName.SHA256,
                RSASignaturePadding.Pkcs1);

            return Convert.ToBase64String(signature);
        }
    }

    public bool VerifySignature(string textThatWasSigned, string signature)
    {
        var textThatWasSignedBytes = Encoding.UTF8.GetBytes(textThatWasSigned);
        var signatureBytes = Convert.FromBase64String(signature);

        using (var rsa = _signingCertificate.GetRSAPublicKey() ??
             throw new InvalidOperationException("Certificate does not contain an RSA public key"))
        {
            return rsa.VerifyData(textThatWasSignedBytes,
                   signatureBytes,
                   HashAlgorithmName.SHA256,
                   RSASignaturePadding.Pkcs1);
        }
    }

    private void ValidateCertificateChain(X509Certificate2 certificate)
    {
        using (var chain = new X509Chain())
        { 
            // Detect if this is a self-signed certificate
            bool isSelfSigned = certificate.Subject == certificate.Issuer;

            if (isSelfSigned)
            {
                // For self-signed certificates, use custom trust store
                chain.ChainPolicy.TrustMode = X509ChainTrustMode.CustomRootTrust;
                chain.ChainPolicy.CustomTrustStore.Add(certificate);

                // Disable revocation checking for self-signed certificates
                chain.ChainPolicy.RevocationMode = X509RevocationMode.NoCheck;
            }
            else
            {
                // For CA-signed certificates, use standard validation
                chain.ChainPolicy.TrustMode = X509ChainTrustMode.System;
                chain.ChainPolicy.RevocationMode = X509RevocationMode.Online;
            } 

            // this method will trigger validation
            bool isChainValid = chain.Build(certificate);

            if (!isChainValid)
            {
                var errors = new List<string>();

                foreach (var element in chain.ChainElements)
                {
                    foreach (var status in element.ChainElementStatus)
                    {
                        if (status.Status == X509ChainStatusFlags.NoError)
                            continue;

                        var errorMessage = $"Certificate '{element.Certificate.Subject}': {status.Status} - {status.StatusInformation}";
                        errors.Add(errorMessage);
                    }
                }

                throw new InvalidOperationException(
                    $"Certificate chain validation failed. Certificate cannot be trusted. Errors: {string.Join("; ", errors)}");
            }

            // Validate certificate expiration
            var now = DateTime.UtcNow;
            if (certificate.NotBefore > now)
            {
                throw new InvalidOperationException(
                    $"Certificate is not yet valid. Valid from: {certificate.NotBefore:u}");
            }

            if (certificate.NotAfter < now)
            {
                throw new InvalidOperationException(
                    $"Certificate has expired. Valid until: {certificate.NotAfter:u}");
            }
        }
    }

}
