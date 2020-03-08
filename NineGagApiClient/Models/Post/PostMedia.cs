using System;
using System.Collections.Generic;
using System.Text;

namespace NineGagApiClient.Models
{
    public class PostMedia
    {
        public PostType Type { get; set; }
        public string Url { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
    }
}
