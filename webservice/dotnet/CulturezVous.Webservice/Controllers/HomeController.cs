using CulturezVous.Service.Areas.Service.Controllers;
using CulturezVous.Service.Data.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CulturezVous.Webservice.Controllers
{
    public class HomeController : ServiceBaseController
    {
        //
        // GET: /Home/

        public ActionResult Index(bool encrypt = false)
        {
            ServiceResponse r = new ServiceResponse();

            r.Code = (int)ServiceCodes.OK;
            r.Message = "Culturez-Vous webservice. Encryption : "+encrypt;

            return PrepareResponse(r, encrypt);
        }

    }
}
