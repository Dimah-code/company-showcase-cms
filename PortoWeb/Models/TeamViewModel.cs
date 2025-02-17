using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PortoWeb.Models
{
    public class TeamViewModel
    {
        public List<View_TeamRole> wteam { get; set; }
        public List<Table_Roles> role { get; set; }
        public List<Table_Team> team { get; set; }
    }
}