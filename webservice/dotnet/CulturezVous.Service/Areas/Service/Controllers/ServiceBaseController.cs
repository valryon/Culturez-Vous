using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CulturezVous.Service.Areas.Service.Models;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace CulturezVous.Service.Areas.Service.Controllers
{
    public abstract class ServiceBaseController : Controller
    {
        public ActionResult PrepareResponse(ServiceResponse response, string format)
        {
            if (format == "xml")
            {
                XmlSerializer serializer = new XmlSerializer(typeof(ServiceResponse));
                //string xml = serializer.S

                return new ContentResult
                {
                    Content = "<truc></truc>",
                    ContentType = "application/xml"
                };
            }
            else if (format == "json")
            {
                string json = JsonConvert.SerializeObject(response,
                     Formatting.None,
                     new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }
                 );

                return new JsonResult()
                {
                    Data = json,
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
            }
            else throw new ArgumentException("Unknow format " + format);
        }

    }
}
