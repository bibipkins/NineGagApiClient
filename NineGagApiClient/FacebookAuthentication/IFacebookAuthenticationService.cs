
namespace Logic.FacebookAuthentication
{
    public interface IFacebookAuthenticationService
    {
        string GetAuthenticationPageUrl(string state);
    }
}
