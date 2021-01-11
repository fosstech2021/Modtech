using Microsoft.AspNetCore.Mvc.Rendering;

namespace BasePackageModule2.TagHelpers
{
    public  static class ActiveRouteTagHelper
    {
        public static string IsActive(this IHtmlHelper htmlHelper, string controller, string action)
        {
            var routeData = htmlHelper.ViewContext.RouteData;

            var routeAction = routeData.Values["action"].ToString();
            var routeController = routeData.Values["controller"].ToString();

            var returnActive = (controller == routeController && (action == routeAction || routeAction == "Details"));

            return returnActive ? "active" : "";
        }

        public static string IsMenuActive(this IHtmlHelper htmlHelper, string url)
        {
            var routeData = htmlHelper.ViewContext.RouteData;
            var routeController = routeData.Values["controller"].ToString();

            if (url == "/" && routeController == "Home")
            {
                return "active";
            }

            return url.Contains(routeController ?? string.Empty) ? "active" : "";
        }
    }
}
