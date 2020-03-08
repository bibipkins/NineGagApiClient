using Newtonsoft.Json;

namespace Models.Post
{
    public class PostSection
    {
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
        public string Url { get; set; }
        public string ImageUrl { get; set; }
    }
}
