using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using CulturezVous.Service.Data.Elements.Dao;
using CulturezVous.Service.Data.Elements;
using CulturezVous.Service.Data.Response;

namespace CulturezVous.Webservice.Controllers
{
    /// <summary>
    /// Contrôleur principal du webservice pour la gestion des éléments
    /// </summary>
    public class ElementController : ServiceBaseController
    {
        private string cvDb;
        private bool useEncryption;

        public ElementController()
        {
            cvDb = ConfigurationManager.ConnectionStrings["CV_DB"].ToString();
            useEncryption = Convert.ToBoolean(ConfigurationManager.AppSettings["UseEncryption"].ToString());
        }

        /// <summary>
        /// Récupération d'éléments
        /// </summary>
        /// <param name="type"></param>
        /// <param name="startFrom"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public ActionResult Elements(string type = "Element", int startFrom = 0, int count = 20)
        {
            ServiceResponse r = new ServiceResponse();

            ElementDao dao = new ElementDao(cvDb);
            List<Element> elements = dao.GetAllElements(true);

            // Filtrer par type
            if (type == "Word")
            {
                elements = elements.Where(e => e is Word).ToList();
            }
            else if (type == "Contrepeterie")
            {
                elements = elements.Where(e => e is Contrepeterie).ToList();
            }
            else if (type != "Element")
            {
                elements = null;
                r.Code = (int)ServiceCodes.InvalidParameters;
                r.Message = type + " is not a valid element type. Use Element|Word|Contrepeterie";
            }

            if (elements != null)
            {
                if (elements.Count > 0)
                {
                    elements = elements.OrderByDescending(e => e.Date).Skip(startFrom).Take(count).ToList();

                    r.Code = (int)ServiceCodes.OK;
                    r.ResponseData = elements;
                }
                else if (dao.LastException != null)
                {
                    r.Code = (int)ServiceCodes.InternalError;
                    r.Message = "Elements not retrieved. " + dao.LastException;
                }
            }

            return PrepareResponse(r, useEncryption);
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

            return PrepareResponse(r, useEncryption);
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

            return PrepareResponse(r, useEncryption);
        }

    }
}
