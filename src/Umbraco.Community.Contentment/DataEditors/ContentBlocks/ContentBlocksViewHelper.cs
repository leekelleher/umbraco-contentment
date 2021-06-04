/* Copyright © 2020 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Umbraco.Community.Contentment.DataEditors
{
    internal static class ContentBlocksViewHelper
    {
        [Core.Composing.HideFromTypeFinder]
        private class ContentBlocksFakeController : ControllerBase { protected override void ExecuteCore() { } }

        private static readonly RazorViewEngine _viewEngine = new RazorViewEngine
        {
            PartialViewLocationFormats = new[]
            {
                "~/Views/Partials/Blocks/{0}.cshtml",
                "~/Views/Partials/Blocks/Default.cshtml",
                Constants.Internals.PackagePathRoot + "render/ContentBlockPreview.cshtml"
            }
        };

        internal static string RenderPartial(string partialName, ViewDataDictionary viewData)
        {
            using (var sw = new StringWriter())
            {
                var httpContext = new HttpContextWrapper(HttpContext.Current);

                var routeData = new RouteData { Values = { { "controller", nameof(ContentBlocksFakeController) } } };

                var controllerContext = new ControllerContext(new RequestContext(httpContext, routeData), new ContentBlocksFakeController());

                var viewResult = _viewEngine.FindPartialView(controllerContext, partialName, false);

                if (viewResult.View == null)
                {
                    return null;
                }

                viewResult.View.Render(new ViewContext(controllerContext, viewResult.View, viewData, new TempDataDictionary(), sw), sw);

                return sw.ToString();
            }
        }
    }
}
