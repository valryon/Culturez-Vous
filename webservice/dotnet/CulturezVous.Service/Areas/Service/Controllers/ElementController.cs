using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using CulturezVous.Service.Data.Elements.Dao;
using CulturezVous.Service.Data.Elements;
using CulturezVous.Service.Data.Response;

namespace CulturezVous.Service.Areas.Service.Controllers
{
    /// <summary>
    /// Contrôleur principal du webservice pour la gestion des éléments
    /// </summary>
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
        public ActionResult LastElements(int page)
        {
            if (page < 0) page = 1;
            ServiceResponse r = new ServiceResponse();

            ElementDao dao = new ElementDao(cvDb);
            List<Element> elements = dao.GetAllElements(true);

            if (elements != null & elements.Count > 0)
            {
                elements = elements.OrderByDescending(e => e.Date).Skip((page - 1) * pageSize).Take(pageSize).ToList();

                r.Code = (int)ServiceCodes.OK;
                r.ResponseData = elements;
            }
            else
            {
                r.Code = (int)ServiceCodes.InternalError;
                r.Message = "Elements not retrieved. " + dao.LastException;
            }
            return PrepareResponse(r);
        }

        /// <summary>
        /// Récupération des éléments avec pagination
        /// </summary>
        /// <param name="format"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public ActionResult BestOf()
        {
            ElementDao dao = new ElementDao(cvDb);
            List<Element> elements = dao.GetAllElements(true);

            elements = elements.OrderByDescending(e => e.FavoriteCount).Take(3 * pageSize).ToList();

            ServiceResponse r = new ServiceResponse();
            r.Code = (int)ServiceCodes.OK;
            r.ResponseData = elements;

            return PrepareResponse(r);
        }


        public ActionResult Detail(int id)
        {
            ServiceResponse r = new ServiceResponse();

            // Récupérer l'élément
            ElementDao dao = new ElementDao(cvDb);

            Element element = dao.GetElementById(id);

            if (element == null)
            {
                r.Code = (int)ServiceCodes.UnknowId;
                r.Message = "Unknow element: " + id;
            }
            else
            {
                r.Code = (int)ServiceCodes.OK;
                r.ResponseData = element;
            }

            return PrepareResponse(r);
        }

        private static object updateLock = new object();

        /// <summary>
        /// Marquer un élément comme favori
        /// </summary>
        /// <returns></returns>
#if DEBUG
        [AcceptVerbs("GET", "POST")]
#else
        [HttpPost]
#endif
        public ActionResult Favorite(int id)
        {
            ServiceResponse r = new ServiceResponse();

            // One ne traite qu'une mise à jour à la fois
            lock (updateLock)
            {
                // Récupérer l'élément
                ElementDao dao = new ElementDao(cvDb);

                Element element = dao.GetElementById(id);
                element.FavoriteCount++;

                if (element == null)
                {
                    r.Code = (int)ServiceCodes.UnknowId;
                    r.Message = "Unknow element: " + id;
                }
                else
                {
                    bool exec = dao.UpdateFavoriteView(element);

                    if (exec)
                    {
                        r.Code = (int)ServiceCodes.OK;
                    }
                    else
                    {
                        r.Code = (int)ServiceCodes.InternalError;
                        r.Message = "Updating element failed. " + dao.LastException;
                    }
                }
            }

            return PrepareResponse(r);
        }

    }
}
