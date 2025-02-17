using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PortoWeb.Models;

namespace PortoWeb.Models
{
    public class GeneralViewModel
    {
        public Table_Pages page { get; set; }
        public List<View_ProjectList> projects { get; set; }
        public List<Table_Category> category { get; set; }
        public List<View_TeamRole> team { get; set; }
        public View_TeamRole thisteam { get; set; }
        public Table_Team member { get; set; }
        public List<Table_Roles> roles { get; set; }
        public Table_siteInfo siteinfo { get; set; }
        public string favicon { get; set; }
        public Table_Projects projectDetail { get; set; }
        public List<Table_ProjectImage> projectImages { get; set; }
        public View_ProjectList viewProjectDetail { get; set; }
        public List<View_BlogNcat> blogs { get; set; }
        public View_BlogNcat blogDetail { get; set; }
        public List<Table_BlogImages> blogImages { get; set; }
        public List<Table_BlogCategory> blogCategory { get; set; }
    }
}