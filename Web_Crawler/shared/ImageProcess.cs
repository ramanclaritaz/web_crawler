using HtmlAgilityPack;
using Svg;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Web_Crawler.shared
{

    /// <summary>
    /// 
    /// </summary>
    public class ImageProcess
    {
        string _imagePath;
        int _incrementNumber;

        /// <summary>
        /// Set and Get ImagePath
        /// </summary>
        public string ImagePath { get => _imagePath; set => _imagePath = value; }

        /// <summary>
        /// Get and set for Increment number
        /// </summary>
        public int IncrementNumber { get => _incrementNumber; set => _incrementNumber = value; }



        /// <summary>
        /// Contructor for image process
        /// </summary>
        /// <param name="imagePath"></param>
        /// <param name="MaxNumber"></param>
        public ImageProcess(string imagePath, int MaxNumber)
        {
            _imagePath = imagePath;
            _incrementNumber = MaxNumber;
        }
        /// <summary>
        /// Contructor for image process
        /// </summary>
        /// <param name="imagePath"></param>
        public ImageProcess(string imagePath)
        {
            _imagePath = imagePath;
        }

        /// <summary>
        /// Download image from URL
        /// return file name
        /// </summary>
        /// <param name="URL"></param>
        /// <returns></returns>
        public string DownloadImage(string URL)
        {
            Uri uri = new Uri(URL);
            string Extentions = uri.Segments.Last();
            if(!Path.HasExtension(Extentions))
                Extentions = Extentions + ".jpg";
            Extentions = GenerateNumber() + "_" + Extentions;
            string fileName = ImagePath + "\\" + Extentions;

            using(WebClient Client = new WebClient())
            {
                Client.DownloadFile(URL, fileName);
            }

            return Extentions;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string ConvertSVGtoImage(HtmlNode htmlNode)
        {
            TextWriter textWriter = File.CreateText(Environment.CurrentDirectory + "\\" + "readsvg.svg");
            textWriter.WriteLine(htmlNode.OuterHtml);
            textWriter.Close();
            var svgDocument = SvgDocument.Open(Environment.CurrentDirectory + "\\readsvg.svg");
            string fileName = GenerateNumber() + "_SVGImage.png";
            string filePath = ImagePath + "\\" + fileName;
            using(var smallBitmap = svgDocument.Draw())
            {
                var width = smallBitmap.Width;
                var height = smallBitmap.Height;
                if(width != 2000)// I resize my bitmap
                {
                    width = 2000;
                    height = 2000 / smallBitmap.Width * height;
                }

                using(var bitmap = svgDocument.Draw(width, height))//I render again
                {
                    bitmap.Save(filePath, ImageFormat.Png);
                }
            }
            File.Delete(Environment.CurrentDirectory + "\\readsvg.svg");
            return fileName;

        }

        /// <summary>
        /// Generate auto number
        /// </summary>
        /// <returns></returns>
        public string GenerateNumber()
        {
            int prefix = 1000000;
            IncrementNumber++;
            return (prefix / Math.Pow(10, IncrementNumber.ToString().Length)).ToString() + IncrementNumber.ToString();

        }

    }

}
