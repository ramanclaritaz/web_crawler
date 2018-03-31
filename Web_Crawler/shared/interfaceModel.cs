using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Web_Crawler.shared
{

    interface iavatar
    {
        string image { get; set; }
        string info { get; set; }
        string selector { get; set; }
    }
    interface icat
    {
        string href { get; set; }
        string meta { get; set; }
        string photo { get; set; }
        string title { get; set; }
        string list { get; set; }
        string selector { get; set; }
        iavatar avatar { get; set; }
        List<subCategory> subCategoryList { get; set; }
    }
    interface iarticle
    {
        string article_info { get; set; }
        string avatar { get; set; }
        string descriptions { get; set; }
        string title { get; set; }
    }
    interface icategoryDetails
    {
        string page_title { get; set; }
        icat category { get; set; }
        icat subcategory { get; set; }
        iarticle article { get; set; }
    }
    interface iSubCategory : icat
    {
        List<article> article { get; set; }
    }
    interface icategoryData
    {
        string page_title { get; set; }
        List<cat> categoryList { get; set; }
    }
}
