/* Copyright © 2021 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.WebPages;
using Umbraco.Core;
using Umbraco.Core.Models.PublishedContent;

namespace Umbraco.Web.Mvc
{
    public static class HtmlHelperExtensions
    {
        // Extension method derived from a StackOverflow answer.
        // https://stackoverflow.com/a/44870370/12787
        // Licensed under the permissions of the CC BY-SA 3.0.
        // https://creativecommons.org/licenses/by-sa/3.0/
        public static bool DoesPartialExist(this HtmlHelper helper, string partialViewName)
        {
            var controllerContext = helper.ViewContext.Controller.ControllerContext;
            var result = ViewEngines.Engines.FindPartialView(controllerContext, partialViewName);

            return result.View != null;
        }

        public static MvcHtmlString Partial<TModel>(this HtmlHelper helper, string partialViewName, TModel model)
        {
            var viewData = new ViewDataDictionary(helper.ViewData) { Model = model };
            return PartialExtensions.Partial(helper, partialViewName, model, viewData);
        }

        public static HelperResult RenderElements<TPublishedElement>(
            this HtmlHelper helper,
            IEnumerable<TPublishedElement> elements,
            string viewPath = null,
            string fallbackPartialViewName = null,
            Func<TPublishedElement, bool> predicate = null,
            ViewDataDictionary viewData = null)
            where TPublishedElement : IPublishedElement
        {
            return new HelperResult(writer =>
            {
                if (elements != null)
                {
                    var elementIndex = 0;

                    if (viewData == null)
                    {
                        viewData = new ViewDataDictionary();
                    }

                    foreach (var element in elements)
                    {
                        viewData[nameof(elementIndex)] = elementIndex++;

                        if (predicate != null && predicate(element) == false)
                        {
                            continue;
                        }

                        var partialViewName = viewPath?.EnsureEndsWith("/") + element.ContentType.Alias;

                        if (helper.DoesPartialExist(partialViewName) == false && string.IsNullOrWhiteSpace(fallbackPartialViewName) == false)
                        {
                            partialViewName = viewPath?.EnsureEndsWith("/") + fallbackPartialViewName;
                        }

                        if (helper.DoesPartialExist(partialViewName) == true)
                        {
                            writer.WriteLine(helper.Partial(partialViewName, element, viewData));
                        }
                        else
                        {
                            writer.WriteLine($"<!-- Missing partial view for element type: '{element.ContentType.Alias}' -->");
                        }
                    }
                }
            });
        }
    }
}
