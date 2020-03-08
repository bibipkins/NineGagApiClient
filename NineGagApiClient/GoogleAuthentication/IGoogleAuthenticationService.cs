using System.Threading.Tasks;

namespace NineGagApiClient.GoogleAuthentication
{
    public interface IGoogleAuthenticationService
    {
        string GetAuthenticationPageUrl();
        Task<string> GetAccessTokenAsync(string code);        
    }
}
