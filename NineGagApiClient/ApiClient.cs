using Newtonsoft.Json.Linq;
using NineGagApiClient.Models;
using NineGagApiClient.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace NineGagApiClient
{
    public class ApiClient : IApiClient, IDisposable
    {
        #region Constructors

        public ApiClient() : this(new HttpClient())
        {
            _disposeHttpClient = true;
        }

        public ApiClient(HttpClient httpClient)
        {
            _disposeHttpClient = false;
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            AuthenticationInfo = new AuthenticationInfo();
        }

        #endregion

        #region Properties

        public AuthenticationInfo AuthenticationInfo { get; protected set; }

        #endregion

        #region Api Methods

        public virtual async Task<IList<Post>> GetPostsAsync(PostCategory postCategory, int count, string olderThanPostId = "")
        {
            var args = new Dictionary<string, string>()
            {
                { "group", "1" },
                { "type", postCategory.ToString().ToLower() },
                { "itemCount", count.ToString() },
                { "entryTypes", "animated,photo,video" },
                { "offset", "10" }
            };

            if (!string.IsNullOrEmpty(olderThanPostId))
            {
                args["olderThan"] = olderThanPostId;
            }

            var posts = new List<Post>();
            var request = FormRequest(NineGagUtils.ApiUrl, RequestUtils.POSTS_PATH, args);
            await ExecuteRequestAsync(request, responseText =>
            {
                var jsonData = JObject.Parse(responseText);
                var rawPosts = jsonData["data"]["posts"];

                foreach (var item in rawPosts)
                {
                    var post = CreatePost(item);
                    posts.Add(post);
                }
            });

            return posts;
        }
        public virtual async Task<IList<Comment>> GetCommentsAsync(string postUrl, int count)
        {
            string path =
                "v1/topComments.json?" +
                "appId=a_dd8f2b7d304a10edaf6f29517ea0ca4100a43d1b" +
                "&urls=" + postUrl +
                "&commentL1=" + count +
                "&pretty=0";

            var comments = new List<Comment>();
            var request = FormRequest(NineGagUtils.CommentCdnUrl, path, new Dictionary<string, string>());
            await ExecuteRequestAsync(request, responseText =>
            {
                var jsonData = JObject.Parse(responseText);
                var postComments = jsonData.SelectToken("payload.data.[0].comments");

                foreach (var item in postComments)
                {
                    Comment comment = item.ToObject<Comment>();
                    comment.MediaUrl = GetUrlFromJsonComment(item);
                    comments.Add(comment);
                }

                comments = comments.OrderByDescending(c => c.LikesCount).ToList();
            });

            return comments;
        }

        #endregion

        #region Auth Methods

        public virtual async Task LoginWithCredentialsAsync(string userName, string password)
        {
            var args = new Dictionary<string, string>()
            {
                { "loginMethod", "9gag" },
                { "loginName", userName },
                { "password", RequestUtils.GetMd5(password) },
                { "language", "en_US" },
                { "pushToken", AuthenticationInfo.Token }
            };

            await LoginAsync(args, AuthenticationType.Credentials);
            AuthenticationInfo.UserLogin = userName;
            AuthenticationInfo.UserPassword = password;
        }
        public virtual async Task LoginWithGoogleAsync(string token)
        {
            var args = new Dictionary<string, string>()
                {
                    { "userAccessToken", token },
                    { "loginMethod", "google-plus" },
                    { "language", "en_US" },
                    { "pushToken", AuthenticationInfo.Token }
                };

            await LoginAsync(args, AuthenticationType.Google);
        }
        public virtual async Task LoginWithFacebookAsync(string token)
        {
            var args = new Dictionary<string, string>()
            {
                { "loginMethod", "facebook" },
                { "userAccessToken", token },
                { "language", "en_US" },
                { "pushToken", AuthenticationInfo.Token }
            };

            await LoginAsync(args, AuthenticationType.Facebook);
        }
        public virtual void Logout()
        {
            AuthenticationInfo.ClearToken();
        }

        protected async Task LoginAsync(Dictionary<string, string> args, AuthenticationType authenticationType)
        {
            var request = FormRequest(NineGagUtils.ApiUrl, RequestUtils.LOGIN_PATH, args);
            await ExecuteRequestAsync(request, responseText =>
            {
                var jsonData = JObject.Parse(responseText);
                var authData = jsonData["data"];

                AuthenticationInfo.Token = authData["userToken"].ToString();
                AuthenticationInfo.LastAuthenticationType = authenticationType;

                long.TryParse(authData["tokenExpiry"].ToString(), out long seconds);
                AuthenticationInfo.TokenWillExpireAt = DateTimeOffset.FromUnixTimeSeconds(seconds).UtcDateTime;

                string readStateParams = authData["noti"]["readStateParams"].ToString();
            });
        }

        #endregion

        #region Helpers

        public virtual void Dispose()
        {
            if (_disposeHttpClient)
            {
                _httpClient.Dispose();
            }
        }

        protected virtual HttpRequestMessage FormRequest(string api, string path, Dictionary<string, string> args)
        {
            var timestamp = RequestUtils.GetTimestamp();

            var headers = new Dictionary<string, string>()
            {
                { "User-Agent", ".NET" },
                { "Accept", " */*" },
                { "9GAG-9GAG_TOKEN", AuthenticationInfo.Token },
                { "9GAG-TIMESTAMP", timestamp },
                { "9GAG-APP_ID", AuthenticationInfo.AppId },
                { "X-Package-ID", AuthenticationInfo.AppId },
                { "9GAG-DEVICE_UUID", AuthenticationInfo.DeviceUuid },
                { "X-Device-UUID", AuthenticationInfo.DeviceUuid },
                { "9GAG-DEVICE_TYPE", "android" },
                { "9GAG-BUCKET_NAME", "MAIN_RELEASE" },
                { "9GAG-REQUEST-SIGNATURE", RequestUtils.GetSignature(timestamp, AuthenticationInfo.AppId, AuthenticationInfo.DeviceUuid) }
            };

            var argsStrings = new List<string>();

            foreach (var entry in args)
            {
                argsStrings.Add(string.Format("{0}/{1}", entry.Key, entry.Value));
            }

            var urlItems = new List<string>()
            {
                api,
                path,
                string.Join("/", argsStrings)
            };

            var url = string.Join("/", urlItems);
            var request = new HttpRequestMessage(HttpMethod.Get, url);

            foreach (var entry in headers)
            {
                request.Headers.Add(entry.Key, entry.Value);
            }

            return request;
        }
        protected virtual async Task ExecuteRequestAsync(HttpRequestMessage request, Action<string> onSuccess = null)
        {
            using (var response = await _httpClient.SendAsync(request))
            {
                var responseText = await response.Content.ReadAsStringAsync();
                ValidateRequestResponse(responseText);
                onSuccess?.Invoke(responseText);
            }
        }
        protected virtual void ValidateRequestResponse(string response)
        {
            var jsonData = JObject.Parse(response);

            if (jsonData.ContainsKey("meta"))
            {
                if (jsonData["meta"]["status"].ToString() != "Success")
                {
                    throw new InvalidOperationException($"Request failed: {jsonData["meta"]["errorMessage"].ToString()}");
                }
            }
            else if (jsonData.ContainsKey("status"))
            {
                if (jsonData["status"].ToString() == "ERROR")
                {
                    throw new InvalidOperationException($"Request failed: {jsonData["error"].ToString()}");
                }
            }
        }

        protected virtual Post CreatePost(JToken item)
        {
            var post = item.ToObject<Post>();
            post.PostMedia = CreatePostMedia(post.Type, item);
            return post;
        }
        protected virtual PostMedia CreatePostMedia(PostType type, JToken jsonObject)
        {
            switch (type)
            {
                case PostType.Photo:
                    return jsonObject["images"]["image700"].ToObject<PostMedia>();
                case PostType.Animated:
                    return jsonObject["images"]["image460sv"].ToObject<PostMedia>();
                default:
                    throw new InvalidOperationException("Could not convert this post to any of existing media views");
            }
        }
        protected virtual string GetUrlFromJsonComment(JToken token)
        {
            var urlToken =
                token.SelectToken("media.[0].imageMetaByType.animated.url") ??
                token.SelectToken("media.[0].imageMetaByType.image.url") ??
                string.Empty;

            return urlToken.ToString();
        }

        #endregion

        #region Fields

        private readonly HttpClient _httpClient;
        private readonly bool _disposeHttpClient;

        #endregion
    }
}
