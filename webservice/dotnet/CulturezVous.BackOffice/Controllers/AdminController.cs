using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CulturezVous.Service.Areas.Admin.Models;
using CulturezVous.Service.Data.Elements.Dao;
using System.Configuration;

namespace CulturezVous.Service.Areas.Admin.Controllers
{
    public class AdminController : Controller
    {
        //
        // GET: /Admin/Admin/

        public ActionResult Index()
        {
            return View();
        }

    }
}
