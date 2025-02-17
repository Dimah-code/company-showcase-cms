using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PortoWeb.Models;
using PortoWeb.Helpers;
using System.IO;
using PortoWeb.Services;

namespace PortoWeb.Controllers
{
    public class ComponentController : Controller
    {
        PortoDB1 context = new PortoDB1();
        // GET: Component
        public string GetLanguage()
        {
            var langCookie = Request.Cookies["lang"];
            return langCookie?.Value ?? "fa";
        }
        private string GenerateUniqueFileName(string originalFileName)
        {
            var extension = Path.GetExtension(originalFileName);
            var uniqueName = Guid.NewGuid().ToString() + extension;
            return uniqueName;
        }

        public ActionResult _ProfileHeader()
        {
            var cookie = Request.Cookies["Ai"];
            if (cookie != null && cookie["MID"] != null)
            {
                int memberID = int.Parse(cookie["MID"]);
                var adminInfo = context.Table_Members.Where(d => d.MID == memberID).Single();
                var messages = context.Table_Messages.Take(5).ToList();
                var counter = messages.Count();
                ViewBag.messages = messages;
                ViewBag.counter = counter;
                return PartialView(adminInfo);
            }
            return RedirectToAction("LoginPage", "Admin");
        }
        public ActionResult _RandomProjects()
        {
            var randomProject = context.View_ProjectList.OrderBy(x => Guid.NewGuid()).Take(4).ToList();
            return PartialView(randomProject);
        }
        public ActionResult _footer()
        {
            GeneralViewModel model = new GeneralViewModel
            {
                siteinfo = context.Table_siteInfo.SingleOrDefault(x => x.pkID == 1)
            };
            var content = context.Table_BottomIndex.FirstOrDefault(x => x.pkID == 1);
            var lang = GetLanguage();
            ViewBag.Language = lang;
            var address = lang == "fa" ? model.siteinfo.address : content.AddressEn;
            ViewBag.address = address;
            return View(model);
        }
        public ActionResult _header()
        {
            var header = context.Table_Header.FirstOrDefault(h => h.pkID == 1);
            string lang = Request.Cookies["lang"]?.Value ?? "fa";
            var home = lang == "fa" ? header.homeFa : header.homeEn;
            var about = lang == "fa" ? header.aboutFa : header.aboutEn;
            var projects = lang == "fa" ? header.projectsFa : header.projectsEn;
            var contact = lang == "fa" ? header.contactFa : header.contactEn;
            var blog = lang == "fa" ? header.blogFa : header.blogEn;
            var team = lang == "fa" ? header.teamFa : header.teamEn;
            var logo = header.logoName;
            var logoAlt = header.logoAlt;
            ViewBag.alt = logoAlt;
            ViewBag.home = home; ViewBag.about = about;
            ViewBag.projects = projects; ViewBag.contact = contact;
            ViewBag.blog = blog; ViewBag.team = team;
            ViewBag.logo = logo;
            return View();
        }
        public ActionResult _banner()
        {
            var sitenameFa = context.Table_siteInfo.FirstOrDefault(t => t.pkID == 1);
            string lang = Request.Cookies["lang"]?.Value ?? "fa";            
            var banner = context.Table_Banner.FirstOrDefault(b => b.pkID == 1);
            var bannerImage = banner.bannerImage;
            var sitename = lang == "fa" ? sitenameFa.siteName : banner.siteNameEn;
            var bannerTitle = lang == "fa" ? banner.bannerTitleFa : banner.bannerTitleEn;
            var bannerAlt = banner.bannerAltFa;
            var buttonCaption = lang == "fa" ? banner.buttonCaptionFa : banner.buttonCaptionEn;
            ViewBag.image = bannerImage; ViewBag.bannerTitle = bannerTitle; ViewBag.bannerAlt = bannerAlt;ViewBag.buttonCaption = buttonCaption;
            ViewBag.sitename = sitename;
            return View();
        }
        public ActionResult _homeabout() 
        {
            var lang = Request.Cookies["lang"]?.Value ?? "fa";
            var content = context.Table_HomeAbout.FirstOrDefault(c => c.pkID == 1);
            var MainTitle = lang == "fa" ? content.MainTitleFa : content.MainTitleEn;
            var MainParag = lang == "fa" ? content.MainPgFa : content.MainPgEn;
            var Title1 = lang == "fa" ? content.Title01Fa : content.Title01En;
            var Title2 = lang == "fa" ? content.Title02Fa : content.Title02En;
            var Title3 = lang == "fa" ? content.Title03Fa : content.Title03En;
            var Title4 = lang == "fa" ? content.Title04Fa : content.Title04En;
            ViewBag.maintitle = MainTitle;ViewBag.mainparag = MainParag;
            ViewBag.t1 = Title1; ViewBag.t2 = Title2; ViewBag.t3 = Title3; ViewBag.t4 = Title4;

            return View();
        }
        public ActionResult _ProjTeamLink()
        {
            var projects = context.View_ProjectList.ToList();
            ViewBag.projects = projects;
            var team = context.View_TeamRole.ToList();
            ViewBag.team = team;
            var lang = Request.Cookies["lang"]?.Value ?? "fa";
            var content = context.Table_ProjTeam.FirstOrDefault(c => c.pkID == 1);
            ViewBag.Tproj = lang == "fa" ? content.Title1Fa : content.Title1En;
            ViewBag.btnCproj = lang == "fa" ? content.ButtonCaption1Fa : content.ButtonCaption1En;
            ViewBag.Tteam = lang == "fa" ? content.Title2Fa : content.Title2En;
            ViewBag.btnCteam = lang == "fa" ? content.ButtonCaption2Fa : content.ButtonCaption2En;

            return View();
        }
        public ActionResult _indexBottom()
        {
            GeneralViewModel model = new GeneralViewModel
            {
                siteinfo = context.Table_siteInfo.SingleOrDefault(s => s.pkID == 1)
            };
            var lang = GetLanguage();
            ViewBag.Language = lang;
            var content = context.Table_BottomIndex.FirstOrDefault(c => c.pkID == 1);
            ViewBag.WorkTime = lang == "fa" ? model.siteinfo.timeWork : content.TimeWorkEn;
            ViewBag.Address = lang == "fa" ? model.siteinfo.address : content.AddressEn;
            return View(model);
        }
        public ActionResult _AboutTitle()
        {
            var lang = GetLanguage();
            var content = context.Table_AboutTitle.FirstOrDefault(c => c.pkID == 1);
            var aboutTitle = lang == "fa" ? content.AbtTitleFa : content.AbtTitleEn;
            var aboutParag = lang == "fa" ? content.AbtParagFa : content.AbtParagEn;
            ViewBag.aboutTitle = aboutTitle; ViewBag.aboutParag = aboutParag;
            return View();
        }
        public ActionResult _3Slogan()
        {
            var lang = GetLanguage();
            var content = context.Table_TopOfCounter.FirstOrDefault(c => c.pkID == 1);
            var t1 = lang == "fa" ? content.t1fa : content.t1en;
            var t2 = lang == "fa" ? content.t2fa : content.t2en;
            var t3 = lang == "fa" ? content.t3fa : content.t3en;
            var p1 = lang == "fa" ? content.p1fa : content.p1en;
            var p2 = lang == "fa" ? content.p2fa : content.p2en;
            var p3 = lang == "fa" ? content.p3fa : content.p3en;
            ViewBag.t1 = t1; ViewBag.t2 = t2; ViewBag.t3 = t3;
            ViewBag.p1 = p1; ViewBag.p2 = p2; ViewBag.p3 = p3;
            return View();
        }
        public ActionResult _Counter()
        {
            // counter numbers
            var blogs = context.Table_Blogs.Count();
            var projects = context.Table_Projects.Count();
            var members = context.Table_Team.Count();
            ViewBag.blogs = blogs; ViewBag.projects = projects; ViewBag.members = members;
            // counter multi language texts
            var lang = GetLanguage();
            var content = context.Table_CounterTexts.FirstOrDefault(c => c.pkID == 1);
            var blogText = lang == "fa" ? content.blogCounterFa : content.blogCounterEn;
            var projectText = lang == "fa" ? content.projectCounterFa : content.projectCounterEn;
            var teamText = lang == "fa" ? content.teamCounterFa : content.teamCounterEn;
            ViewBag.blog = blogText; ViewBag.project = projectText; ViewBag.team = teamText;
            return View();
        }
        public ActionResult _AboutUs()
        {
            var lang = GetLanguage();
            var content = context.Table_UnderOFCounter.FirstOrDefault(c => c.pkID == 1);
            var photo = content.photoPath;
            var aboutTitle = lang == "fa" ? content.aboutTitleFa : content.aboutTitleEn;
            var aboutParag = lang == "fa" ? content.aboutParagFa : content.aboutParagEn;
            var aboutAlt = content.photoAlt;
            ViewBag.AboutAlt = aboutAlt;
            ViewBag.AboutPhoto = photo;ViewBag.abtTitle = aboutTitle; ViewBag.abtParag = aboutParag;
            return View();
        }
        public ActionResult _MessageToAdmin()
        {
            var lang = GetLanguage();
            ViewBag.lang = lang;
            return View();
        }
        public ActionResult _SubBanner(int id)
        {
            var content = context.Table_AboutTitle.FirstOrDefault(x => x.pkID == id);
            var menu = context.Table_Header.FirstOrDefault(x => x.pkID == 1);
            var lang = GetLanguage();
            string nowPage = null;
            switch(id)
            {
                case 1:  nowPage = lang == "fa" ? menu.aboutFa : menu.aboutEn;break;
                case 2:  nowPage = lang == "fa" ? menu.projectsFa : menu.projectsEn;break;
                case 3:  nowPage = lang == "fa" ? menu.contactFa : menu.contactEn; break;
                case 4:  nowPage = lang == "fa" ? menu.teamFa : menu.teamEn;break;
                case 5:  nowPage = lang == "fa" ? menu.blogFa : menu.blogEn;break;
            }
            ViewBag.id = id;
            var prevPage = lang == "fa" ? menu.homeFa : menu.homeEn;
            var slogan = lang == "fa" ? content.AbtSloganFa : content.AbtSloganEn;
            ViewBag.slogan = slogan; ViewBag.prevPage = prevPage; ViewBag.nowPage = nowPage;
            return View();
        }
        [AuthorizeAdmin]
        [HttpPost]
        public ActionResult EditContext(string enTxt , string faTxt, string elementId)
            {
            var response = Services.Finder.UpdateText(elementId, enTxt, faTxt);
            return Json(new { success = response.Success , message = response.Message });
        }
        [AuthorizeAdmin]
        [HttpPost]
        public ActionResult SetImage(string elementId ,string imageAlt, HttpPostedFileBase image)
        {
            string path = Server.MapPath("~/assets/img/gallery");
            var parts = elementId.Split('-');
            if (parts.Length < 2 || !int.TryParse(parts[1], out int ID))
                return Json(new { success = false });
            var uniqueName = GenerateUniqueFileName(image.FileName);
            var newPath = Path.Combine(path, uniqueName);
            if (ID == 1)
            {
                var header = context.Table_Header.FirstOrDefault();
                var oldPath = Path.Combine(path, header.logoName);
                if (System.IO.File.Exists(oldPath))
                {
                    System.IO.File.Delete(oldPath);
                }
                image.SaveAs(newPath);
                header.logoAlt = imageAlt;
                header.logoName = uniqueName;
                context.SaveChanges();
                return Json(new { success = true });
            }
            else if (ID == 2)
            {
                var banner = context.Table_Banner.FirstOrDefault();
                var oldPath = Path.Combine(path, banner.bannerImage);
                if (System.IO.File.Exists(oldPath))
                {
                    System.IO.File.Delete(oldPath);
                }
                image.SaveAs(newPath);
                banner.bannerAltFa = imageAlt;
                banner.bannerImage = uniqueName;
                context.SaveChanges();
                return Json(new { success = true });
            }
            else if (ID == 3)
            {
                var about = context.Table_UnderOFCounter.FirstOrDefault();
                var oldPath = Path.Combine(path, about.photoPath);
                if (System.IO.File.Exists(oldPath))
                {
                    System.IO.File.Delete(oldPath);
                }
                about.photoAlt = imageAlt;
                about.photoPath = uniqueName;
                image.SaveAs(newPath);
                context.SaveChanges();
                return Json(new { success = true });
            }
            else
            {
                return Json(new { success = false });
            }
        }
        public ActionResult _favicon()
        {
            var info = context.Table_siteInfo.FirstOrDefault();
            GeneralViewModel model = new GeneralViewModel
            {
                favicon = info.siteLogo
            };
            return View(model);
        }
    }
}