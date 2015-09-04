using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DotWeb.Controller;
using ProcCore.Business.DB0;
namespace DotWeb.WebApp.Controllers
{
    public class NewsController : WebUserController
    {
        // GET: News
        public ActionResult Index()
        {
            NewsInfo info = new NewsInfo();
            using (var db0 = getDB0())
            {
                info.News = db0.News.Where(x => x.i_Hide == false & x.i_Lang == System.Globalization.CultureInfo.CurrentCulture.Name).OrderByDescending(x => x.news_date).ToList();

                foreach (var item in info.News)
                {
                    item.imgsrc = GetImg(item.news_id, "Photo1", "News", "News");//顯示列表圖
                }

                info.title = info.News.Select(x => x.news_title).Take(3).ToList();
            }
            return View("list", info);
        }

        public ActionResult Content(int id)
        {
            NewsContent content = new NewsContent();
            using (db0 = getDB0())
            {
                Boolean Exist = db0.News.Any(x => x.news_id == id && x.i_Hide == false & x.i_Lang == System.Globalization.CultureInfo.CurrentCulture.Name);
                if (!Exist)
                {
                    return Redirect("~/News");
                }
                else
                {
                    content.item = db0.News.Find(id);
                    content.title= db0.News.Where(x => x.i_Hide == false & x.i_Lang == System.Globalization.CultureInfo.CurrentCulture.Name).OrderByDescending(x => x.news_date).Take(3).Select(x=>x.news_title).ToList();
                    content.item.imgsrc = GetImg(id, "Photo1", "News", "News");//內頁圖片
                }
            }
            return View(content);
        }
    }
}