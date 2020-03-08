using System.Threading.Tasks;

namespace Logic.GoogleAuthentication
{
    public interface IGoogleAuthenticationService
    {
        string GetAuthenticationPageUrl();
        Task<string> GetAccessTokenAsync(string code);        
    }
}
