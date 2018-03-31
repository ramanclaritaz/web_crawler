using HtmlAgilityPack;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Web_Crawler.shared;

namespace Web_Crawler
{
    static class Crawler
    {
        static WebRequest myWebRequest;
        static WebResponse myWebResponse;
        static HtmlDocument htmlDoc;
        static icategoryDetails _cat;
        static icategoryData _categoryData;
        static string baseURL;

        public static void readData(string URL)
        {
            try
            {
                htmlDoc = new HtmlDocument();
                _categoryData = new categoryData();
                LoadJson();
                htmlDoc.LoadHtml(getResponse(URL));
                _categoryData.page_title = htmlDoc.QuerySelector(_cat.page_title).InnerHtml;
                var categoryList = htmlDoc.QuerySelectorAll(_cat.category.list);
                if (URL.EndsWith("/"))
                    baseURL = URL.Remove(URL.Length - 1);
                foreach (var item in categoryList)
                {
                    var cat = new cat();
                    cat.href = baseURL + item.QuerySelector(_cat.category.href).Attributes["href"].Value;
                    cat.title = item.QuerySelector(_cat.category.title).InnerText;
                    cat.meta = item.QuerySelector(_cat.category.meta).InnerText;
                    cat.avatar.image = item.QuerySelector(_cat.category.avatar.image).Attributes["src"].Value;
                    cat.avatar.info = item.QuerySelector(_cat.category.avatar.info).InnerText;
                    cat.subCategoryList = getSubcategory(cat.href);
                    _categoryData.categoryList.Add(cat);

                }

                //tw.WriteLine(_categoryData.page_title);
                foreach (var cat in _categoryData.categoryList)
                {
                    TextWriter tw = File.CreateText(Environment.CurrentDirectory + "\\Output\\" + cat.title.Replace(' ', '_') + ".md");
                    tw.WriteLine("# " + cat.title);
                    tw.WriteLine("\t" + cat.meta);
                    if (cat.photo != null && cat.photo != "")
                        tw.WriteLine("\t ![] (" + cat.photo + ")");
                    if (cat.avatar.image != null && cat.avatar.image != "")
                        tw.WriteLine("![avatar] (" + cat.avatar.image + ")");
                    if (cat.avatar.info != null && cat.avatar.info != "")
                        tw.WriteLine("\t" + cat.avatar.info);
                    if (cat.subCategoryList.Count > 0)
                    {
                        tw.WriteLine("\n");
                        writeSubCategory(tw, cat.subCategoryList);
                    }
                    tw.Close();
                }



                Console.WriteLine(Console.Read());


            }
            catch (Exception ex) { throw ex; }
        }

        private static void writeSubCategory(TextWriter tw, List<subCategory> subCategoryList)
        {
            try
            {
                foreach (var cat in subCategoryList)
                {
                    tw.WriteLine("## " + cat.title);
                    tw.WriteLine("\t" + cat.meta);
                    if (cat.avatar.image != null && cat.avatar.image != "")
                        tw.WriteLine("![avatar](" + cat.avatar.image + ")");
                    tw.WriteLine("\t" + cat.avatar.info);
                    if (cat.article.Count > 0)
                    {
                        tw.WriteLine("\n");
                        writeArticle(tw, cat.article);
                    }

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private static void writeArticle(TextWriter tw, List<article> article)
        {
            try
            {
                foreach (var cat in article)
                {
                    HtmlDocument _htmlDoc = new HtmlDocument();
                    _htmlDoc.LoadHtml(cat.article_info);
                    var List = _htmlDoc.DocumentNode.ChildNodes[0];
                    foreach (var data in List.ChildNodes)
                    {
                        tw.WriteLine(getInnerText(data));
                    }

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private static string getInnerText(HtmlNode data)
        {
            string InnerText = string.Empty;
            try
            {
                switch (data.Name)
                {
                    case "p":
                        InnerText = "\t" + data.InnerText;
                        break;
                    case "div":
                        foreach (var val in data.ChildNodes)
                        {
                            InnerText = getInnerText(val);
                        }
                        break;
                    case "img":
                        InnerText = "![img](" + data.Attributes["src"].Value + ")";
                        break;

                }
            }
            catch (Exception ex) { throw ex; }
            return InnerText;
        }
        private static List<subCategory> getSubcategory(string href)
        {
            List<subCategory> List = new List<subCategory>();
            try
            {
                var readHtml = new HtmlDocument();
                readHtml.LoadHtml(getResponse(href));
                var list = readHtml.QuerySelectorAll(_cat.subcategory.selector);
                foreach (var sub in list)
                {
                    var data = new subCategory();
                    data.title = sub.QuerySelector(_cat.subcategory.title).InnerText;
                    data.href = baseURL + sub.Attributes["href"].Value;
                    //data.photo = sub.QuerySelector(_cat.subcategory.photo).Attributes["src"].Value;
                    data.meta = sub.QuerySelector(_cat.subcategory.meta).InnerText;
                    data.article = getArticle(data.href);
                    List.Add(data);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return List;
        }

        private static List<article> getArticle(string href)
        {
            List<article> List = new List<article>();
            try
            {
                var readHtml = new HtmlDocument();
                readHtml.LoadHtml(getResponse(href));
                var data = new article();
                data.title = readHtml.QuerySelector(_cat.article.title).InnerText;
                data.article_info = readHtml.QuerySelector(_cat.article.article_info).OuterHtml;
                data.descriptions = readHtml.QuerySelector(_cat.article.descriptions).InnerText;
                data.avatar = readHtml.QuerySelector(_cat.article.avatar).InnerHtml;
                List.Add(data);
            }
            catch (Exception ex) { throw ex; }
            return List;
        }

        private static string getResponse(string URL)
        {
            string res = string.Empty;
            try
            {
                myWebRequest = WebRequest.Create(URL);
                myWebResponse = myWebRequest.GetResponse(); //Returns a response from an Internet resource
                Stream streamResponse = myWebResponse.GetResponseStream(); //return the data stream from the internet
                StreamReader sreader = new StreamReader(streamResponse); //reads the data stream
                res = sreader.ReadToEnd(); //reads it to the end
                streamResponse.Close();
                sreader.Close();
                myWebResponse.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return res;
        }
        private static icategoryDetails LoadJson()
        {
            using (StreamReader r = new StreamReader("D:\\Ramanathan\\BizTalk\\Crawler\\Web_Crawler\\Web_Crawler\\application.json"))
            {
                string json = r.ReadToEnd();
                _cat = new categoryDetails();
                _cat = JsonConvert.DeserializeObject<categoryDetails>(json);
            }
            return _cat;
        }

    }

}
