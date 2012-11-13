using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using CulturezVous.Service.Data.Db;
using CulturezVous.Service.Data.Elements;

namespace CulturezVous.Service.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/

        public ActionResult Index()
        {
            string cs = ConfigurationManager.ConnectionStrings["CV_DB"].ToString();

            ElementDao dao = new ElementDao(cs);
            dao.TestDb();
            return View();
        }

    }
}
