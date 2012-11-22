using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml.Serialization;
using Newtonsoft.Json;
using System.IO;
using System.Text;
using System.Runtime.Serialization;
using CulturezVous.Service.Data.Response;
using CulturezVous.Service.Data.Serializer;
using CulturezVous.Service.Data.Crypto;

namespace CulturezVous.Service.Areas.Service.Controllers
{
    /// <summary>
    /// Contrôleur commun avec aide à la sérialisation et à l'encryption des données
    /// </summary>
    public abstract class ServiceBaseController : Controller
    {
        public ActionResult PrepareResponse(ServiceResponse response, bool encrypt = true)
        {
            string json = JsonHelper.Serialize(response);

            // Encryption
            if (encrypt)
            {
                return new ContentResult()
                {
                    Content = CVCrypto.Encrypt(json),
                    ContentType = "application/octet-stream"
                };
            }
            else
            {
                return new JsonResult()
                {
                    Data = json,
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
            }
        }

        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            base.OnActionExecuted(filterContext);
        }
    }
}
