﻿using Newtonsoft.Json;
using System.Net;

namespace NineGagApiClient.Models
{
    public class Post
    {
        #region Properties

        public string Id
        {
            get;
            set;
        }
        public string Title
        {
            get { return _title; }
            set { _title = WebUtility.HtmlDecode(value); }
        }
        public string Url
        {
            get;
            set;
        }
        public int UpvoteCount
        {
            get;
            set;
        }
        public int DownvoteCount
        {
            get;
            set;
        }
        public int CommentsCount
        {
            get;
            set;
        }
        public PostMedia PostMedia
        {
            get;
            set;
        }

        public PostType Type
        {
            get;
            set;
        }
        [JsonProperty(PropertyName = "postSection")]
        public PostSection Section
        {
            get;
            set;
        }
        [JsonProperty(PropertyName = "nsfw")]
        public bool IsNsfw
        {
            get;
            set;
        }

        #endregion

        #region Methods

        public override string ToString()
        {
            return $"{Id}, {Title}, URL: {Url}, MediaURL: {PostMedia.Url}, " +
                $"CommentsCount: {CommentsCount}, UpvoteCount: {UpvoteCount}, " +
                $"DownvoteCount: {DownvoteCount}, Type: {Type.ToString()}, NSFW: {IsNsfw}, " +
                $"Section: {Section?.Name}";
        }

        #endregion

        #region Fields

        private string _title;

        #endregion
    }
}
