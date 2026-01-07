namespace TravelInspiration.Client.Web.Services;

public interface ISigningService
{
    string SignData(string textToSign);
    bool VerifySignature(string textThatWasSigned, string signature);
}
