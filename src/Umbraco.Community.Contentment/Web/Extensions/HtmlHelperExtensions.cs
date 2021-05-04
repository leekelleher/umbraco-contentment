/* Copyright © 2021 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

// TODO: [LK:2021-05-03] v9 Commenting out, as I'm (currently) unsure how to do the `HtmlHelper` bits.
// Feel that I need more understanding of .NET Core RazorPages.

//using System;
//using System.Collections.Generic;
//using Microsoft.AspNetCore.Html;
//using Microsoft.AspNetCore.Mvc.Razor;
//using Microsoft.AspNetCore.Mvc.Rendering;
//using Microsoft.AspNetCore.Mvc.ViewFeatures;
//using Umbraco.Cms.Core.Models.PublishedContent;
//using Umbraco.Extensions;

//namespace Umbraco.Web.Mvc
//{
//    public static class HtmlHelperExtensions
//    {
//        // Extension method derived from a StackOverflow answer.
//        // https://stackoverflow.com/a/44870370/12787
//        // Licensed under the permissions of the CC BY-SA 3.0.
//        // https://creativecommons.org/licenses/by-sa/3.0/
//        public static bool DoesPartialExist(this HtmlHelper helper, string partialViewName)
//        {
//            var controllerContext = helper.ViewContext.Controller.ControllerContext;
//            var result = ViewEngines.Engines.FindPartialView(controllerContext, partialViewName);

//            return result.View != null;
//        }

//        public static IHtmlContent Partial<TModel>(this HtmlHelper helper, string partialViewName, TModel model)
//        {
//            var viewData = new ViewDataDictionary(helper.ViewData) { Model = model };
//            return PartialExtensions.Partial(helper, partialViewName, model, viewData);
//        }

//        public static HelperResult RenderElements<TPublishedElement>(
//            this HtmlHelper helper,
//            IEnumerable<TPublishedElement> elements,
//            string viewPath = null,
//            string fallbackPartialViewName = null,
//            Func<TPublishedElement, bool> predicate = null,
//            ViewDataDictionary viewData = null)
//            where TPublishedElement : IPublishedElement
//        {
//            return new HelperResult(writer =>
//            {
//                if (elements != null)
//                {
//                    var elementIndex = 0;

//                    if (viewData == null)
//                    {
//                        viewData = new ViewDataDictionary();
//                    }

//                    foreach (var element in elements)
//                    {
//                        viewData[nameof(elementIndex)] = elementIndex++;

//                        if (predicate != null && predicate(element) == false)
//                        {
//                            continue;
//                        }

//                        var partialViewName = viewPath?.EnsureEndsWith("/") + element.ContentType.Alias;

//                        if (helper.DoesPartialExist(partialViewName) == false && string.IsNullOrWhiteSpace(fallbackPartialViewName) == false)
//                        {
//                            partialViewName = viewPath?.EnsureEndsWith("/") + fallbackPartialViewName;
//                        }

//                        if (helper.DoesPartialExist(partialViewName) == true)
//                        {
//#pragma warning disable MVC1000 // Use of IHtmlHelper.{0} should be avoided.
//                            writer.WriteLine(helper.Partial(partialViewName, element, viewData));
//#pragma warning restore MVC1000 // Use of IHtmlHelper.{0} should be avoided.
//                        }
//                        else
//                        {
//                            writer.WriteLine($"<!-- Missing partial view for element type: '{element.ContentType.Alias}' -->");
//                        }
//                    }
//                }
//            });
//        }
//    }
//}
