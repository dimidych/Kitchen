using System.Web.Mvc;
using System.Web.Routing;

namespace MvcStudy
{
   public class RouteConfig
   {
      public static void RegisterRoutes(RouteCollection routes)
      {
         routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
         
         routes.MapRoute(
             name: "Default",
             url: "{controller}/{action}/{id}",
             defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
         );

         routes.MapRoute(
                name: "User",
                url: "user/{action}/{id}",
                defaults: new { controller = "User", action = "Index", id = UrlParameter.Optional }
            );

         routes.MapRoute(
               name: "Meal",
               url: "meal/{action}/{id}",
               defaults: new { controller = "Meal", action = "Index", id = UrlParameter.Optional }
           );

         routes.MapRoute(
               name: "Menu",
               url: "menu/{action}/{id}",
               defaults: new { controller = "Menu", action = "Index", id = UrlParameter.Optional }
           );

         routes.MapRoute(
               name: "Duty",
               url: "duty/{action}/{id}",
               defaults: new { controller = "Duty", action = "Index", id = UrlParameter.Optional }
           );

         routes.MapRoute(
              name: "Settings",
              url: "settings/{action}/{id}",
              defaults: new { controller = "Settings", action = "Index", id = UrlParameter.Optional }
          );
      }
   }
}