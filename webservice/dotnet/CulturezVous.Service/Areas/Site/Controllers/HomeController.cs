using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CulturezVous.Service.Models;
using System.Configuration;
using CulturezVous.Service.Data.Elements.Dao;
using CulturezVous.Service.Data.Elements;

namespace CulturezVous.Service.Areas.Site.Controllers
{
    public class HomeController : Controller
    {
        private static string cvDb = ConfigurationManager.ConnectionStrings["CV_DB"].ToString();

        //
        // GET: /Site/SiteHome/

        public ActionResult Index()
        {
            return View();
        }


        public ActionResult DisplayElement(int id, string title = null)
        {
            // Récupération de l'élément
            ElementDao dao = new ElementDao(cvDb);
            Element e = dao.GetElementById(id);

            if (e == null)
            {
                return RedirectToAction("Index");
            }

            ElementViewModel viewModel = new ElementViewModel(e);
            return View(viewModel);
        }
    }
}
