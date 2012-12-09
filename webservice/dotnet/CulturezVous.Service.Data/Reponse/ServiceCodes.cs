using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CulturezVous.Service.Data.Response
{
    /// <summary>
    /// Codes d'erreur du service
    /// </summary>
    public enum ServiceCodes
    {
        OK = 0,
        UnknowId = 100,
        InvalidParameters = 101,
        InternalError = 200
    }
}