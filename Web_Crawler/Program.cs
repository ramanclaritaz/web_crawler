using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Web_Crawler.shared;

namespace Web_Crawler
{
    class Program
    {
        static string OutPutPath = string.Empty, URL = string.Empty;
        static ImageProcess imageProcess = new ImageProcess(Environment.CurrentDirectory);
        static void Main(string[] args)
        {
            Console.WriteLine("***\tBizTalk - Developed By Claritaz Techlabs\t***");
            Console.WriteLine("If you want stop press ctrl + c");
            getDatafromUser();

        }

        static void getDatafromUser()
        {
            if(string.IsNullOrEmpty(URL))
            {
                Console.Write("Enter your website URL :");
                URL = Console.ReadLine();
                if(!Uri.IsWellFormedUriString(URL, UriKind.Absolute))
                {
                    URL = string.Empty;
                    Console.WriteLine("Enter valid url");
                    getDatafromUser();
                }
            }
            if(string.IsNullOrEmpty(OutPutPath))
            {
                Console.Write("Enter the output path :");
                OutPutPath = Console.ReadLine();
                if(!System.IO.Directory.Exists(OutPutPath))
                {
                    OutPutPath = string.Empty;
                    Console.WriteLine("Path does not exists");
                    getDatafromUser();
                }
            }
            if(Crawler.readData(URL, OutPutPath))
            {
                Console.WriteLine("\n");
                Console.WriteLine("Sucessfully Created...! ");
            }
            else
            {
                string path = string.Format(Environment.CurrentDirectory + "\\ErrorLog_{0}.txt", DateTime.Now.ToString("ddMMyyyy"));
                Console.WriteLine("\n Error has occured. please check error log file. " + path);

            }
            exit();
        }
        private static void exit()
        {
            Console.WriteLine("Do you want continue? Y/N");
            switch(Console.ReadLine())
            {
                case "Y":
                case "y":
                    URL = OutPutPath = string.Empty;
                    getDatafromUser();
                    break;
                case "n":
                case "N":
                default:
                    Environment.Exit(0);
                    break;

            }
        }


    }
}
