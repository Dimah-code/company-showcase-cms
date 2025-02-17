
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using PortoWeb.Models;

namespace PortoWeb.Services
{
    internal class Finder
    {
        public static Response UpdateText(string id, string en, string fa)
        {
            var response = new Response();
            try
            {
                using (var db = new PortoDB1())
                {

                    var parts = id.Split('-');
                    if (parts.Length < 2 || !int.TryParse(parts[1], out int ID))
                    {
                        response.Success = false;
                        response.Message = "آیدی نامعتبر! لطفا دوباره تلاش کنید.";
                        return response;
                    }
                    var header = db.Table_Header.FirstOrDefault();
                    var homeBanner = db.Table_Banner.FirstOrDefault();
                    var siteInfo = db.Table_siteInfo.FirstOrDefault();
                    var homeAbout = db.Table_HomeAbout.FirstOrDefault();
                    var teamProj = db.Table_ProjTeam.FirstOrDefault();
                    var indexBottom = db.Table_BottomIndex.FirstOrDefault();
                    var about = db.Table_AboutTitle.FirstOrDefault(c => c.pkID == 1);
                    var projects = db.Table_AboutTitle.FirstOrDefault(c => c.pkID == 2);
                    var contact = db.Table_AboutTitle.FirstOrDefault(c => c.pkID == 3);
                    var team = db.Table_AboutTitle.FirstOrDefault(c => c.pkID == 4);
                    var blog = db.Table_AboutTitle.FirstOrDefault(c => c.pkID == 5);
                    var Slogans = db.Table_TopOfCounter.FirstOrDefault();
                    var UnderCounter = db.Table_UnderOFCounter.FirstOrDefault();
                    var counterText = db.Table_CounterTexts.FirstOrDefault();
                    switch (ID)
                    {
                        case 1:
                            header.homeEn = en;
                            header.homeFa = fa;
                            break;
                        case 2:
                            header.aboutFa = fa;
                            header.aboutEn = en;
                            break;
                        case 3:
                            header.projectsEn = en;
                            header.projectsFa = fa;
                            break;
                        case 4:
                            header.contactEn = en;
                            header.contactFa = fa;
                            break;
                        case 5:
                            header.teamEn = en;
                            header.teamFa = fa;
                            break;
                        case 6:
                            header.blogFa = fa;
                            header.blogEn = en;
                            break;
                        case 7:
                            homeBanner.siteNameEn = en;
                            siteInfo.siteName = fa;
                            break;
                        case 8:
                            homeBanner.bannerTitleEn = en;
                            homeBanner.bannerTitleFa = fa;
                            break;
                        case 9:
                            homeBanner.buttonCaptionEn = en;
                            homeBanner.buttonCaptionFa = fa;
                            break;
                        case 10:
                            homeAbout.MainTitleEn = en;
                            homeAbout.MainTitleFa = fa;
                            break;
                        case 11:
                            homeAbout.MainPgEn = en;
                            homeAbout.MainPgFa = fa;
                            break;
                        case 12:
                            homeAbout.Title01En = en;
                            homeAbout.Title01Fa = fa;
                            break;
                        case 13:
                            homeAbout.Title02En = en;
                            homeAbout.Title02Fa = fa;
                            break;
                        case 14:
                            homeAbout.Title03En = en;
                            homeAbout.Title03Fa = fa;
                            break;
                        case 15:
                            homeAbout.Title04En = en;
                            homeAbout.Title04Fa = fa;
                            break;
                        case 16:
                            teamProj.Title1Fa = fa;
                            teamProj.Title1En = en;
                            break;
                        case 17:
                            teamProj.ButtonCaption1En = en;
                            teamProj.ButtonCaption1Fa = fa;
                            break;
                        case 18:
                            teamProj.Title2Fa = fa;
                            teamProj.Title2En = en;
                            break;
                        case 19:
                            teamProj.ButtonCaption2En = en;
                            teamProj.ButtonCaption2Fa = fa;
                            break;
                        case 20:
                            siteInfo.address = fa;
                            indexBottom.AddressEn = en;
                            break;
                        case 21:
                            siteInfo.timeWork = fa;
                            indexBottom.TimeWorkEn = en;
                            break;
                        case 22:
                            about.AbtSloganEn = en;
                            about.AbtSloganFa = fa;
                            break;
                        case 23:
                            projects.AbtSloganFa = fa;
                            projects.AbtSloganEn = en;
                            break;
                        case 24:
                            contact.AbtSloganEn = en;
                            contact.AbtSloganFa = fa;
                            break;
                        case 25:
                            team.AbtSloganFa = fa;
                            team.AbtSloganEn = en;
                            break;
                        case 26:
                            blog.AbtSloganFa = fa;
                            blog.AbtSloganEn = en;
                            break;
                        case 27:
                            about.AbtTitleEn = en;
                            about.AbtTitleFa = fa;
                            break;
                        case 28:
                            about.AbtParagEn = en;
                            about.AbtParagFa = fa;
                            break;
                        case 29:
                            Slogans.t1fa = fa;
                            Slogans.t1en = en;
                            break;
                        case 30:
                            Slogans.p1fa = fa;
                            Slogans.p1en = en;
                            break;
                        case 31:
                            Slogans.t2fa = fa;
                            Slogans.t2en = en;
                            break;
                        case 32:
                            Slogans.p2fa = fa;
                            Slogans.p2en = en;
                            break;
                        case 33:
                            Slogans.t3fa = fa;
                            Slogans.t3en = en;
                            break;
                        case 34:
                            Slogans.p3fa = fa;
                            Slogans.p3en = en;
                            break;
                        case 35:
                            UnderCounter.aboutTitleFa = fa;
                            UnderCounter.aboutTitleEn = en;
                            break;
                        case 36:
                            UnderCounter.aboutParagEn = en;
                            UnderCounter.aboutParagFa = fa;
                            break;
                        case 37:
                            counterText.blogCounterFa = fa;
                            counterText.blogCounterEn = en;
                            break;
                        case 38:
                            counterText.projectCounterEn = en;
                            counterText.projectCounterFa = fa;
                            break;
                        case 39:
                            counterText.teamCounterEn = en;
                            counterText.teamCounterFa = fa;
                            break;
                        default:
                            response.Success = false;
                            response.Message = "پارامتر مورد نظر بافت نشد! لطفا دوباره تلاش کنید.";
                            return response;
                    }
                    db.SaveChanges();
                    response.Success = true;
                    response.Message = "ویرایش نوشتار با موفقیت انجام شد.";
                    return response;
                }
            }
            catch (Exception e)
            {
                response.Success = false;
                response.Message = e.Message;
                return response;
            }
}
        
    }
}

