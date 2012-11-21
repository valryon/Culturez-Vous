using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using CulturezVous.Service.Areas.Service.Models;
using CulturezVous.Service.Data.Elements.Dao;
using CulturezVous.Service.Data.Elements;

namespace CulturezVous.Service.Areas.Service.Controllers
{
    public class ElementController : ServiceBaseController
    {
        private static int pageSize = 7;

        private static string cvDb = ConfigurationManager.ConnectionStrings["CV_DB"].ToString();

        /// <summary>
        /// Récupération des éléments avec pagination
        /// </summary>
        /// <param name="format"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public ActionResult LastElements(string format, int page)
        {
            if(page < 0) page = 1;

            ElementDao dao = new ElementDao(cvDb);
            List<Element> elements = dao.GetAllElements(true);

            elements = elements.Skip((page - 1) * pageSize).Take(pageSize).OrderByDescending(e => e.Date).ToList();

            ServiceResponse r = new ServiceResponse();
            r.Code = (int)ServiceCodes.OK;
            r.ResponseData = elements;

            return PrepareResponse(r, format);
        }

    }
}
