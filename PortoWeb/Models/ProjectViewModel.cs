using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PortoWeb.Models
{
    public class ProjectViewModel
    {
        public int pid { get; set; }
        public List<View_ProjectList> pList { get; set; }
        public List<Table_Category> cList { get; set; }
        public List<Table_ProjectImage> pImage { get; set; }

    }
}