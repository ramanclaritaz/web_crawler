using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Web_Crawler.shared
{
    class article : iarticle
    {
        string _article_info;
        string _avatar;
        string _descriptions;
        string _title;
        public string article_info
        {
            get { return _article_info; }
            set { _article_info = value; }
        }

        public string avatar
        {
            get { return _avatar; }
            set { _avatar = value; }
        }

        public string descriptions
        {
            get { return _descriptions; }
            set { _descriptions = value; }
        }

        public string title
        {
            get { return _title; }
            set { _title = value; }
        }
        public article() { }
    }
    class subCategory : iSubCategory
    {

        string _href;
        string _list;
        string _meta;
        string _photo;
        string _title;
        string _selector;
        iavatar _avatar;
        List<article> _article;
        List<subCategory> _subCategoryList;
        public List<article> article
        {
            get { return _article; }
            set { _article = value; }
        }
        public iavatar avatar
        {
            get { return _avatar; }
            set { _avatar = value; }
        }
        public string href
        {
            get { return _href; }
            set { _href = value; }
        }

        public string list
        {
            get { return _list; }
            set { _list = value; }
        }

        public string meta
        {
            get { return _meta; }
            set { _meta = value; }
        }

        public string photo
        {
            get { return _photo; }
            set { _photo = value; }
        }

        public string title
        {
            get { return _title; }
            set { _title = value; }
        }

        public List<subCategory> subCategoryList
        {
            get { return _subCategoryList; }
            set { _subCategoryList = value; }
        }

        public string selector
        {
            get
            {
                return _selector;
            }

            set
            {
                _selector = value;
            }
        }

        public subCategory()
        {
            _avatar = new avatar();
            _article = new List<article>();
        }
    }
    class cat : icat
    {
        string _href;
        string _meta;
        string _photo;
        string _title;
        string _list;
        string _selector;
        iavatar _avatar;
        List<subCategory> _subCategoryList;
        public string href
        {
            get { return _href; }
            set { _href = value; }
        }

        public string list
        {
            get { return _list; }
            set { _list = value; }
        }

        public string meta
        {
            get { return _meta; }
            set { _meta = value; }
        }

        public string photo
        {
            get { return _photo; }
            set { _photo = value; }
        }

        public string title
        {
            get { return _title; }
            set { _title = value; }
        }

        public iavatar avatar
        {
            get { return _avatar; }
            set { _avatar = value; }
        }

        public List<subCategory> subCategoryList
        {
            get { return _subCategoryList; }
            set { _subCategoryList = value; }
        }

        public string selector
        {
            get
            {
                return _selector;
            }

            set
            {
                _selector = value;
            }
        }

        public cat()
        {
            _avatar = new avatar();
            _subCategoryList = new List<subCategory>();
        }

    }
    class avatar : iavatar
    {
        private string _image;
        private string _info;
        private string _selector;
        public string image
        {
            get { return _image; }
            set { _image = value; }
        }

        public string info
        {
            get { return _info; }
            set { _info = value; }
        }

        public string selector
        {
            get { return _selector; }
            set { _selector = value; }
        }
        public avatar() { }
    }

    class categoryDetails : icategoryDetails
    {
        string _page_title;
        icat _category;
        icat _subcategory;
        iarticle _article;

        public string page_title
        {
            get { return _page_title; }
            set { _page_title = value; }
        }

        public icat category
        {
            get { return _category; }
            set { _category = value; }
        }

        public icat subcategory
        {
            get { return _subcategory; }
            set { _subcategory = value; }
        }

        public iarticle article
        {
            get { return _article; }
            set { _article = value; }
        }
        public categoryDetails()
        {
            _category = new cat();
            _subcategory = new cat();
            _article = new article();
        }
    }
    class categoryData : icategoryData
    {
        List<cat> _categoryList;
        string _page_title;
        public List<cat> categoryList
        {
            get
            {
                return _categoryList;
            }

            set
            {
                _categoryList = value;
            }
        }

        public string page_title
        {
            get
            {
                return _page_title;
            }

            set
            {
                _page_title = value;
            }
        }
        public categoryData()
        {
            _categoryList = new List<cat>();
        }
    }
}
