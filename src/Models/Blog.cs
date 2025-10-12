using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PortoWeb.Models
{
    public class Blog
    {
        public int bID { get; set; }
        public string bName { get; set; }
        public string bKeywords { get; set; }
        public string language { get; set; }
        public int bWriter { get; set; }
        public int bCategory { get; set; }
        public string bTitle { get; set; }
        [AllowHtml] // Allow HTML content for this property
        public string bContent { get; set; }
        public HttpPostedFileBase mainPhoto { get; set; }
        public List<HttpPostedFileBase> additionalPhotos { get; set; }

    }
}