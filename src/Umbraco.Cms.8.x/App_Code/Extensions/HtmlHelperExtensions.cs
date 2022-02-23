using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.WebPages;
using Umbraco.Core;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Web.PublishedModels;

namespace Umbraco.Web.Mvc
{
    public static class HtmlHelperExtensions
    {
        public static HelperResult RenderCssModules(this HtmlHelper helper, IEnumerable<IPublishedElement> modules, string prefix = "css")
        {
            return helper.RenderModules(modules, x => x is IModuleRenderCss, prefix.EnsureEndsWith("/"));
        }

        public static HelperResult RenderJsModules(this HtmlHelper helper, IEnumerable<IPublishedElement> modules, RenderPosition position, string prefix = "js")
        {
            return helper.RenderModules(modules, x => x is IModuleRenderJs && x is IComponentCodeRenderPositionProperty js && js.RenderPosition == position, prefix.EnsureEndsWith("/"));
        }

        public static HelperResult RenderMetaTagModules(this HtmlHelper helper, IEnumerable<IPublishedElement> modules, string prefix = "meta")
        {
            return helper.RenderModules(modules, x => x is IModuleRenderMetaTag, prefix.EnsureEndsWith("/"));
        }

        private static HelperResult RenderModules(this HtmlHelper helper, IEnumerable<IPublishedElement> modules, Func<IPublishedElement, bool> func, string prefix)
        {
            return new HelperResult(writer =>
            {
                if (modules != null)
                {
                    int i = 0;
                    foreach (var module in modules)
                    {
                        var partialViewName = prefix + module.ContentType.Alias;
                        if (func(module) == true && helper.DoesPartialExist(partialViewName) == true)
                        {
                            writer.WriteLine(helper.Partial(partialViewName, module, new ViewDataDictionary(helper.ViewData) { { "position", ++i } }));
                        }
                    }
                }
            });
        }
    }
}
