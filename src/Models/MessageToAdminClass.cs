using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PortoWeb.Models
{
    public class MessageToAdminClass
    {
        public int pkID { get; set; }
        public string userName { get; set; }
        public string userPhone { get; set; }
        public string userMessage { get; set; }
    }
}