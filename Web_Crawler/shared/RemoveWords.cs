using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Web_Crawler.shared
{
    class RemoveWords
    {
        Dictionary<string, string> _regExpression;
        string _basePath;
        ImageProcess ImageProcess;
        string imagePrefixURL = string.Empty, linkPrefixURL = string.Empty;
        Uri _URL;

        public RemoveWords(string basePath)
        {
            _regExpression = new Dictionary<string, string>();
            loadDefault();
            _basePath = basePath;
            ImageProcess = new ImageProcess(basePath);
            imagePrefixURL = ConfigurationManager.AppSettings["ImagePrefixURL"].ToString();
            linkPrefixURL = ConfigurationManager.AppSettings["LinkPrefixURL"].ToString();
        }

        public RemoveWords()
        {
            _regExpression = new Dictionary<string, string>();
            loadDefault();
        }

        /// <summary>
        /// Constructor
        /// </summary>
        private void loadDefault()
        {
            RegExpression.Add("<b>", " **");
            RegExpression.Add("<br>", "\n");
            RegExpression.Add("<code>", "`");
            RegExpression.Add("</code>", "`");
            RegExpression.Add("<i>", "*");
            RegExpression.Add("</i>", "*");
            RegExpression.Add("</b>", "** ");
            RegExpression.Add("<strong>", "**");
            RegExpression.Add("</strong>", "**");
            RegExpression.Add("<blockquote>", ">");
            RegExpression.Add("</blockquote>", " ");

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="textWriter"></param>
        /// <returns>string</returns>
        public void getInnerText(HtmlNode data, TextWriter textWriter)
        {
            string InnerText = string.Empty;
            try
            {
                StringBuilder stringBuilder = new StringBuilder();
                foreach(var item in data.ChildNodes)
                {
                    stringBuilder.Append(writeTextfile(item));
                }
                textWriter.Write(stringBuilder);
            }
            catch(Exception ex) { throw ex; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns>string</returns>
        public string getInnerText(HtmlNode data)
        {
            StringBuilder stringBuilder = new StringBuilder();
            try
            {
                foreach(var item in data.ChildNodes)
                {
                    stringBuilder.Append(writeTextfile(item));
                }
            }
            catch(Exception ex) { throw ex; }
            return stringBuilder.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="htmlNode"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        public string readValue(HtmlNode htmlNode, string selector)
        {
            string value = string.Empty;
            string fileName = string.Empty;
            switch(selector.ToLower())
            {
                case "innerhtml":
                    value = replaceRegEx(htmlNode.InnerHtml.Trim());
                    break;
                case "innertext":
                    value = replaceRegEx(htmlNode.InnerText);
                    break;
                case "href":
                    Uri uri = new Uri(htmlNode.Attributes[selector].Value);

                    if(_URL.Host == uri.Host)
                        value = linkPrefixURL + uri.AbsolutePath;
                    else
                        value = htmlNode.Attributes[selector].Value;
                    break;
                case "src":
                    //string alt = htmlNode.Attributes["alt"].Value;
                    fileName = ImageProcess.DownloadImage(htmlNode.Attributes[selector].Value);
                    value = string.Format("![{0}]({1})", fileName, imagePrefixURL + fileName);
                    break;
                case "svg":
                    fileName = ImageProcess.ConvertSVGtoImage(htmlNode);
                    value = string.Format("![{0}]({1})", fileName, imagePrefixURL + fileName);
                    break;
            }
            value = HttpUtility.HtmlDecode(value);
            return value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        private string writeTextfile(HtmlNode item)
        {
            string value = string.Empty;
            switch(item.Name)
            {
                case "#text":
                    value = readValue(item, "InnerText");
                    break;
                case "span":
                    if(item.HasChildNodes)
                        value = getInnerText(item).ToString();
                    else if(item.InnerText.Trim().Length > 0)
                        value = readValue(item, "InnerText");
                    break;
                case "h1":
                case "h2":
                case "h3":
                case "h4":
                case "h5":
                case "h6":
                    if(item.HasChildNodes)
                        value = string.Format("\n#### {0}", getInnerText(item).ToString());
                    else if(item.InnerText.Trim().Length > 0)
                        value = string.Format("#### {0}", readValue(item, "InnerText"));
                    value += "\n";
                    break;
                case "p":
                    if(item.HasChildNodes)
                        value = getInnerText(item).ToString();
                    else if(item.InnerText.Trim().Length > 0)
                        value = readValue(item, "InnerText");
                    value = value + "\n";
                    break;
                case "b":
                case "strong":
                    if(item.InnerText.Trim().Length > 0)
                    {
                        value = string.Format(" **{0}** ", replaceRegEx(item.InnerText.Trim()));
                    }
                    break;
                case "br":
                    value = "\n";
                    break;

                case "code":
                    if(item.InnerText.Trim().Length > 0)
                    {
                        value = string.Format(" `{0}` ", replaceRegEx(item.InnerText.Trim()));
                    }
                    break;
                case "i":
                    if(item.InnerText.Trim().Length > 0)
                    {
                        value = string.Format(" *{0}* ", replaceRegEx(item.InnerText.Trim()));
                    }
                    break;

                case "ol":
                    int i = 1;
                    foreach(var val in item.ChildNodes)
                    {
                        if(val.Name == "li")
                        {
                            value += (i++).ToString() + ". " + getInnerText(val).ToString() + "\n";
                        }
                    }
                    value += "\n";
                    break;
                case "li":
                    if(item.HasChildNodes)
                        value = string.Format("* {0}", getInnerText(item).ToString());
                    else if(item.InnerText.Trim().Length > 0)
                        value = "* " + readValue(item, "InnerText");
                    break;
                case "div":
                case "ul":
                    foreach(var val in item.ChildNodes)
                    {
                        value += writeTextfile(val);
                    }
                    value += "\n";
                    break;
                case "iframe":
                    value += item.OuterHtml.Trim();
                    value += "\n";
                    break;
                case "img":
                    value = readValue(item, "src");
                    break;
                case "svg":
                    value = readValue(item, "svg");
                    break;
                case "path":
                    value = readValue(item, "d");
                    break;
                case "a":
                    if(item.HasChildNodes)
                        value = string.Format("[{0}]({1})", getInnerText(item).ToString(), readValue(item, "href"));
                    else
                        value = readValue(item, "href");
                    break;

            }
            return value;
        }

        public string replaceRegEx(string word)
        {
            var regList = RegExpression.Where(x => word.IndexOf(x.Key) != -1).ToList();
            if(regList.Count > 0)
            {
                foreach(var regEx in regList)
                {
                    int _index = word.IndexOf(regEx.Key);
                    if(_index != -1)
                    {
                        word = word.Replace(regEx.Key, regEx.Value);
                    }
                }
            }
            return word;
        }

        public Dictionary<string, string> RegExpression { get => _regExpression; set => _regExpression = value; }
        public string BasePath { get => _basePath; set => _basePath = value; }
        public Uri URL { get => _URL; set => _URL = value; }
    }
}
