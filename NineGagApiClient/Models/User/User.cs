using Newtonsoft.Json;

namespace NineGagApiClient.Models
{
    public class User
    {
        [JsonProperty(PropertyName = "userId")]
        public string Id { get; set; }
        [JsonProperty(PropertyName = "displayName")]
        public string UserName { get; set; }
        [JsonProperty(PropertyName = "avatarUrl")]
        public string UserAvatar { get; set; }
    }
}
