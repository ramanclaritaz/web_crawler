using HtmlAgilityPack;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Web;
using System.Text;
using System.Threading.Tasks;
using Web_Crawler.shared;

namespace Web_Crawler.shared
{
    /// <summary>
    /// Crawler
    /// </summary>
    static class Crawler
    {
        static WebRequest myWebRequest;
        static WebResponse myWebResponse;
        static HtmlDocument htmlDoc;
        static Webcrawler webcrawler = new Webcrawler();
        static RemoveWords wordReplace;
        static string baseURL;
        static string OutputDirectory = string.Empty;
        static string imageDownloadPath = string.Empty;
        static bool webcrawlerStart;

        /// <summary>
        /// Retrive the data from URL and create the MD file
        /// </summary>
        /// <param name="URL"></param>
        /// <param name="Path"></param>
        public static bool readData(string URL, string Path)
        {
            try
            {
                OutputDirectory = Environment.CurrentDirectory + "\\Output";

                if(!System.IO.Directory.Exists(OutputDirectory))
                    System.IO.Directory.CreateDirectory(OutputDirectory);
                else
                {
                    System.IO.Directory.Delete(OutputDirectory, true);
                    System.IO.Directory.CreateDirectory(OutputDirectory);
                }
                htmlDoc = new HtmlDocument();
                webcrawler = (Webcrawler)loadJson();
                if(URL.EndsWith("/"))
                    URL = URL.Remove(URL.Length - 1);
                baseURL = URL;
                return getCategoryList(baseURL, Path);


            }
            catch(Exception ex) { createErrorLog(ex); }
            return false;
        }

        /// <summary>
        /// Get category list
        /// </summary>
        /// <param name="URL"></param>
        /// <param name="Path"></param>
        /// <returns></returns>
        private static bool getCategoryList(string URL, string Path)
        {
            try
            {
                webcrawlerStart = false;
                Uri uri = new Uri(URL);
                string filename = uri.Host.Replace('.', '_');
                imageDownloadPath = OutputDirectory + "\\" + filename + "\\Images";
                if(!System.IO.Directory.Exists(imageDownloadPath))
                    System.IO.Directory.CreateDirectory(imageDownloadPath);
                wordReplace = new RemoveWords(imageDownloadPath);
                htmlDoc.LoadHtml(getResponse(URL));
                string categoryfileName = filename + "\\" + getFileName(wordReplace.readValue(htmlDoc.QuerySelector(webcrawler.category.Selector), webcrawler.category.Text));
                if(!System.IO.Directory.Exists(OutputDirectory + "\\" + categoryfileName))
                    System.IO.Directory.CreateDirectory(OutputDirectory + "\\" + categoryfileName);
                if(htmlDoc.QuerySelector(webcrawler.category.Selector).HasChildNodes)
                {
                    webcrawlerStart = true;
                    if(htmlDoc.QuerySelectorAll(webcrawler.subcategorylist.Selector).Count > 0)
                    {
                        var subCategoryList = htmlDoc.QuerySelectorAll(webcrawler.subcategorylist.Selector);
                        using(var progress = new ProgressBar())
                        {
                            double increment = 100 / subCategoryList.Count;
                            double i = 0;
                            foreach(var item in subCategoryList)
                            {
                                progress.Report((double)i / 100);

                                var childNodeValue = baseURL + removeDuplicate(item.QuerySelector("a").Attributes[webcrawler.subcategorylist.Text].Value);

                                getSubcategory(childNodeValue, categoryfileName);
                                i = i + increment;
                            }
                        }


                    }
                }
                else if(htmlDoc.QuerySelector(webcrawler.subcategory.Selector).HasChildNodes)
                {
                    getSubcategory(URL, categoryfileName);
                }
                else if(htmlDoc.QuerySelectorAll(webcrawler.article.selector).Count > 0)
                {
                    getArticle(URL, categoryfileName);
                }
                if(webcrawlerStart)
                    genrateZipFile(startPath: OutputDirectory + "\\" + filename, zipPath: Path + "\\" + filename + ".zip");

            }
            catch(Exception ex)
            {
                createErrorLog(ex);
            }
            return webcrawlerStart;
        }

        private static string removeDuplicate(string URL)
        {
            Uri uri = new Uri(baseURL);
            string value = string.Empty;
            string[] arrayURL = URL.Split(new char[] { '/' });
            foreach(string val in arrayURL)
            {
                if(val != "")
                    if(!uri.Segments.Contains(val))
                        value += "/" + val;
            }
            return value;
        }

        /// <summary>
        /// Generate the zip file
        /// </summary>
        /// <param name="startPath"></param>
        /// <param name="zipPath"></param>
        private static void genrateZipFile(string startPath, string zipPath)
        {
            try
            {
                if(System.IO.Directory.Exists(zipPath))
                {
                    Console.WriteLine("File already exists. So enter alternative path:");
                    zipPath = Console.ReadLine();
                }
                ZipFile.CreateFromDirectory(startPath, zipPath);
            }
            catch(Exception ex) { createErrorLog(ex); }
        }


        /// <summary>
        /// Draw the progress bar 
        /// </summary>
        /// <param name="progress"></param>
        /// <param name="total"></param>
        private static void drawTextProgressBar(int progress, int total)
        {
            try
            {
                //draw empty progress bar
                Console.CursorLeft = 0;
                Console.Write("["); //start
                Console.CursorLeft = 32;
                Console.Write("]"); //end
                Console.CursorLeft = 1;
                float onechunk = 30.0f / total;

                //draw filled part
                int position = 1;
                for(int i = 0; i < onechunk * progress; i++)
                {
                    Console.BackgroundColor = ConsoleColor.Gray;
                    Console.CursorLeft = position++;
                    Console.Write(" ");
                }

                //draw unfilled part
                for(int i = position; i <= 31; i++)
                {
                    Console.BackgroundColor = ConsoleColor.Green;
                    Console.CursorLeft = position++;
                    Console.Write(" ");
                }

                //draw totals
                Console.CursorLeft = 35;
                Console.BackgroundColor = ConsoleColor.Black;
                Console.Write(progress.ToString() + " of " + total.ToString() + "    ");
            }
            catch(Exception ex)
            {
                createErrorLog(ex);

            }//blanks at the end remove any excess
        }


        /// <summary>
        /// Remove the special charactor in file name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private static string getFileName(string name)
        {
            string fileName = name;
            try
            {
                char[] _removeChar = new char[] { '/', '\\', '.', '!', '@', '#', '$', '%', '^', '&', '*', '(', ')', '?', '>', '<', ',', '{', '}', '[', ']', ':', ';', '_', '+', '=' };
                var data = _removeChar.Where(x => fileName.Contains(x)).ToList();
                if(data.Count > 0)
                {
                    foreach(char _char in data)
                    {
                        while(fileName.IndexOf(_char) != -1)
                        {
                            int _index = fileName.IndexOf(_char);
                            fileName = fileName.Remove(_index, 1);
                        }
                    }
                }
                fileName = fileName.Replace("-", " ");
            }
            catch(Exception ex)
            {

                createErrorLog(ex);
            }

            return fileName = (fileName.Length > 50 ? fileName.Substring(0, 50).Trim().Replace(" ", "---") : fileName.Replace(" ", "---"));
        }



        /// <summary>
        /// Create error log file
        /// </summary>
        /// <param name="ex"></param>
        private static void createErrorLog(Exception ex)
        {
            string path = string.Format(Environment.CurrentDirectory + "\\ErrorLog_{0}.txt", DateTime.Now.ToString("ddMMyyyy"));
            FileStream fileStream = new FileStream(path, FileMode.Append);
            StreamWriter streamWriter = new StreamWriter(fileStream);
            string message = string.Format("Time: {0}", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt"));
            message += Environment.NewLine;
            message += "-----------------------------------------------------------";
            message += Environment.NewLine;
            message += string.Format("Message: {0}", ex.Message);
            message += Environment.NewLine;
            message += string.Format("StackTrace: {0}", ex.StackTrace);
            message += Environment.NewLine;
            message += string.Format("Source: {0}", ex.Source);
            message += Environment.NewLine;
            message += string.Format("TargetSite: {0}", ex.TargetSite.ToString());
            message += Environment.NewLine;
            message += "-----------------------------------------------------------";
            message += Environment.NewLine;
            streamWriter.WriteLine(message);
            streamWriter.Close();
        }


        /// <summary>
        /// get subcategory
        /// </summary>
        /// <param name="href"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        private static void getSubcategory(string href, string path)
        {
            try
            {
                var readHtml = new HtmlDocument();
                readHtml.LoadHtml(getResponse(href));
                if(readHtml.QuerySelector(webcrawler.subcategory.Selector).InnerText.Length > 0)
                {
                    var subCategoryfileName = path + "\\" + getFileName(wordReplace.readValue(readHtml.QuerySelector(webcrawler.subcategory.Selector), webcrawler.subcategory.Text));
                    if(!System.IO.Directory.Exists(OutputDirectory + "\\" + subCategoryfileName))
                        System.IO.Directory.CreateDirectory(OutputDirectory + "\\" + subCategoryfileName);
                    if(readHtml.QuerySelectorAll(webcrawler.articlelist.Selector).Count > 0)
                    {
                        var articleList = readHtml.QuerySelectorAll(webcrawler.articlelist.Selector);
                        int i = 1;
                        foreach(var item in articleList)
                        {
                            var childRef = baseURL + removeDuplicate(item.QuerySelector("a").Attributes[webcrawler.articlelist.Text].Value);
                            string[] split = childRef.Split('/');
                            string articleFileName = getFileName(split.Last().ToString());
                            getArticle(childRef, subCategoryfileName + "\\" + (i++).ToString() + "_" + articleFileName + ".md");
                        }
                    }

                }
            }
            catch(Exception ex)
            {
                createErrorLog(ex);
            }
        }

        /// <summary>
        /// get article
        /// </summary>
        /// <param name="href"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private static void getArticle(string href, string fileName)
        {
            try
            {
                var readHtml = new HtmlDocument();
                readHtml.LoadHtml(getResponse(href));
                wordReplace.URL = new Uri(href);

                if(readHtml.QuerySelector(webcrawler.article.selector).InnerHtml.Length > 0)
                {
                    TextWriter textWriter = File.CreateText(OutputDirectory + "\\" + fileName);
                    textWriter.WriteLine("## " + wordReplace.readValue(readHtml.QuerySelector(webcrawler.article.title.Selector), webcrawler.article.title.Text));
                    var article = readHtml.QuerySelectorAll(webcrawler.article.article_info.Selector);
                    foreach(var item in article)
                    {
                        wordReplace.getInnerText(item, textWriter);
                    }
                    textWriter.WriteLine("\n\n\n-------------------------------------- end --------------------------------------\n");
                    textWriter.WriteLine(string.Format("Original Documents : [{0}]({1})", href, href));
                    textWriter.Close();
                }


            }
            catch(Exception ex) { createErrorLog(ex); }
        }

        /// <summary>
        /// Get the response from URL
        /// </summary>
        /// <param name="URL"></param>
        /// <returns>string</returns>
        private static string getResponse(string URL)
        {
            string res = string.Empty;
            try
            {
                myWebRequest = WebRequest.Create(URL);
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12 | SecurityProtocolType.Ssl3;
                myWebResponse = myWebRequest.GetResponse(); //Returns a response from an Internet resource
                Stream streamResponse = myWebResponse.GetResponseStream(); //return the data stream from the internet
                StreamReader sreader = new StreamReader(streamResponse); //reads the data stream
                res = sreader.ReadToEnd(); //reads it to the end
                streamResponse.Close();
                sreader.Close();
                myWebResponse.Close();
            }
            catch(Exception ex)
            {
                createErrorLog(ex);
            }
            return res;
        }

        /// <summary>
        /// Read json config file
        /// </summary>
        /// <returns>icategoryDetails</returns>
        private static object loadJson()
        {
            object categoryDetails;
            if(!File.Exists(Environment.CurrentDirectory + "\\config.json"))
            {
                TextWriter tw = File.CreateText(Environment.CurrentDirectory + "\\config.json");
                tw.WriteLine("{\"webcrawler\":{\"category\":{\"selector\":\"header.header > div.header__container > div.content > h1.header__headline\",\"text\": \"InnerText\"},{\"subcategorylist\": {\"selector\": \"div.g__space\",\"text\": \"href\"}},{\"article\": {\"article_info\": {\"selector\": \"section.section__article > div.paper__large > div.content > div.article > article\",\"text\": \"ChildNodes\"},\"descriptions\": {\"selector\": \"section.section__article > div.paper__large > div.content > div.article > div.article__meta > div.article__desc\",\"text\": \"InnerText\"},\"selector\": \"section.section__article\",\"title\": {\"selector\": \"section.section__article > div.paper__large > div.content > div.article > div.article__meta > h1.t__h1\",\"text\": \"InnerText\"}}},{\"articlelist\": {\"selector\": \"div.container > div.content > section.content > div.section__bg > div.g__space > a.paper__article-preview\",\"text\": \"href\"}},\"subcategory\": {\"selector\": \"section.section__article > div.paper__large > div.content > div.article > div.article__meta > h1.t__h1\",\"text\":\"InnerText\"}}}");
                //tw.WriteLine("{ \"article\": { \"article_info\": \"section.section__article > div.paper__large > div.content > div.article > article\",");
                //tw.WriteLine("\"avatar\": \"section.section__article > div.paper__large > div.content > div.article > div.article__meta > div.avatar\", ");
                //tw.WriteLine("\"descriptions\": \"section.section__article > div.paper__large > div.content > div.article > div.article__meta > div.article__desc\",");
                //tw.WriteLine("\"title\": \"section.section__article > div.paper__large > div.content > div.article > div.article__meta > h1.t__h1\" },");
                //tw.WriteLine("\"category\": { \"avatar\": { \"image\": \"a.paper > div.collection > div.collection_meta > div.avatar > div.avatar__photo > img\",");
                //tw.WriteLine("\"info\": \"a.paper > div.collection > div.collection_meta > div.avatar > div.avatar__info > div\",");
                //tw.WriteLine("\"selector\": \"a.paper >div.collection > div.collection_meta > div.avatar\" }, \"href\": \"div.g__space > a.paper\",");
                //tw.WriteLine("\"list\": \"section > div.g__space\", \"meta\": \"a.paper > div.collection > div.collection_meta > p.paper__preview\",");
                //tw.WriteLine("\"photo\": \"a.paper > div.collection > div.collection__photo > svg\", \"title\": \"a.paper > div.collection > div.collection_meta > h2.t__h3\" },");
                //tw.WriteLine("\"subcategory\": { \"avatar\": { \"image\": \"div.section__bg > div.g__space > a.paper__article-preview > div.article__preview > div.avatar > div.avatar__photo > img\",");
                //tw.WriteLine("\"info\": \"div.section__bg > div.g__space > a.paper__article-preview > div.article__preview > div.avatar > div.avatar__info > div\",");
                //tw.WriteLine("\"selector\": \"div.section__bg > div.g__space > a.paper__article-preview > div.article__preview > div.avatar\" },");
                //tw.WriteLine("\"href\": \"div.section__bg > div.g__space > a.paper__article-preview\",");
                //tw.WriteLine("\"selector\": \"div.container > div.content > section.content > div.section__bg > div.g__space > a.paper__article-preview\",");
                //tw.WriteLine("\"list\": \"div.g__space\", \"meta\": \" div.article__preview > span.paper__preview\", \"title\": \"div.article__preview > h2.t__h3 > span.c__primary\" },");
                //tw.WriteLine("\"page_title\": \"header.header > div.header__container > div.content > h1.header__headline\" }");
                tw.Close();
            }
            using(StreamReader r = new StreamReader(Environment.CurrentDirectory + "\\config.json"))
            {
                string json = r.ReadToEnd();
                categoryDetails = JsonConvert.DeserializeObject<Webcrawler>(json);
            }
            return categoryDetails;
        }

    }

}
