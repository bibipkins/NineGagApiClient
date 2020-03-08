using Newtonsoft.Json;

namespace Models.User
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
