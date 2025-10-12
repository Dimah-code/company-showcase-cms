using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PortoWeb.Helpers;
using PortoWeb.Models;

namespace PortoWeb.Controllers
{
    public class PagesController : Controller
    {
        PortoDB1 context = new PortoDB1();
        [AuthorizeAdmin]
        public ActionResult PagesList()
        {
            var pages = context.Table_Pages.ToList();
            ViewBag.pages = pages;
            return View();
        }
        [AuthorizeAdmin]
        public ActionResult EditPage(int id)
        {
            var page = context.Table_Pages.SingleOrDefault(p => p.pageID == id);
            ViewBag.page = page;
            return View();
        }
        [AuthorizeAdmin]
        public ActionResult EditPagePost(int pageID , string pageName , string pageTitle , string pageDes , string pageKeywords)
        {
            var page = context.Table_Pages.Find(pageID);

            if (page == null)
            {
                return Json(new { success = false, message = "صفحه مورد نظر یافت نشد! لطفا دوباره تلاش کنید." });
            }

            // Check for null or empty fields
            if (string.IsNullOrWhiteSpace(pageName) ||
                string.IsNullOrWhiteSpace(pageTitle) ||
                string.IsNullOrWhiteSpace(pageDes) ||
                string.IsNullOrWhiteSpace(pageKeywords))
            {
                return Json(new { success = false, message = "لطفا تمامی فیلدها را پر کنید!" });
            }

            // Update the page entity
            page.pageName = pageName;
            page.pageDes = pageDes;
            page.pageTitle = pageTitle;
            page.pageKeywords = pageKeywords;

            context.SaveChanges();

            return Json(new { success = true, message = "ویرایش صفحه با موفقیت انجام شد." });

        }
    }
}