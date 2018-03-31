using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Web_Crawler
{
    class Program
    {

        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("*** Web Crawler ***");
                Console.Write("Enter your website URL :");
                string URL = Console.ReadLine();
                Crawler.readData(URL);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.ReadLine();
            }
            finally
            {

            }
        }



    }
}
