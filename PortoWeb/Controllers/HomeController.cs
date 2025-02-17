using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PortoWeb.Models;

namespace PortoWeb.Controllers
{
    public class HomeController : Controller
    {
        PortoDB1 context = new PortoDB1();
        public string GetLanguage()
        {
            var langCookie = Request.Cookies["lang"];
            return langCookie?.Value ?? "fa";
        }
        public ActionResult Index()
        {
            GeneralViewModel model = new GeneralViewModel
            {
                siteinfo = context.Table_siteInfo.SingleOrDefault(s => s.pkID == 1),
                page = context.Table_Pages.FirstOrDefault(p => p.pageID == 1)
            };

            return View(model);
        }

        public ActionResult About()
        {
            GeneralViewModel model = new GeneralViewModel
            {
                page = context.Table_Pages.FirstOrDefault(p => p.pageID == 6),
                siteinfo = context.Table_siteInfo.SingleOrDefault(s => s.pkID == 1)
            };
            return View(model);
        }
        public ActionResult Projects()
        {
            var lang = GetLanguage();
            ViewBag.lang = lang;
            GeneralViewModel model = new GeneralViewModel
            {
                siteinfo = context.Table_siteInfo.SingleOrDefault(s => s.pkID == 1),
                page = context.Table_Pages.FirstOrDefault(p => p.pageID == 3),
                projects = context.View_ProjectList.Where(p => p.lang == lang).OrderByDescending(p => p.pID).ToList(),
                category = context.Table_Category.ToList()
            };
            return View(model);
        }
        public ActionResult ProjectDetail(int id)
        {
            var lang = GetLanguage();
            ViewBag.lang = lang;
            var project = context.Table_Projects.Find(id);
            // Create the view model
            GeneralViewModel model = new GeneralViewModel
            {
                siteinfo = context.Table_siteInfo.SingleOrDefault(s => s.pkID == 1),
                projectDetail = context.Table_Projects.SingleOrDefault(p => p.pID == id),
                projectImages = context.Table_ProjectImage.Where(p => p.projectID == id).ToList(),
                viewProjectDetail = context.View_ProjectList.SingleOrDefault(p => p.pID == id),
                page = context.Table_Pages.SingleOrDefault(p => p.pageID == 7)
            };
            var relatedProjects = context.View_ProjectList.Where(p => p.pCategory == project.pCategory && p.lang == lang).ToList();
            var fourProjects = relatedProjects.OrderBy(p => Guid.NewGuid()).Take(4).ToList();
            ViewBag.fourProjects = fourProjects;
            ViewBag.PersianMonths = new string[]
            {
                "فروردین", "اردیبهشت", "خرداد", "تیر", "مرداد", "شهریور",
                "مهر", "آبان", "آذر", "دی", "بهمن", "اسفند"
            };
            return View(model);
        }

        public ActionResult Contact()
        {
            GeneralViewModel model = new GeneralViewModel
            {
                page = context.Table_Pages.SingleOrDefault(p => p.pageID == 2),
                siteinfo = context.Table_siteInfo.SingleOrDefault(s => s.pkID == 1)
            };
            var lang = GetLanguage();
            ViewBag.lang = lang;
            var informationFa = context.Table_siteInfo.FirstOrDefault(x => x.pkID == 1);
            var informationEn = context.Table_BottomIndex.FirstOrDefault(x => x.pkID == 1);
            var address = lang == "fa" ? informationFa.address : informationEn.AddressEn;
            var timeWork = lang == "fa" ? informationFa.timeWork : informationEn.TimeWorkEn;
            var email = informationFa.siteEmail;
            var phone = informationFa.sitePhone;
            ViewBag.timework = timeWork; ViewBag.address = address;
            return View(model);
        }
        public ActionResult Team()
        {
            var lang = GetLanguage();
            ViewBag.lang = lang;
            GeneralViewModel model = new GeneralViewModel
            {
                siteinfo = context.Table_siteInfo.SingleOrDefault(s => s.pkID == 1),
                page = context.Table_Pages.SingleOrDefault(p => p.pageID == 5),
                roles = context.Table_Roles.ToList(),
                team = context.View_TeamRole.Where(t => t.lang == lang).ToList()
            };
            return View(model);
        }
        public ActionResult TeamDetail(int id)
        {
            var lang = GetLanguage();
            ViewBag.lang = lang;
            var Member = context.Table_Team.Find(id);
            if (Member != null)
            {
                GeneralViewModel model = new GeneralViewModel
                {
                    siteinfo = context.Table_siteInfo.SingleOrDefault(s => s.pkID == 1),
                    page = context.Table_Pages.FirstOrDefault(p => p.pageID == 9),
                    thisteam = context.View_TeamRole.SingleOrDefault(t => t.tID == Member.tID)
                };
                return View(model);
            }
            return RedirectToAction("Team");
        }
        public ActionResult Blogs()
        {
            var lang = GetLanguage();
            ViewBag.lang = lang;
            GeneralViewModel model = new GeneralViewModel
            {
                siteinfo = context.Table_siteInfo.SingleOrDefault(s => s.pkID == 1),
                page = context.Table_Pages.SingleOrDefault(p => p.pageID == 4),
                blogCategory = context.Table_BlogCategory.ToList(),
                blogs = context.View_BlogNcat.Where(b=> b.language == lang).OrderByDescending(b => b.bID).ToList()
            };
            return View(model);
        }
        public ActionResult BlogDetail(int id)
        {
            var lang = GetLanguage();
            ViewBag.lang = lang;
            var blog = context.Table_Blogs.Find(id);
            if (blog != null)
            {
                GeneralViewModel model = new GeneralViewModel
                {
                    siteinfo = context.Table_siteInfo.SingleOrDefault(s => s.pkID == 1),
                    page = context.Table_Pages.FirstOrDefault(p => p.pageID == 8),
                    blogDetail = context.View_BlogNcat.SingleOrDefault(b => b.bID == blog.bID),
                    blogImages = context.Table_BlogImages.Where(b => b.blogID == blog.bID).ToList()
                };
                var relatedBlogs = context.View_BlogNcat.Where(b => b.bCategory == blog.bCategory && b.language == lang).ToList();
                var fourBlogs = relatedBlogs.OrderBy(b => Guid.NewGuid()).Take(4).ToList();
                ViewBag.fourBlogs = fourBlogs;
                ViewBag.PersianMonths = new string[]
                {
                "فروردین", "اردیبهشت", "خرداد", "تیر", "مرداد", "شهریور",
                "مهر", "آبان", "آذر", "دی", "بهمن", "اسفند"
                };
                return View(model);
            }
            return RedirectToAction("Index");
        }
        public ActionResult SelectLanguage()
        {
            return View();
        }

        [HttpGet]
        public ActionResult SetLanguage(string language)
        {
            // Save the language in a cookie
            HttpCookie langCookie = new HttpCookie("lang", language);
            langCookie.Expires = DateTime.Now.AddYears(1); // Cookie valid for 1 year
            Response.Cookies.Add(langCookie);

            // Redirect to the home page after setting the language
            return RedirectToAction("Index");
        }
    }
}