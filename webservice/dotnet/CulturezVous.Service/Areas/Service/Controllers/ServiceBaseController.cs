using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CulturezVous.Service.Areas.Service.Controllers
{
    public abstract class ServiceBaseController : Controller
    {
        public ActionResult PrepareResponse(string format, object model)
        {
            if (format == "xml")
            {
                return new ContentResult
                {
                    Content = "<truc></truc>",
                    ContentType = "application/xml"
                };
            }
            else if (format == "json")
            {
                return new JsonResult()
                {
                    Data = "{Pouet pouet}",
                };
            }
            else throw new ArgumentException("Unknow format " + format);
        }

    }
}
