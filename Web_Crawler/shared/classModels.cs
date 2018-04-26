using HtmlAgilityPack;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Web_Crawler.shared
{

    class Category
    {
        [JsonProperty(PropertyName = "selector")]
        public string Selector { get; set; }
        [JsonProperty(PropertyName = "text")]
        public string Text { get; set; }
    }

    class Title
    {
        public string selector { get; set; }
        public string text { get; set; }
    }

    class Article
    {
        public Category article_info { get; set; }
        public Category descriptions { get; set; }
        public string selector { get; set; }
        public Category title { get; set; }
    }

    class Articlelist
    {
        public string selector { get; set; }
        public string text { get; set; }
    }

    class Subcategory
    {
        public string selector { get; set; }
        public string text { get; set; }
    }

    class Webcrawler
    {
        public Category category { get; set; }
        public Category subcategorylist { get; set; }
        public Article article { get; set; }
        public Category articlelist { get; set; }
        public Category subcategory { get; set; }
    }


    
}
