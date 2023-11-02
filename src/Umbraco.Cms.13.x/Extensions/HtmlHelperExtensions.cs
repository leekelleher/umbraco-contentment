using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace Umbraco.Extensions
{
    public static class HtmlHelperExtensions
    {
        // Extension method derived from a StackOverflow answer.
        // https://stackoverflow.com/a/65323002/12787
        // Licensed under the permissions of the CC BY-SA 3.0.
        // https://creativecommons.org/licenses/by-sa/3.0/
        private static bool DoesPartialExist(this IHtmlHelper helper, string partialViewName)
        {
            var requestServices = helper.ViewContext.HttpContext.RequestServices;
            var viewEngine = requestServices.GetService<ICompositeViewEngine>();

#pragma warning disable CS8602 // Dereference of a possibly null reference.
            var viewEngineResult = viewEngine.FindView(helper.ViewContext, partialViewName, isMainPage: false);
#pragma warning restore CS8602 // Dereference of a possibly null reference.

            return viewEngineResult.Success;
        }

        public static HelperResult RenderCssModules(this IHtmlHelper helper, IEnumerable<IPublishedElement> modules, string prefix = "css")
        {
            return helper.RenderModules(modules, x => x is IModuleRenderCss, prefix.EnsureEndsWith("/"));
        }

        public static HelperResult RenderJsModules(this IHtmlHelper helper, IEnumerable<IPublishedElement> modules, RenderPosition position, string prefix = "js")
        {
            return helper.RenderModules(modules, x => x is IModuleRenderJs && x is IComponentCodeRenderPositionProperty js && js.RenderPosition?.ToString() == position.ToString(), prefix.EnsureEndsWith("/"));
        }

        public static HelperResult RenderMetaTagModules(this IHtmlHelper helper, IEnumerable<IPublishedElement> modules, string prefix = "meta")
        {
            return helper.RenderModules(modules, x => x is IModuleRenderMetaTag, prefix.EnsureEndsWith("/"));
        }

        private static HelperResult RenderModules(this IHtmlHelper helper, IEnumerable<IPublishedElement> modules, Func<IPublishedElement, bool> func, string prefix)
        {
            return new HelperResult(async (writer) =>
            {
                if (modules != null)
                {
                    int i = 0;
                    foreach (var module in modules)
                    {
                        var partialViewName = prefix + module.ContentType.Alias;
                        if (func(module) == true && helper.DoesPartialExist(partialViewName) == true)
                        {
                           await writer.WriteLineAsync(helper.PartialAsync(partialViewName, module, viewData: new ViewDataDictionary(helper.ViewData) { { "position", ++i } }).Result.ToHtmlString());
                        }
                    }
                }
            });
        }
    }
}
