using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using CulturezVous.Service.Data.Db;
using CulturezVous.Service.Data.Elements;
using CulturezVous.Service.Data.Elements.Dao;

namespace CulturezVous.Service.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/

        public ActionResult Index()
        {
            //var elements = new ElementDao(ConfigurationManager.ConnectionStrings["CV_DB"].ToString()).GetAllElements();

            return View();
        }

    }
}
