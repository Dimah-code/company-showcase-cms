using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PortoWeb.Models;
using PortoWeb.Helpers;
using System.IO;
using System.Diagnostics;
using PortoWeb.Services;
namespace PortoWeb.Controllers
{
    public class TeamController : Controller
    {
        PortoDB1 context = new PortoDB1();
        BlogService service = new BlogService();

        [AuthorizeAdmin]
        private string GenerateUniqueFileName(string originalFileName)
        {
            var extension = Path.GetExtension(originalFileName);
            var uniqueName = Guid.NewGuid().ToString() + extension;
            return uniqueName;
        }
        // GET: Team
        [HttpGet]
        [AuthorizeAdmin]
        public ActionResult TeamList()
        {
            ViewBag.team = context.View_TeamRole.Where(p=> p.lang == "fa").OrderByDescending(t => t.tID).ToList();
            return View();
        }
        [HttpGet]
        [AuthorizeAdmin]
        public ActionResult TeamListEn()
        {
            ViewBag.team = context.View_TeamRole.Where(p => p.lang == "en").OrderByDescending(t => t.tID).ToList();
            return View();
        }
        [HttpGet]
        [AuthorizeAdmin]
        public ActionResult AddMember()
        {
            ViewBag.role = context.Table_Roles.ToList();
            return View();
        }
        [HttpPost]
        [AuthorizeAdmin]
        public JsonResult AddMemberPost
            (string memberName,
            HttpPostedFileBase memberPhoto,
            string memberTitle,
            string memberDes,
            int memberRole,
            string memberInstagram,
            string memberLinkedin,
            string lang,
            string skill1, string skill2, string skill3, string skill4)
        {
            // variables 
            const int maxFileSize = 4 * 1024 * 1024; // 4 mb

            if (memberPhoto != null)
            {
                if (memberPhoto.ContentLength > maxFileSize)
                {
                    return Json(new { success = false, message = "عکس انتخاب شده بیشتر از 4 مگابایت است" });
                }
                var uniqeName = GenerateUniqueFileName(memberPhoto.FileName);
                var path = Path.Combine(Server.MapPath("~/assets/img/team/"), uniqeName);
                memberPhoto.SaveAs(path);


                Table_Team team = new Table_Team
                {
                    tNameFamily = memberName,
                    tDes = memberDes,
                    tTitle = memberTitle,
                    tSkill1 = skill1,
                    tSkill2 = skill2,
                    tSkill3 = skill3,
                    tSkill4 = skill4,
                    fkRole = memberRole,
                    tInstagram = memberInstagram,
                    tLinkedin = memberLinkedin,
                    lang = lang,
                    tPhoto = uniqeName
                };
                context.Table_Team.Add(team);
                context.SaveChanges();
                return Json(new { success = true, message = "عضو جدید با موفقیت اضافه شد" });
            }
            return Json(new { success = false, message = "لطفا یک عکس آپلود کنید" });
        }
        [HttpPost]
        [AuthorizeAdmin]
        public ActionResult DeleteMember(int id)
        {
            try
            {
                string blogFolderPath = Server.MapPath("~/assets/img/blog");
                string imageFolderPath = Server.MapPath("~/assets/img/team");
                var res = service.DeleteMember(id, imageFolderPath, blogFolderPath);
                return Json(new { success = res.Success, message = res.Message });
            }
            catch (Exception e)
            {
                return Json(new { success = false, message = e.Message });
            }
        }
        [HttpGet]
        [AuthorizeAdmin]
        public ActionResult EditMember(int id)
        {
            var member = context.View_TeamRole.SingleOrDefault(m => m.tID == id);
            if (member != null)
            {
                var roles = context.Table_Roles.Where(r => r != null).ToList();
                ViewBag.roles = roles;
                ViewBag.member = member;
                return View();
            }
            return RedirectToAction("TeamList");
        }
        [HttpPost]
        [AuthorizeAdmin]
        public ActionResult EditMemberPost(
            int memberId,
            string memberName,
            HttpPostedFileBase memberPhoto,
            string memberTitle,
            string memberDes,
            int memberRole,
            string memberInstagram,
            string memberLinkedin,
            string lang,
            string skill1, string skill2, string skill3, string skill4
            )
        {
            try
            {
                var member = context.Table_Team.Where(m => m.tID == memberId).Single();
                if (member != null)
                {

                    member.tTitle = memberTitle;
                    member.tNameFamily = memberName;
                    member.tDes = memberDes;
                    member.fkRole = memberRole;
                    member.tInstagram = memberInstagram;
                    member.tLinkedin = memberLinkedin;
                    member.tSkill1 = skill1;
                    member.lang = lang;
                    member.tSkill2 = skill2;
                    member.tSkill3 = skill3;
                    member.tSkill4 = skill4;

                    // Photo saving
                    if (memberPhoto != null)
                    {
                        // delete that old photo from files
                        var oldPhotoPath = Path.Combine(Server.MapPath("~/assets/img/team"), member.tPhoto);
                        if (System.IO.File.Exists(oldPhotoPath))
                        {
                            System.IO.File.Delete(oldPhotoPath);
                        }
                        // max filesize
                        int MaxFileSize = 4 * 1024 * 1024; // 4MB
                        // check photo is valid or not
                        if (memberPhoto.ContentLength > MaxFileSize)
                        {
                            return Json(new { success = true, message = "فایل انتخاب شده باید کمتر از 4 مگابایت حجم داشته باشد" });
                        }
                        // uploading new file
                        var uniqueName = GenerateUniqueFileName(memberPhoto.FileName);
                        var path = Path.Combine(Server.MapPath("~/assets/img/team"), uniqueName);
                        // save new photo in files
                        memberPhoto.SaveAs(path);
                        // set in database
                        member.tPhoto = uniqueName;
                    }
                    else
                    {
                        // if photo is null previous set in database
                        member.tPhoto = member.tPhoto;
                    }
                    //save all data
                    context.SaveChanges();
                    return Json(new { success = true, message = "ویرایش اطلاعات با موفقیت انجام شد" });
                }
                return Json(new { success = false, message = "عضو مورد نظر یافت نشد" });
            }
            catch (Exception e)
            {
                return Json(new { success = false, message = e.Message });
            }
        }
        // Roles Managment
        [AuthorizeAdmin]
        [HttpGet]
        public ActionResult Roles()
        {
            // Map Table_Roles to RolesModel
            var roles = context.Table_Roles
                .Select(r => new PortoWeb.Models.RolesModel
                {
                    roleID = r.rID,
                    roleName = r.pkRole,
                    roleDes = r.roleDes
                })
                .ToList();

            return View(roles);
        }
        [HttpPost]
        [AuthorizeAdmin]
        public ActionResult AddNewRole(string Role , string RoleEn)
        {
            try
            {
                Table_Roles newRole = new Table_Roles
                {
                    pkRole = Role,
                    roleDes = RoleEn
                };
                context.Table_Roles.Add(newRole);
                context.SaveChanges();
                return Json(new { success = true, message = "عنوان شغلی با موفقیت اضاقه شد" });
            }
            catch(Exception e)
            {
                return Json(new { success = false, message = e.Message });
            }
        }
        [HttpPost]
        [AuthorizeAdmin]
        public ActionResult EditRole(int id , string roleName, string roleNameEn)
        {
            try
            {
                var role = context.Table_Roles.Where(r => r.rID == id).Single();
                if (role != null)
                {
                    role.pkRole = roleName;
                    role.roleDes = roleNameEn;
                    context.SaveChanges();
                    return Json(new { success = true, message = "ویرایش عنوان شغلی با موفقیت انجام شد" });
                }
                return Json(new { success = false, message = "عنوان شغلی موردنظر یافت نشد" });
            }
            catch(Exception e)
            {
                return Json(new { success = false, message = e.Message });
            }
        }
        [HttpPost]
        [AuthorizeAdmin]
        public ActionResult DeleteRole(int id)
        {
            try
            {
                var response = new Response();
                string teamImagePath = Server.MapPath("~/assets/img/team");
                string blogImagesPath = Server.MapPath("~/assets/img/blog");
                var res = service.DeleteRole(id, teamImagePath, blogImagesPath);
                return Json(new { success = res.Success, message = res.Message });
            }
            catch(Exception e)
            {
                return Json(new { success = false, message = e.Message });
            }
        }
    }
}