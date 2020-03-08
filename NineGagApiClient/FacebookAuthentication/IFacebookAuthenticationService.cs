
namespace NineGagApiClient.FacebookAuthentication
{
    public interface IFacebookAuthenticationService
    {
        string GetAuthenticationPageUrl(string state);
    }
}
