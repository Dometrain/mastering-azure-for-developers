
using Azure.Identity;
using Azure.Security.KeyVault.Keys;
using Azure.Security.KeyVault.Keys.Cryptography;
using System.Text;

namespace TravelInspiration.Client.Web.Services;

public class KeyEncryptionService : IEncryptionService
{
    private readonly CryptographyClient _cryptographyClient;
    public KeyEncryptionService(IConfiguration configuration)
    {
        var keyClient = new KeyClient(new Uri(configuration["KeyVaultUri"] ??
                throw new InvalidOperationException("Missing configuration value KeyVaultUri")),
                new DefaultAzureCredential());

        _cryptographyClient = keyClient
            .GetCryptographyClient("TravelInspirationDataEncryptionKey");
    }
    public async Task<string> DecryptAsync(string encryptedText)
    {
        var ciphertextBytes = Convert.FromBase64String(encryptedText);
        var decryptResult = await _cryptographyClient.DecryptAsync(EncryptionAlgorithm.RsaOaep256,
            ciphertextBytes);

        return Encoding.UTF8.GetString(decryptResult.Plaintext);
    }

    public async Task<string> EncryptAsync(string plainText)
    {
        var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
        var encryptResult = await _cryptographyClient.EncryptAsync(EncryptionAlgorithm.RsaOaep256,
            plainTextBytes);

        return Convert.ToBase64String(encryptResult.Ciphertext);
    }
}
