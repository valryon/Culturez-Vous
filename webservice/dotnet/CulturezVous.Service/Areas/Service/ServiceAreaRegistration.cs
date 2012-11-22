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
                "Service_element_last",
                "ws/elements/{page}",
                new { controller = "Element", action = "LastElements" }
            );

            context.MapRoute(
                "Service_element",
                "ws/element/{id}",
                new { controller = "Element", action = "Detail" }
            );

            context.MapRoute(
                "Service_element_bestof",
                "ws/florilege",
                new { controller = "Element", action = "BestOf" }
            );

            context.MapRoute(
                "Service_element_favorite",
                "ws/favorite/{id}",
                new { controller = "Element", action = "Favorite" }
            );

            context.MapRoute(
                "Service_default",
                "ws/{controller}/{action}",
                new { controller = "Element" }
            );
        }
    }
}
