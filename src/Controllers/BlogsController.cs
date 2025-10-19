using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PortoWeb.Models;
using PortoWeb.Helpers;
using System.IO;
using System.Globalization;
using PortoWeb.Services;

namespace PortoWeb.Controllers
{

    public class BlogsController : Controller
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
        [HttpGet]
        [AuthorizeAdmin]
        public ActionResult BlogsList()
        {
            var blogs = context.View_BlogNcat.Where(b=> b.language == "fa").OrderByDescending(p => p.bID).ToList();
            ViewBag.blogs = blogs;
            return View();
        }
        [HttpGet]
        [AuthorizeAdmin]
        public ActionResult BlogsListEn()
        {
            var blogs = context.View_BlogNcat.Where(b=> b.language == "en").OrderByDescending(p => p.bID).ToList();
            ViewBag.blogs = blogs;
            return View();
        }
        [HttpGet]
        [AuthorizeAdmin]
        [ValidateInput(false)]
        public ActionResult EditBlog(int id)
        {
            var blog = context.View_BlogNcat.Where(b => b.bID == id).Single();
            if (blog != null)
            {
                var images = context.Table_BlogImages.Where(i => i.blogID == id).ToList();
                var categories = context.Table_BlogCategory.ToList();
                var writers = context.Table_Team.ToList();
                ViewBag.images = images;
                ViewBag.blog = blog;
                ViewBag.categories = categories;
                ViewBag.writers = writers;
                return View();
            }
            return RedirectToAction("BlogsList");
        }
        [HttpPost]
        public ActionResult EditBlogPost(Blog model)
        {
            if (model == null || string.IsNullOrWhiteSpace(model.bContent))
            {
                return Json(new { success = false, message = "لطفا محتوای بلاگ را  پر کنید" });
            }

            try
            {

                // Update the blog in the database
                var blog = context.Table_Blogs.Find(model.bID);
                if (blog == null)
                {
                    return Json(new { success = false, message = "بلاگ مورد نظر بافت نشد" });
                }

                blog.bName = model.bName;
                blog.bContent = model.bContent; // Save Quill content
                blog.bTitle = model.bTitle;
                blog.bKeywords = model.bKeywords;
                blog.language = model.language;

                var persianCalendar = new PersianCalendar();
                var now = DateTime.Now;
                var persianDate = $"{persianCalendar.GetYear(now)}/{persianCalendar.GetMonth(now):D2}/{persianCalendar.GetDayOfMonth(now):D2}";

                blog.bCreatedAt = persianDate; // Save Persian date

                blog.bCategory = model.bCategory;
                blog.bWriter = model.bWriter;
                if (model.mainPhoto != null)
                {
                    var oldMainPhoto = Path.Combine(Server.MapPath("~/Assets/img/blog"), blog.bMainPic);
                    if (System.IO.File.Exists(oldMainPhoto))
                    {
                        System.IO.File.Delete(oldMainPhoto);
                    }
                    var newPhotoName = GenerateUniqueFileName(model.mainPhoto.FileName);
                    var newPhotoPath = Path.Combine(Server.MapPath("~/Assets/img/blog"), newPhotoName);
                    model.mainPhoto.SaveAs(newPhotoPath);
                    blog.bMainPic = newPhotoName;
                }
                else
                {
                    blog.bMainPic = blog.bMainPic;
                }
                if (model.additionalPhotos.Count > 2)
                {
                    var oldImages = context.Table_BlogImages.Where(i => i.blogID == blog.bID).ToList();
                    foreach (var image in oldImages)
                    {
                        var imagePath = Path.Combine(Server.MapPath("~/Assets/img/blog"), image.bpUrl);
                        if (System.IO.File.Exists(imagePath))
                        {
                            System.IO.File.Delete(imagePath);
                        }
                    }
                    context.Table_BlogImages.RemoveRange(oldImages);
                    var newImages = model.additionalPhotos;
                    foreach (var image in newImages)
                    {
                        var uniqueName = GenerateUniqueFileName(image.FileName);
                        var imagePath = Path.Combine(Server.MapPath("~/Assets/img/blog"), uniqueName);
                        image.SaveAs(imagePath);
                        Table_BlogImages newBlogImages = new Table_BlogImages
                        {
                            blogID = blog.bID,
                            bpUrl = uniqueName
                        };
                        context.Table_BlogImages.Add(newBlogImages);
                    }
                }
                context.SaveChanges();
                return Json(new { success = true, message = "بلاگ با موفقیت ویرایش شد" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "خطا : " + ex.Message + ex.HelpLink });
            }
        }
        [AuthorizeAdmin]
        [HttpPost]
        public ActionResult DeleteBlog(int id)
        {
            try
            {
                string folderPath = Server.MapPath("~/Assets/img/blog");
                var response = service.DeleteBlogWithImages(id, folderPath);
                return Json(new { success = response.Success, message = response.Message });
            }
            catch (Exception e)
            {
                return Json(new { success = false, message = "خطا : " + e.Message + e.HelpLink });
            }
        }
        [HttpGet]
        [AuthorizeAdmin]
        public ActionResult AddNewBlog()
        {
            var writers = context.Table_Team.ToList();
            var categories = context.Table_BlogCategory.ToList();
            ViewBag.writers = writers;
            ViewBag.categories = categories;
            return View();
        }
        [AuthorizeAdmin]
        [HttpPost]
        public ActionResult AddNewBlogPost(Blog model)
        {
            if (model.additionalPhotos.Count != 3)
            {
                return Json(new { success = false, message = "حداقل باید 3 عکس برای جزئیات بلاگ وارد کنید" });
            }
            try
            {


                if (model != null)
                {
                    if (model.mainPhoto != null)
                    {
                        var mainPhotoName = GenerateUniqueFileName(model.mainPhoto.FileName);
                        var mainPhotoPath = Path.Combine(Server.MapPath("~/Assets/img/blog"), mainPhotoName);
                        model.mainPhoto.SaveAs(mainPhotoPath);



                        var persianCalendar = new PersianCalendar();
                        var now = DateTime.Now;
                        var persianDate = $"{persianCalendar.GetYear(now)}/{persianCalendar.GetMonth(now):D2}/{persianCalendar.GetDayOfMonth(now):D2}";

                        Table_Blogs newBlog = new Table_Blogs
                        {
                            bCategory = model.bCategory,
                            bContent = model.bContent,
                            bCreatedAt = persianDate,
                            bKeywords = model.bKeywords,
                            bMainPic = mainPhotoName,
                            bName = model.bName,
                            bTitle = model.bTitle,
                            language = model.language,
                            bWriter = model.bWriter
                        };
                        context.Table_Blogs.Add(newBlog);
                        context.SaveChanges();
                        if (model.additionalPhotos.Any() && model.additionalPhotos.Count > 1)
                        {
                            foreach (var image in model.additionalPhotos)
                            {
                                var imageUniqueName = GenerateUniqueFileName(image.FileName);
                                var imagePath = Path.Combine(Server.MapPath("~/Assets/img/blog"), imageUniqueName);
                                image.SaveAs(imagePath);
                                Table_BlogImages newImages = new Table_BlogImages
                                {
                                    blogID = newBlog.bID,
                                    bpUrl = imageUniqueName
                                };
                                context.Table_BlogImages.Add(newImages);
                            }
                            context.SaveChanges();
                            return Json(new { success = true, message = "بلاگ با موفقیت اضافه شد" });
                        }
                        else
                        {
                            return Json(new { success = false, message = "لطفا چند عکس آپلود کنید" });
                        }

                    }
                    else
                    {
                        return Json(new { success = false, message = "لطفا عکس اصلی را آپلود کنید" });
                    }

                }
                return Json(new { success = false, message = "لطفا تمامی فیلد هارا پر کنید" });
            }
            catch (Exception e)
            {
                return Json(new { success = false, message = "خطا :" + e.Message + e.HelpLink });
            }
        }
        [AuthorizeAdmin]
        [HttpGet]
        public ActionResult Categories()
        {
            var categories = context.Table_BlogCategory.ToList();
            ViewBag.categories = categories;
            return View();
        }
        [HttpPost]
        [AuthorizeAdmin]
        public ActionResult AddNewCategory(string catName, string catNameEn)
        {
            if (!string.IsNullOrEmpty(catName) || !string.IsNullOrWhiteSpace(catName))
            {
                try
                {
                    Table_BlogCategory newCat = new Table_BlogCategory
                    {
                        bcName = catName,
                        bcNameEn = catNameEn
                    };
                    context.Table_BlogCategory.Add(newCat);
                    context.SaveChanges();
                    return Json(new { success = true, message = "دسته بندی با موفقیت ایجاد شد" });
                }
                catch (Exception e)
                {
                    return Json(new { success = false, message = "خطا : " + e.Message + e.HelpLink });
                }

            }
            return Json(new { success = false, message = "لطفا نام دسته‌بندی را پر کنید" });
        }
        [AuthorizeAdmin]
        [HttpPost]
        public ActionResult EditCategory(int id, string cname ,string cnameEn)
        {
            var category = context.Table_BlogCategory.Find(id);
            if (category != null)
            {
                try
                {
                    if (!string.IsNullOrEmpty(cname) && !string.IsNullOrWhiteSpace(cname))
                    {
                        category.bcName = cname;
                        category.bcNameEn = cnameEn;
                        context.SaveChanges();
                        return Json(new { success = true, message = "دسته‌بندی با موفقیت ویرایش شد" });
                    }
                    return Json(new { success = false, message = "لطفا نام دسته‌بندی را وارد کنید" });
                }
                catch (Exception e)
                {
                    return Json(new { success = false, message = "خطا : " + e.Message + e.HelpLink });
                }
            }
            return Json(new { success = false, message = "دسته‌بندی مورد نظر یافت نشد" });
        }
        [AuthorizeAdmin]
        [HttpPost]
        public ActionResult DeleteCategory(int categoryId)
        {
            var category = context.Table_BlogCategory.Find(categoryId);
            if (category != null)
            {
                try
                {
                    string folderPath = Server.MapPath("~/Assets/img/blog");
                    var response = service.DeleteCategory(categoryId, folderPath);
                    return Json(new { success = response.Success, message = response.Message });
                }
                catch (Exception e)
                {
                    return Json(new { success = false, message = "خطا : " + e.Message + e.HelpLink });
                }
            }
            return Json(new { success = false, message = "دسته بندی مورد نظر یافت نشد" });
        }
    }
}
