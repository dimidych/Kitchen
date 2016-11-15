using System;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace MvcStudy.Utils
{
    public static class RazorHelper
    {
        public static MvcHtmlString MenuItem(this HtmlHelper razorHelper,
            string text, string action,
            string controller,
            object routeValues = null,
            object htmlAttributes = null)
        {
            MvcHtmlString result = null;

            try
            {
                var li = new TagBuilder("li");
                var routeData = razorHelper.ViewContext.RouteData;
                var currentAction = routeData.GetRequiredString("action");
                var currentController = routeData.GetRequiredString("controller");

                if (string.Equals(currentAction,
                    action,
                    StringComparison.OrdinalIgnoreCase) &&
                    string.Equals(currentController,
                        controller,
                        StringComparison.OrdinalIgnoreCase))
                {
                    li.AddCssClass("active");
                }

                if (routeValues != null)
                {
                    li.InnerHtml = (htmlAttributes != null)
                        ? razorHelper.ActionLink(text,
                            action,
                            controller,
                            routeValues,
                            htmlAttributes).ToHtmlString()
                        : razorHelper.ActionLink(text,
                            action,
                            controller,
                            routeValues).ToHtmlString();
                }
                else
                {
                    li.InnerHtml = (htmlAttributes != null)
                        ? razorHelper.ActionLink(text,
                            action,
                            controller,
                            null,
                            htmlAttributes).ToHtmlString()
                        : razorHelper.ActionLink(text,
                            action,
                            controller).ToHtmlString();
                }

                result = MvcHtmlString.Create(li.ToString());
            }
            catch
            {

            }

            return result;
        }
    }
}