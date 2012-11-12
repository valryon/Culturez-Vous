using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CulturezVous.Service.Areas.Service.Controllers
{
    public class ElementController : ServiceBaseController
    {
        [HttpPost]
        public ActionResult Index(string format)
        {
            return PrepareResponse(format, "truc");
        }

    }
}
