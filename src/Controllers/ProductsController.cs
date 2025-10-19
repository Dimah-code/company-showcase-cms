using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PortoWeb.Models;
using PortoWeb.Helpers;
using System.Globalization;
using System.IO;

namespace PortoWeb.Controllers
{
    public class ProductsController : Controller
    {
        PortoDB1 context = new PortoDB1();
        public string GetLanguage()
        {
            var langCookie = Request.Cookies["lang"];
            return langCookie?.Value ?? "fa";
        }
        // GET: Products
        [HttpGet]
        [AuthorizeAdmin]
        public ActionResult ProductsList()
        {
            ViewBag.products = context.View_ProjectList.Where(p=> p.lang == "fa").OrderByDescending(p => p.pID).ToList();
            return View();
        }
        [HttpGet]
        [AuthorizeAdmin]
        public ActionResult ProductsListEn()
        {
            var lang = GetLanguage();
            ViewBag.products = context.View_ProjectList.Where(p => p.lang == "en").OrderByDescending(p => p.pID).ToList();
            return View();
        }
        [HttpGet]
        [AuthorizeAdmin]
        public ActionResult editProduct(int id)
        {
            var product = context.View_ProjectList.Where(p => p.pID == id).Single();
            if (product != null)
            {
                var category = context.Table_Category.ToList();
                var photo = context.Table_ProjectImage.Where(p => p.projectID == id).ToList();
                ViewBag.photo = photo;
                ViewBag.category = category;
                ViewBag.product = product;
                return View();
            }
            return RedirectToAction("Products", "ProductsList");
        }
        [AuthorizeAdmin]
        [HttpPost]
        public ActionResult EditProduct(int id, string ProductName, string ProductDescription, string ClientName, int CategoryId, HttpPostedFileBase MainPhoto, IEnumerable<HttpPostedFileBase> AdditionalPhotos , string lang)
        {
            if (string.IsNullOrEmpty(ProductName) || string.IsNullOrEmpty(ProductDescription) || string.IsNullOrEmpty(ClientName))
            {
                return Json(new { success = false, message = "Please fill in all the required fields." });
            }

            // Fetch the product from the database
            var product = context.Table_Projects.Find(id);
            if (product == null)
            {
                return Json(new { success = false, message = "Product not found." });
            }

            // Update text fields
            product.pName = ProductName;
            product.pDes = ProductDescription;
            product.pClient = ClientName;
            product.pCategory = CategoryId;
            product.lang = lang;
            // Handle Main Photo
            if (MainPhoto != null)
            {
                // Generate a unique file name with the original file extension
                string uniqueName = GenerateUniqueFileName(MainPhoto.FileName);

                // Combine the unique file name with the save directory path
                string mainPhotoPath = Path.Combine(Server.MapPath("~/assets/img/projects"), uniqueName);

                // Save the new main photo
                MainPhoto.SaveAs(mainPhotoPath);

                // Delete the old main photo if it exists
                if (!string.IsNullOrEmpty(product.pMainPic))
                {
                   
                    string oldMainPhotoPath = Path.Combine(Server.MapPath("~/assets/img/projects/"),product.pMainPic);

                    if (System.IO.File.Exists(oldMainPhotoPath))
                    {
                        System.IO.File.Delete(oldMainPhotoPath);
                    }
                }

                // Update the product's main photo file name in the database
                product.pMainPic = uniqueName; // Save only the unique file name
            }


            // Handle Additional Photos
            if (AdditionalPhotos != null && AdditionalPhotos.Any(photo => photo != null && photo.ContentLength > 0))
            {
                // Delete old additional photos
                var oldPhotos = context.Table_ProjectImage.Where(p => p.projectID == product.pID).ToList();

                foreach (var oldPhoto in oldPhotos)
                {
                    // Find image path for delete it
                    string oldPhotoPath = Path.Combine(Server.MapPath("~/assets/img/projects/"),oldPhoto.imageUrl);

                    if (System.IO.File.Exists(oldPhotoPath))
                    {
                        System.IO.File.Delete(oldPhotoPath);
                    }
                }
                context.Table_ProjectImage.RemoveRange(oldPhotos);
                // Save new additional photos
                foreach (var photo in AdditionalPhotos)
                {
                    // create image path for save it
                    string uniqueName = GenerateUniqueFileName(photo.FileName);
                    string photoPath = Path.Combine(Server.MapPath("~/assets/img/projects/"), uniqueName);
                    photo.SaveAs(photoPath);

                    context.Table_ProjectImage.Add(new Table_ProjectImage
                    {
                        projectID = product.pID,
                        imageUrl = uniqueName
                    });
                }
            }

            context.SaveChanges();

            return Json(new { success = true });
        }
        [HttpPost]
        [AuthorizeAdmin]
        public ActionResult DeleteProduct(int id)
        {
            var product = context.Table_Projects.Find(id);
            var relatedPhotos = context.Table_ProjectImage.Where(r => r.projectID == id).ToList();
            var mainPicPath = Path.Combine(Server.MapPath("~/assets/img/projects/"), product.pMainPic);
            if (System.IO.File.Exists(mainPicPath))
            {
                System.IO.File.Delete(mainPicPath);
            }
            foreach(var item in relatedPhotos)
            {
                var imagePath = Path.Combine(Server.MapPath("~/assets/img/projects/"), item.imageUrl);
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
            }
            if (product != null)
            {
                var images = context.Table_ProjectImage.Where(i => i.projectID == id).ToList();
                if (images != null)
                {
                    context.Table_ProjectImage.RemoveRange(images);
                }
                context.Table_Projects.Remove(product);
                context.SaveChanges();
                return Json(new { success = true, message = "نمونه کار با موفقیت حذف شد" });
            }
            return Json(new { success = false, message = "نمونه کار یافت نشد" });

        }
        [HttpGet]
        [AuthorizeAdmin]
        public ActionResult AddProduct()
        {
            ViewBag.Category = context.Table_Category.ToList();
            return View();
        }
        [HttpPost]
        [AuthorizeAdmin]
        public ActionResult AddNewProduct(string productName, string productDescription, string clientName, int categoryId, HttpPostedFileBase mainPhoto, IEnumerable<HttpPostedFileBase> additionalPhotos, string lang)
        {
            try
            {
                // Validate required fields
                if (string.IsNullOrEmpty(productName) || string.IsNullOrEmpty(productDescription) || mainPhoto == null)
                {
                    return Json(new { success = false, message = "لطفاً تمام اطلاعات ضروری را وارد کنید." });
                }

                // Validate the main photo size
                const int maxFileSize = 8 * 1024 * 1024; // 8MB
                if (mainPhoto.ContentLength > maxFileSize)
                {
                    return Json(new { success = false, message = "حجم عکس اصلی نمی‌تواند بیشتر از 8 مگابایت باشد." });
                }

                // Validate additional photos
                if (additionalPhotos != null)
                {
                    foreach (var photo in additionalPhotos)
                    {
                        if (photo.ContentLength > maxFileSize)
                        {
                            return Json(new { success = false, message = "حجم یکی از عکس‌های اضافی بیشتر از 8 مگابایت است." });
                        }
                    }
                }

                // Save Main Photo
                var mainPhotoName = GenerateUniqueFileName(mainPhoto.FileName);
                var mainPhotoPath = Path.Combine(Server.MapPath("~/assets/img/projects/"), mainPhotoName);
                mainPhoto.SaveAs(mainPhotoPath);


                var persianCalendar = new PersianCalendar();
                var now = DateTime.Now;
                var persianDate = $"{persianCalendar.GetYear(now)}/{persianCalendar.GetMonth(now):D2}/{persianCalendar.GetDayOfMonth(now):D2}";

                var newProduct = new Table_Projects
                {
                    pName = productName,
                    pDes = productDescription,
                    pClient = clientName,
                    pCategory = categoryId,
                    pMainPic = mainPhotoName,
                    lang = lang,
                    DoneDate = persianDate
                };

                context.Table_Projects.Add(newProduct);
                context.SaveChanges();

                // Save Additional Photos
                if (additionalPhotos != null)
                {
                    foreach (var photo in additionalPhotos)
                    {
                        var additionalPhotoName = GenerateUniqueFileName(photo.FileName);
                        var additionalPhotoPath = Path.Combine(Server.MapPath("~/assets/img/projects/"), additionalPhotoName);
                        photo.SaveAs(additionalPhotoPath);

                        var projectImage = new Table_ProjectImage
                        {
                            projectID = newProduct.pID, // Foreign key relation
                            imageUrl = additionalPhotoName
                        };

                        context.Table_ProjectImage.Add(projectImage);
                    }

                    context.SaveChanges();
                }

                return Json(new { success = true, message = "محصول با موفقیت اضافه شد." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "خطایی در ذخیره اطلاعات رخ داده است.", ex.Message });
            }
        }
        [AuthorizeAdmin]
        private string GenerateUniqueFileName(string originalFileName)
        {
            var extension = Path.GetExtension(originalFileName);
            var uniqueName = Guid.NewGuid().ToString() + extension;
            return uniqueName;
        }
        [HttpGet]
        [AuthorizeAdmin]
        public ActionResult Categories()
        {
            ViewBag.category = context.Table_Category.ToList();
            return View();
        }
        [HttpPost]
        [AuthorizeAdmin]
        public JsonResult EditCategory(int id, string cname , string cnameEn)
        {
            try
            {

                var existingCategory = context.Table_Category.Find(id);
                if (existingCategory == null)
                {
                    return Json(new { success = false, message = "دسته‌بندی یافت نشد." });
                }

                // Update the category
                existingCategory.Cname = cname;
                existingCategory.CnameEn = cnameEn;
                context.SaveChanges();

                return Json(new { success = true });

            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "خطا در ویرایش دسته‌بندی.", details = ex.Message });
            }
        }
        [HttpPost]
        [AuthorizeAdmin]
        public JsonResult DeleteCategory(int categoryId)
        {
            try
            {
                var category = context.Table_Category.Find(categoryId);
                if (category == null)
                {
                    return Json(new { success = false, message = "دسته‌بندی مورد نظر یافت نشد." });
                }
                var products = context.Table_Projects.Where(p => p.pCategory == categoryId).ToList();
                foreach(var product in products)
                {
                    var images = context.Table_ProjectImage.Where(i => i.projectID == product.pID).ToList();
                    foreach(var image in images)
                    {
                        var imagePath = Path.Combine(Server.MapPath("~/assets/img/projects/"), image.imageUrl);
                        if (System.IO.File.Exists(imagePath))
                        {
                            System.IO.File.Delete(imagePath);
                        }
                    }
                    context.Table_ProjectImage.RemoveRange(images);
                    var mainImagePath = Path.Combine(Server.MapPath("~/assets/img/projects"), product.pMainPic);
                    if (System.IO.File.Exists(mainImagePath))
                    {
                        System.IO.File.Delete(mainImagePath);
                    }
                }
                context.Table_Projects.RemoveRange(products);
                context.Table_Category.Remove(category);
                context.SaveChanges();
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "خطای داخلی سرور: " + ex.Message });
            }
        }
        [HttpPost]
        [AuthorizeAdmin]
        public ActionResult AddNewCategory(string catName , string catNameEn)
        {
            if(string.IsNullOrEmpty(catName) || string.IsNullOrWhiteSpace(catName))
            {
                return Json(new { success = false, message = "نام دسته بندی نمیتواند خالی باشد." });
            }
            if (string.IsNullOrEmpty(catNameEn) || string.IsNullOrWhiteSpace(catNameEn))
            {
                return Json(new { success = false, message = "نام دسته بندی به انگلیسی نمیتواند خالی باشد." });
            }
            try
            {
                Table_Category cat = new Table_Category
                {
                    Cname = catName,
                    CnameEn = catNameEn
                };
                context.Table_Category.Add(cat);
                context.SaveChanges();

                return Json(new { success = true, message = "عملیات ایجاد نام دسته‌بندی با موفقیت انجام شد" });
            } 
            catch(Exception e)
            {
                return Json(new { success = false, message = "خطا از سمت سرور:" + e.Message });
            }
        }
    }
}