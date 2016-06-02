using DotWeb.CommSetup;
using ProcCore;
using System;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
//using System.Web.Optimization;
using System.Web.Routing;
using System.Web.WebPages;

namespace DotWeb.AppStart
{
    public class MvcApplication : System.Web.HttpApplication
    {
        string VarCookie = CommWebSetup.WebCookiesId;
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            GlobalConfiguration.Configuration.Formatters.XmlFormatter.SupportedMediaTypes.Clear();

            //DisplayModeProvider.Instance.Modes.Insert(0, new DefaultDisplayMode("tablet")
            //{
            //    ContextCondition = (x => (new WebInfo()).isTablet()) 
            //});

            DisplayModeProvider.Instance.Modes.Insert(0, new DefaultDisplayMode("en-US")
            {
                ContextCondition = (C => C.Request.Cookies[VarCookie + ".Lang"] != null && C.Request.Cookies[VarCookie + ".Lang"].Value.Contains("en-US"))
            });


        }
        protected void Application_BeginRequest(Object sender, EventArgs e)
        {
            HttpCookie WebLang = Request.Cookies[VarCookie + ".Lang"];
            var set_lang = string.Empty;
            string[] allow_lang = new string[] { "en-US", "zh-TW" };

            if (WebLang == null)
            {
                if (Request.UserLanguages != null && Request.UserLanguages.Length > 0)
                {
                    var q = Request.UserLanguages[0];
                    var n = System.Globalization.CultureInfo.CreateSpecificCulture(q);//轉換完整 語系-國家 編碼

                    if (allow_lang.Contains(n.Name))
                        set_lang = n.Name;
                    else
                        set_lang = allow_lang[0];
                }
                else
                {
                    var n = System.Threading.Thread.CurrentThread.CurrentCulture;
                    if (allow_lang.Contains(n.Name))
                        set_lang = n.Name;
                    else
                        set_lang = allow_lang[0];
                }
                WebLang = new HttpCookie(VarCookie + ".Lang", set_lang);
                Response.Cookies.Add(WebLang);
            }
            else
            {
                if (!allow_lang.Contains(WebLang.Value))
                {
                    set_lang = allow_lang[0];
                    WebLang.Value = set_lang;
                }
            }

            Log.SetupBasePath = Server.MapPath("~\\_Code\\Log\\");
            Log.Write(Request.UserHostAddress, "Language", WebLang.Value, Request.Browser.Browser, Request.Browser.Version);
            Log.WriteToFile();

            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(WebLang.Value);
            System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(System.Threading.Thread.CurrentThread.CurrentCulture.Name, false);
        }
    }
}
