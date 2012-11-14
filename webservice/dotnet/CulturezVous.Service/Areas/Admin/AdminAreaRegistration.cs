using System.Web.Mvc;

namespace CulturezVous.Service.Areas.Admin
{
    public class AdminAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Admin";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
               "admin_elements",
               "admin/elements/{page}",
               new { action = "Index", controller = "Elements", page = 1 }
           );

            context.MapRoute(
               "admin_elements_add",
               "admin/elements/edit/{type}/{id}/",
               new { action = "Edit", controller = "Elements", id = UrlParameter.Optional }
            );

            context.MapRoute(
             "admin_elements_remove",
             "admin/elements/remove/{type}/{id}/",
             new { action = "Delete", controller = "Elements" }
          );

            context.MapRoute(
               "admin_elements_defaut",
               "admin/elements/{action}/{type}/{id}/",
               new { controller = "Elements" }
           );

            context.MapRoute(
                "Admin_default",
                "admin/{action}/{id}",
                new { action = "Index", controller= "Admin", id = UrlParameter.Optional }
            );
        }
    }
}
