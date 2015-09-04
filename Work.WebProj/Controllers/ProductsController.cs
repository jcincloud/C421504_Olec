using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DotWeb.Controller;
using ProcCore.Business.DB0;

namespace DotWeb.WebApp.Controllers
{
    public class ProductsController : WebUserController
    {
        // GET: Products
        public ActionResult Index(int? id)
        {
            ProductInfo info = new ProductInfo();
            using (db0 = getDB0())
            {
                if (id == null)
                {
                    var check = db0.Product.Where(x => x.i_Hide == false & x.i_Lang == System.Globalization.CultureInfo.CurrentCulture.Name).OrderByDescending(x => x.sort).FirstOrDefault();
                    if (check == null)
                    {
                        return Redirect("~/Products/NoData");
                    }
                    else
                    {
                        id = check.product_id;
                    }
                }


                Boolean Exist = db0.Product.Any(x => x.product_id == id && x.i_Hide == false & x.i_Lang == System.Globalization.CultureInfo.CurrentCulture.Name);
                if (!Exist)
                {
                    return Redirect("~/Products");
                }
                else
                {
                    info.Product = db0.Product.Find(id);
                    info.list = db0.Product.Where(x => x.i_Hide == false & x.i_Lang == System.Globalization.CultureInfo.CurrentCulture.Name).Select(x => new Plist() { p_id = x.product_id, p_name = x.product_name }).ToList();
                }
                ViewBag.ID = id;
            }
            return View("Content", info);
        }
        public ActionResult NoData()
        {
            return View();
        }
    }
}