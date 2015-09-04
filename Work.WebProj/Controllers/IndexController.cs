using System.Linq;
using System.Web.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;
using System.IO;
using System;
using System.Collections.Generic;
using System.Net.Mail;
using ProcCore.HandleResult;
using DotWeb.CommSetup;
using DotWeb.Controller;
using ProcCore.Business.DB0;

namespace DotWeb.Controllers
{
    public class IndexController : WebUserController
    {
        public ActionResult Index()
        {
            IList<News> news = new List<News>();
            using (var db0 = getDB0())
            {
                news = db0.News.Where(x => x.i_Hide == false & x.i_Lang == System.Globalization.CultureInfo.CurrentCulture.Name).OrderByDescending(x => x.news_date).Take(10).ToList();

                foreach (var item in news)
                {
                    item.imgsrc = GetImg(item.news_id, "Photo1", "News", "News");//顯示列表圖
                }
            }
            return View(news);
        }

        public RedirectResult Login()
        {
            return Redirect("~/Base/Login");
        }
    }
}
