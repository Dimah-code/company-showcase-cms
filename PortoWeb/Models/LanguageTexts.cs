using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PortoWeb.Models
{
    public static class LanguageTexts
    {
        public static Dictionary<string, Dictionary<string, string>> Texts
        = new Dictionary<string, Dictionary<string, string>>()
    {
        { "en", new Dictionary<string, string>
            {
            // for bottom of index
                { "Address", "Address" },
                { "Working Hours", "Working Hours" },
                { "Call Now" , "Call Now" },
                { "Company's Email" , "Company's Email" },
            // end
            //for footer
                { "Stay with us" , "Stay with us" },
                { "Contact us" , "Contact us" },
                { "Follow us" , "Follow us" },
                { "Ways of communication with us" , "Ways of communication with us" },
            //end
            // for message to admin
                { "Please Fill Name" , "Please Enter Your Name" },
                { "Name" , "Your Name..." },
                { "Please Fill Phone Number" , "Please Enter Your Phone Number" },
                { "Phone" , "Your Phone Number" },
                {"Please Fill The Message" , "Please Enter Your Message" },
                {"Message" , "Your Message" },
                {"Button Submit" , "Send" },
                {"On Loading" , "Loading Please Wait..." },
            // For projects and team category
                {"Show all" , "Show All" },
                {"description" , "Description" },
                {"More Info" , "More Information" },
                {"ProjectName" , "Name" },
                {"Client" , "Client" },
                {"ID" , "ID" },
                {"Date", "Date" },
                {"Category", "Category" },
                {"RelativeProjects" , "Related Projects" },
                {"Follow me" , "Follow me on social media" },
                {"Writer", "Writer" },
                {"Read More" , "Read More" },
                {"Keywords" , "Keywords" },
                {"Related Posts" , "Related Posts" }


            }
        },
        { "fa", new Dictionary<string, string>
            {
                // for bottom of index
                { "Address", "آدرس ما" },
                { "Working Hours", "ساعت کاری" },
                { "Call Now" , "هم اکنون تماس بگیرید" },
                { "Company's Email" , "ایمیل شرکت" },
                // footer
                { "Stay with us" , "با ما همراه باشید" },
                { "Contact us" , "ارتباط با ما" },
                { "Follow us" , "مارا دنبال کنید" },
                { "Ways of communication with us" , " راه های ارتباطی با ما" },
                // for message to admin
                { "Please Fill Name" , "لطفا نام خود را وارد کنید" },
                { "Name" , "نام شما..." },
                { "Please Fill Phone Number" , "لطفا شماره تلفن خود را وارد کنید" },
                { "Phone" , "شماره تلقن شما..." },
                { "Please Fill The Message" , "لطفا پیام خود را بنویسید" },
                { "Message" , "پیام شما..." },
                { "Button Submit" , "ارسال" },
                { "On Loading" , "درحال بارگذاری . لطفا صبر کنید..." },
            // for projects and team category
                {"Show all" , "نمایش همه" },
                {"description" , "توضیحات محصول" },
                {"More Info" , "اطلاعات بیشتر" },
                {"ProjectName" , "نام" },
                {"Client" , "مشتری" },
                {"ID" , "شناسه" },
                {"Date", "تاریخ" },
                {"Category", "دسته‌بندی" },
                {"RelativeProjects" , "پروژه های مرتبط" },
                {"Follow me" , "من را در شبکه‌های اجتماعی دنبال کنید" },
                {"Writer" , "نویسنده" },
                {"Read More" , "بیشتر بخوانید" },
                {"Keywords" , "کلمات کلیدی" },
                {"Related Posts" , "Related Posts" }
            }
        }
    };
    }

}