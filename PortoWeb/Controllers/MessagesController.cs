using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PortoWeb.Models;
using PortoWeb.Helpers;
namespace PortoWeb.Controllers
{
    public class MessagesController : Controller
    {
        PortoDB1 context = new PortoDB1();
        [AuthorizeAdmin]
        public ActionResult MessagesList()
        {
            var model = new MessagesClass();
            model.MessagesList = context.Table_Messages.ToList();
            return View(model);
        }
        [HttpPost]
        [AuthorizeAdmin]
        public ActionResult DeleteMessage(int id)
        {
            var Message = context.Table_Messages.Find(id);
            try
            {
                if (Message != null)
                {
                    context.Table_Messages.Remove(Message);
                    context.SaveChanges();
                    return Json(new { success = true, message = "پیام مورد نظر حذف شد" });
                }
                return Json(new { success = false, message = "پیام یافت نشد" });
            }
            catch (Exception e)
            {
                return Json(new { success = false, message = e.Message });
            }
        }
    }
}