using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PortoWeb.Models;
using PortoWeb.Helpers;
using System.IO;
using System.Net.Mail;
using System.Net;
using System.Threading.Tasks;
using System.Web.UI.WebControls;

namespace PortoWeb.Controllers
{
    public class AdminController : Controller
    {
        PortoDB1 context = new PortoDB1();
        // GET: Admin
        [AuthorizeAdmin]
        private string GenerateUniqueFileName(string originalFileName)
        {
            var extension = Path.GetExtension(originalFileName);
            var uniqueName = Guid.NewGuid().ToString() + extension;
            return uniqueName;
        }
        public string GetLanguage()
        {
            var langCookie = Request.Cookies["lang"];
            return langCookie?.Value ?? "fa";
        }
        [HttpGet]
        [AuthorizeAdmin]
        public ActionResult Dashboard()
        {
            ViewBag.projects = context.View_ProjectList.OrderBy(Guid => new Guid()).Take(8).ToList();
            ViewBag.blogs = context.View_BlogNcat.OrderBy(Guid => new Guid()).Take(8).ToList();
            ViewBag.messages = context.Table_Messages.OrderByDescending(m => m.pkID).ToList();
            ViewBag.blogCounter = context.Table_Blogs.Count();
            ViewBag.projectCounter = context.Table_Projects.Count();
            ViewBag.teamCounter = context.Table_Team.Count();
            return View();
        }
        [HttpGet]
        public ActionResult LoginPage()
        {
            return View();
        }
        [HttpPost]
        public ActionResult LoginCheck(string username, string password)
        {
            var admin = context.Table_Members.SingleOrDefault(a => a.Musername == username && a.Mpassword == password);
            if (admin != null)
            {
                // Set an encrypted cookie (store "admin" to match `AuthorizeAdmin` check)
                CookieHelper.SetEncryptedCookie("Aa", "a", Response);
                Request.Cookies["Aa"].Expires = DateTime.Now.AddDays(30);
                var cookie = new HttpCookie("Ai");
                cookie["MID"] = admin.MID.ToString();
                cookie.Expires = DateTime.Now.AddDays(30);
                Response.Cookies.Add(cookie);
                // Return a success JSON response
                return Json(new { success = true, message = "ورود موفق" });
            }

            // Return failure JSON response
            return Json(new { success = false, message = "نام کاربری یا رمز عبور اشتباه است" });
        }
        [HttpGet]
        [AuthorizeAdmin]
        public ActionResult ProfilePage()
        {
            var cookie = Request.Cookies["Ai"];
            if (cookie != null && cookie["MID"] != null)
            {
                int memberID = int.Parse(cookie["MID"]);
                var adminInfo = context.Table_Members.Where(d => d.MID == memberID).Single();
                return View(adminInfo);
            }
            return RedirectToAction("LoginPage", "Admin");
        }
        [HttpPost]
        [AuthorizeAdmin]
        public ActionResult ChangePassword(string currentUsername, string currentPassword, string newUsername, string newPassword)
        {
            // Fetch the current user from the database
            var user = context.Table_Members.FirstOrDefault(u => u.Musername == currentUsername);

            if (user == null)
            {
                return Json(new { success = false, message = "نام کاربری یافت نشد. لطفا نام کاریری خود را با دقت وارد کنید" });
            }
            if (currentUsername != user.Musername || currentPassword != user.Mpassword)
            {
                return Json(new { success = false, message = "رمز عبور یا نام کاربری فعلی را اشتباه وارد کردید" });
            }
            if (string.IsNullOrEmpty(newUsername) || string.IsNullOrWhiteSpace(newUsername))
            {
                user.Musername = user.Musername;
            }

            // Update username if provided
            if (!string.IsNullOrWhiteSpace(newUsername))
            {
                user.Musername = newUsername;
            }


            // Update password
            if (!string.IsNullOrWhiteSpace(newPassword))
            {
                user.Mpassword = newPassword; // Adjust to your hashing logic
            }

            // Save changes
            context.SaveChanges();

            return Json(new { success = true, message = "یه روزرسانی اطلاعات با موفقیت انجام شد" });
        }
        [HttpPost]
        [AuthorizeAdmin]
        public ActionResult EditProfile(Table_Members editedProfile, HttpPostedFileBase ProfilePic)
        {
            if (string.IsNullOrEmpty(editedProfile.MnameFamily) || string.IsNullOrWhiteSpace(editedProfile.MnameFamily))
            {
                return Json(new { success = false, message = "لطفا تمامی ورودی هارا پر کنید!" });
            }
            var cookie = Request.Cookies["Ai"];
            if (cookie != null && cookie["MID"] != null)
            {
                int memberID = int.Parse(cookie["MID"]);
                var adminInfo = context.Table_Members.SingleOrDefault(d => d.MID == memberID);

                if (adminInfo != null)
                {
                    // Update fields
                    adminInfo.MnameFamily = editedProfile.MnameFamily;
                    adminInfo.Mdes = editedProfile.Mdes;
                    adminInfo.Mrole = editedProfile.Mrole;
                    adminInfo.Mphone = editedProfile.Mphone;
                    adminInfo.Memail = editedProfile.Memail;
                    adminInfo.Minstagram = editedProfile.Minstagram;
                    adminInfo.Mlinkedin = editedProfile.Mlinkedin;

                    // Handle profile picture upload
                    if (ProfilePic != null && ProfilePic.ContentLength > 0)
                    {
                        var fileExtension = Path.GetExtension(ProfilePic.FileName);
                        var customFileName = $"Profile_{adminInfo.MID}_{DateTime.Now.ToString("yyyyMMddHHmmss")}{fileExtension}";
                        var path = Path.Combine(Server.MapPath("~/Admin_assets/img/AdminProfile"), customFileName);
                        var oldpath = Path.Combine(Server.MapPath("~/Admin_assets/img/AdminProfile"), adminInfo.Mpic);
                        if (System.IO.File.Exists(oldpath))
                        {
                            System.IO.File.Delete(oldpath);
                        }
                        ProfilePic.SaveAs(path);
                        adminInfo.Mpic = customFileName; // Save the filename to the database
                    }

                    // Save changes
                    context.SaveChanges();

                    return Json(new { success = true, message = "ویرایش با موفقیت انجام شد" });
                }
            }

            return Json(new { success = false, message = "درخواست نامعتبر یا کاربر یافت نشد" });
        }

        // View Edit Site Information
        [HttpGet]
        [AuthorizeAdmin]
        public ActionResult editSiteInfo()
        {
            var info = context.Table_siteInfo.SingleOrDefault();
            return View(info);
        }
        [HttpPost]
        [AuthorizeAdmin]
        [ValidateInput(false)]
        public ActionResult editInfo(
            string siteName,
            string address,
            string timeWork,
            string siteEmail,
            string sitePhone,
            string siteLinkedin,
            string siteYT,
            string siteInstagram,
            string googleMap,
            HttpPostedFileBase siteLogo
            )
        {
            var main = context.Table_siteInfo.SingleOrDefault(m => m.pkID == 1);
            if (siteLogo != null)
            {
                if (siteLogo == null || siteLogo.ContentLength == 0)
                {
                    return Json(new { success = false, message = "لطفاً یک فایل معتبر انتخاب کنید." });
                }

                // Validate file type
                string[] allowedExtensions = { ".ico", ".png", ".jpg", ".svg" };
                string fileExtension = Path.GetExtension(siteLogo.FileName).ToLower();
                string uniqefilename = GenerateUniqueFileName(siteLogo.FileName);
                if (!allowedExtensions.Contains(fileExtension))
                {
                    return Json(new { success = false, message = "فرمت فایل مجاز نیست! فقط ICO, PNG, JPG, SVG مجاز است." });
                }

                // Limit file size (Max 100KB)
                if (siteLogo.ContentLength > 102400)
                {
                    return Json(new { success = false, message = "حجم فایل نباید بیشتر از 100KB باشد!" });
                }
                
                // Save file
                string path = Path.Combine(Server.MapPath("~/Assets/img/icons"), uniqefilename);
                siteLogo.SaveAs(path);
                main.siteLogo = uniqefilename;
            }
            
            if (main != null)
            {
                main.siteName = siteName;
                main.address = address;
                main.timeWork = timeWork;
                main.siteEmail = siteEmail;
                main.sitePhone = sitePhone;
                main.siteLinkedin = siteLinkedin;
                main.siteYT = siteYT;
                main.siteInstagram = siteInstagram;
                main.googleMap = googleMap;
                context.SaveChanges();
                return Json(new { success = true, message = "مقادیر با موفقیت بروزرسانی شد" });
            }
            return Json(new { success = false, message = "مشکلی در بروزرسانی اطلاعات رخ داد" });
        }
        [HttpGet]
        [AuthorizeAdmin]
        public ActionResult HelpMikay()
        {
            return View();
        }
        [AuthorizeAdmin]
        public ActionResult Signout()
        {

            // Clear cookies
            if (Request.Cookies["Aa"] != null)
            {
                var cookie1 = new HttpCookie("Aa")
                {
                    Expires = DateTime.Now.AddDays(-1)
                };
                Response.Cookies.Add(cookie1);
            }

            if (Request.Cookies["Ai"] != null)
            {
                var cookie2 = new HttpCookie("Ai")
                {
                    Expires = DateTime.Now.AddDays(-1)
                };
                Response.Cookies.Add(cookie2);
            }

            // Clear session
            Session.Clear();
            Session.Abandon();

            // Sign out and redirect
            System.Web.Security.FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }
        [HttpPost]
        public ActionResult SendMessage(MessageToAdminClass model)
        {
            var lang = GetLanguage();
            int limit = 40;
            var messages = context.Table_Messages.Count();
            if (limit > messages)
            {
                var newMessage = new Table_Messages
                {
                    name = model.userName,
                    phone = model.userPhone,
                    message = model.userMessage,
                    CreatedAt = DateTime.Now
                };
                context.Table_Messages.Add(newMessage);
                context.SaveChanges();
                return Json(new { success = true, lang = lang });
            }
            else
            {
                return Json(new { success = false, lang = lang });
            }
        }
        [AuthorizeAdmin]
        [HttpGet]
        public ActionResult EditContext()
        {
            return View();
        }
        [AuthorizeAdmin]
        public ActionResult EditSession(bool handle)
        {
            if (handle)
            {
                Session["Edit"] = true;
            }
            else
            {
                Session["Edit"] = false;
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }
        [AuthorizeAdmin]
        [HttpGet]
        public ActionResult ForgotPassword()
        {
            ViewBag.user = context.Table_Members.FirstOrDefault();
            return View();
        }
        [AuthorizeAdmin]
        [HttpGet]
        public ActionResult SendEmail()
        {
            try
            {
                var user = context.Table_Members.FirstOrDefault();
                string userEmail = user.Memail.ToString();
                // ✅ Step 1: Define email settings
                string senderEmail = "portoportoweb@gmail.com";  // Your Gmail address
                string senderPassword = "xiny csal uitp emla";  // Your actual Gmail password
                string recipientEmail = userEmail;  // The recipient

                string emailBody = $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{
            font-family: Arial, sans-serif;
            background-color: #f4f4f9;
            color: #333;
            padding: 20px;
        }}
        .email-container {{
            background-color: #ffffff;
            border-radius: 10px;
            box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
            padding: 30px;
            text-align: center;
        }}
        h2 {{
            color: #4CAF50;
        }}
        p {{
            font-size: 16px;
            line-height: 1.6;
            color: #555;
        }}
        .btn {{
            display: inline-block;
            background-color: #4CAF50;
            color: #fff;
            padding: 10px 20px;
            text-decoration: none;
            border-radius: 5px;
            font-weight: bold;
        }}
        .footer {{
            margin-top: 20px;
            font-size: 12px;
            color: #888;
        }}
    </style>
</head>
<body>
    <div class='email-container'>
        <h2>سلام {user.MnameFamily} عزیز</h2>
        <p>لطفا پس از دریافت نام کاریری و رمزعبور هرچه سریع تر اقدام به تغییر آن کنید!!!</p>
        <p>نام کاربری: {user.Musername}</p>
        <p>رمزعبور: {user.Mpassword}</p>
        
    </div>
</body>
</html>
";
                // ✅ Step 2: Create the email
                MailMessage mail = new MailMessage
                {
                    From = new MailAddress(senderEmail),
                    Subject = "بازیابی رمز عبور",
                    Body = emailBody,
                    IsBodyHtml = true
                };
                mail.To.Add(recipientEmail);

                // ✅ Step 3: Set up SMTP (Gmail SMTP server)
                SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587)
                {
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(senderEmail, senderPassword),
                    EnableSsl = true
                };

                // ✅ Step 4: Send the email
                smtp.Send(mail);

                return Json(new { success = true, message = "ایمیل با موفقیت ارسال شد! در صورت عدم مشاهده پوشه اسپم را باز کنید." }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "در صورت ادامه یافتن این ارور به پشتیبانی پیام بدهید." + "Error: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpGet]
        public ActionResult RecoveryPage()
        {
            return View();
        }
        [HttpPost]
        public ActionResult RecoveryPost(string userEmail)
        {
            if (Session["IsRecoveryTime"] != null)
            {
                DateTime lastTime = (DateTime)Session["IsRecoveryTime"];
                if (DateTime.Now < lastTime.AddHours(2))
                {
                    return Json(new { success = false, message = "شما هر دو ساعت اجازه ارسال ایمیل بازیابی را دارید." });
                }
            }
            try
            {
                var user = context.Table_Members.FirstOrDefault();
                string userEmailmain = user.Memail.ToString();
                if (userEmail != userEmailmain)
                {
                    return Json(new { success = false, message = "ایمیل ارسالی با ایمیل ثبت شده در اطلاعات همخوانی ندارد! لطفا ایمیل خود را با دقت وارد کنید." });
                }
                // ✅ Step 1: Define email settings
                string senderEmail = "portoportoweb@gmail.com";  // Your Gmail address
                string senderPassword = "xiny csal uitp emla";  // Your actual Gmail password
                string recipientEmail = userEmailmain;  // The recipient

                string emailBody = $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{
            font-family: Arial, sans-serif;
            background-color: #f4f4f9;
            color: #333;
            padding: 20px;
        }}
        .email-container {{
            background-color: #ffffff;
            border-radius: 10px;
            box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
            padding: 30px;
            text-align: center;
        }}
        h2 {{
            color: #4CAF50;
        }}
        p {{
            font-size: 16px;
            line-height: 1.6;
            color: #555;
        }}
        .btn {{
            display: inline-block;
            background-color: #4CAF50;
            color: #fff;
            padding: 10px 20px;
            text-decoration: none;
            border-radius: 5px;
            font-weight: bold;
        }}
        .footer {{
            margin-top: 20px;
            font-size: 12px;
            color: #888;
        }}
    </style>
</head>
<body>
    <div class='email-container'>
        <h2>سلام {user.MnameFamily} عزیز</h2>
        <p>در صورتی که این درخواست از طرف شما نیست هرچه زودتر اقدام به تغییر ایمیل خود در وبسایت کنید<p>
        <p>لطفا پس از دریافت نام کاریری و رمزعبور هرچه سریع تر اقدام به تغییر آن کنید!!!</p>
        <p>نام کاربری: {user.Musername}</p>
        <p>رمزعبور: {user.Mpassword}</p>
        
    </div>
</body>
</html>
";
                // ✅ Step 2: Create the email
                MailMessage mail = new MailMessage
                {
                    From = new MailAddress(senderEmail),
                    Subject = "بازیابی اطلاعات ورود به سایت",
                    Body = emailBody,
                    IsBodyHtml = true
                };
                mail.To.Add(recipientEmail);

                // ✅ Step 3: Set up SMTP (Gmail SMTP server)
                SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587)
                {
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(senderEmail, senderPassword),
                    EnableSsl = true
                };

                // ✅ Step 4: Send the email
                smtp.Send(mail);
                Session["IsRecoveryTime"] = DateTime.Now;
                return Json(new { success = true, message = "ایمیل با موفقیت ارسال شد! در صورت عدم مشاهده پوشه اسپم را باز کنید." }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error: " + ex.Message });
            }
        }
    }
}