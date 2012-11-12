﻿using System.Web.Mvc;

namespace CulturezVous.Service.Areas.Service
{
    public class ServiceAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Service";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Service_default",
                "ws/{controller}/{action}.{format}",
                new { controller = "Element" }
            );
        }
    }
}
