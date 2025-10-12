using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PortoWeb.Models;
using PortoWeb.Helpers;
using System.IO;


namespace PortoWeb.Services
{

    public class BlogService
    {
        PortoDB1 context = new PortoDB1();

        public Response DeleteBlogWithImages(int id, string folderPath)
        {
            var response = new Response();
            var blog = context.Table_Blogs.Find(id);
            if (blog != null)
            {
                var mainImagePath = Path.Combine(folderPath, blog.bMainPic);
                if (File.Exists(mainImagePath))
                {
                    File.Delete(mainImagePath);
                }
                context.Table_Blogs.Remove(blog);
                var images = context.Table_BlogImages.Where(i => i.blogID == blog.bID).ToList();
                foreach (var image in images)
                {
                    var imagePath = Path.Combine(folderPath, image.bpUrl);
                    if (File.Exists(imagePath))
                    {
                        File.Delete(imagePath);
                    }
                }
                context.Table_BlogImages.RemoveRange(images);
                context.SaveChanges();
                response.Success = true;
                response.Message = "بلاگ با موفقیت حذف شد";
                return response;
            }
            response.Success = false;
            response.Message = "بلاگ مورد نظر یافت نشد! لطفا دوباره تلاش کنید.";
            return response;
        }
        public Response DeleteCategory(int id, string folderPath)
        {
            var response = new Response();
            var category = context.Table_BlogCategory.Find(id);
            if (category != null)
            {
                var blogs = context.Table_Blogs.Where(b => b.bCategory == category.bcID).ToList();
                if (blogs != null && blogs.Count > 0)
                {
                    foreach (var blog in blogs)
                    {
                        DeleteBlogWithImages(blog.bID, folderPath);
                    }
                }
                context.Table_BlogCategory.Remove(category);
                context.SaveChanges();
                response.Success = true;
                response.Message = "دسته بندی با موفقیت حذف شد";
                return response;
            }
            response.Success = false;
            response.Message = "دسته بندی مورد نظر یافت نشد";
            return response;
        }
        public Response DeleteMember(int id, string imageFolderPath, string blogFolderPath)
        {
            var response = new Response();
            var member = context.Table_Team.Find(id);
            if (member != null)
            {
                var blogs = context.Table_Blogs.Where(b => b.bWriter == member.tID).ToList();
                foreach (var blog in blogs)
                {
                    DeleteBlogWithImages(blog.bID, blogFolderPath);
                }
                var path = Path.Combine(imageFolderPath, member.tPhoto);
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
                context.Table_Team.Remove(member);
                context.SaveChanges();
                response.Success = true;
                response.Message = "عملیات حذف عضو با موفقیت انجام شد";
                return response;
            }
            else
            {
                response.Success = false;
                response.Message = "عضو مورد نظر یافت نشد!";
                return response;
            }
        }
        public Response DeleteRole(int id, string teamImagePath, string blogImagePath)
        {
            var response = new Response();
            var role = context.Table_Roles.Find(id);
            if (role != null)
            {
                var members = context.Table_Team.Where(m => m.fkRole == id).ToList();
                if (members != null && members.Any(p => p != null))
                {
                    foreach (var member in members)
                    {
                        DeleteMember(member.tID, teamImagePath, blogImagePath);
                    }
                    context.Table_Roles.Remove(role);
                    context.SaveChanges();
                    response.Success = true;
                    response.Message = "عملیات حذف عنوان شغلی با موفقیت انجام شد";
                    return response;
                }
                else
                {
                    response.Success = true;
                    response.Message = "عملیات حذف عنوان شغلی با موفقیت انجام شد";
                    return response;
                }
            }
            else
            {
                response.Success = false;
                response.Message = "عنوان شغلی یافت نشد";
                return response;
            }
        }
    }
}