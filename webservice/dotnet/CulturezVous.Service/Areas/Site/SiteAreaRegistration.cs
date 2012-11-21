using System.Web.Mvc;

namespace CulturezVous.Service.Areas.Site
{
    public class SiteAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Site";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Site_element",
                "display/{id}/{title}",
                new { action = "DisplayElement", controller = "Home", title = UrlParameter.Optional}
            );

            context.MapRoute(
                "Site_default",
                "{controller}/{action}/{id}",
                new { action = "Index", controller ="Home", id = UrlParameter.Optional }
            );
        }
    }
}
