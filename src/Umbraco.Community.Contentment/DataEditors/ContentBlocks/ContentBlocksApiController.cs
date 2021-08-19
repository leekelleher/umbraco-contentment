/* Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Web.BackOffice.Controllers;
using Umbraco.Cms.Web.Common.Attributes;
using Umbraco.Community.Contentment.Web.PublishedCache;
using Umbraco.Extensions;

namespace Umbraco.Community.Contentment.DataEditors
{
    [PluginController(Constants.Internals.PluginControllerName), IsBackOffice]
    public sealed class ContentBlocksApiController : UmbracoAuthorizedJsonController
    {
        private readonly ILogger<ContentBlocksApiController> _logger;
        private readonly IPublishedModelFactory _publishedModelFactory;
        private readonly IContentTypeService _contentTypeService;
        private readonly IUmbracoContextAccessor _umbracoContextAccessor;
        private readonly IModelMetadataProvider _modelMetadataProvider;
        private readonly IRazorViewEngine _viewEngine;
        private readonly ITempDataProvider _tempDataProvider;

        public ContentBlocksApiController(
            ILogger<ContentBlocksApiController> logger,
            IPublishedModelFactory publishedModelFactory,
            IContentTypeService contentTypeService,
            IUmbracoContextAccessor umbracoContextAccessor,
            IModelMetadataProvider modelMetadataProvider,
            IRazorViewEngine viewEngine,
            ITempDataProvider tempDataProvider)
        {
            _logger = logger;
            _publishedModelFactory = publishedModelFactory;
            _contentTypeService = contentTypeService;
            _umbracoContextAccessor = umbracoContextAccessor;
            _modelMetadataProvider = modelMetadataProvider;
            _viewEngine = viewEngine;
            _tempDataProvider = tempDataProvider;
        }

        [HttpPost]
        public ActionResult GetPreviewMarkup([FromBody] JObject item, int elementIndex, Guid elementKey, int contentId)
        {
            var preview = true;
            var umbracoContext = _umbracoContextAccessor.GetRequiredUmbracoContext();

            var content = umbracoContext.Content.GetById(true, contentId);
            if (content == null)
            {
                _logger.LogDebug($"Unable to retrieve content for ID '{contentId}', it is most likely a new unsaved page.");
            }

            var element = default(IPublishedElement);
            var block = item.ToObject<ContentBlock>();
            if (block != null && block.ElementType.Equals(Guid.Empty) == false)
            {
                if (ContentTypeCacheHelper.TryGetAlias(block.ElementType, out var alias, _contentTypeService) == true)
                {
                    var contentType = umbracoContext.PublishedSnapshot.Content.GetContentType(alias);
                    if (contentType != null && contentType.IsElement == true)
                    {
                        var properties = new List<IPublishedProperty>();

                        foreach (var thing in block.Value)
                        {
                            var propType = contentType.GetPropertyType(thing.Key);
                            if (propType != null)
                            {
                                properties.Add(new DetachedPublishedProperty(propType, null, thing.Value, preview));
                            }
                        }

                        element = _publishedModelFactory.CreateModel(new DetachedPublishedElement(block.Key, contentType, properties));
                    }
                }
            }

            var viewData = new ViewDataDictionary(_modelMetadataProvider, new ModelStateDictionary())
            {
                Model = element,
                [nameof(content)] = content,
                [nameof(element)] = element,
                [nameof(elementIndex)] = elementIndex,

            };

            if (ContentTypeCacheHelper.TryGetIcon(content.ContentType.Alias, out var contentIcon, _contentTypeService) == true)
            {
                viewData.Add(nameof(contentIcon), contentIcon);
            }

            if (ContentTypeCacheHelper.TryGetIcon(element.ContentType.Alias, out var elementIcon, _contentTypeService) == true)
            {
                viewData.Add(nameof(elementIcon), elementIcon);
            }

            string markup;

            try
            {
                markup = RenderPartialViewToString(element.ContentType.Alias, viewData);
            }
            catch (InvalidCastException icex)
            {
                // NOTE: This type of exception happens on a new (unsaved) page, when the context becomes the parent page,
                // and the preview view is strongly typed to the current page's model type.
                markup = "<p class=\"text-center mt4\">Unable to render the preview until the page has been saved.</p>";

                _logger.LogError(icex, "Error rendering preview view.");
            }
            catch (Exception ex)
            {
                markup = $"<pre class=\"error\"><code>{ex}</code></pre>";

                _logger.LogError(ex, "Error rendering preview view.");
            }

            return new ObjectResult(new { elementKey, markup });
        }

        // HACK: [v9] [LK:2021-05-13] Got it working. Future rewrite, make nicer.
        // The following code has been hacked and butchered from:
        // https://github.com/aspnet/Entropy/blob/master/samples/Mvc.RenderViewToString/RazorViewToStringRenderer.cs
        // https://gist.github.com/ahmad-moussawi/1643d703c11699a6a4046e57247b4d09
        private string RenderPartialViewToString(string viewName, ViewDataDictionary viewData)
        {
            IView view = default;

            // TODO: [v9] [LK:2021-05-13] Implement the custom partial-view paths.
            // e.g. "~/Views/Partials/Blocks/{0}.cshtml", "~/Views/Partials/Blocks/Default.cshtml", "~/App_Plugins/Contentment/render/ContentBlockPreview.cshtml"

            var getViewResult = _viewEngine.GetView(executingFilePath: null, viewPath: viewName, isMainPage: true);
            if (getViewResult.Success)
            {
                view = getViewResult.View;
            }

            var actionContext = new ActionContext(HttpContext, new RouteData(), new ActionDescriptor());
            var findViewResult = _viewEngine.FindView(actionContext, viewName, isMainPage: true);
            if (findViewResult.Success)
            {
                view = findViewResult.View;
            }

            if (view == default)
            {
                var messages = new List<string> { $"Unable to find view '{viewName}'. The following locations were searched:" };
                messages.AddRange(getViewResult.SearchedLocations);
                messages.AddRange(findViewResult.SearchedLocations);

                var errorMessage = string.Join(Environment.NewLine, messages);

                throw new InvalidOperationException(errorMessage);
            }

            using var output = new StringWriter();

            var tempDataDictionary = new TempDataDictionary(actionContext.HttpContext, _tempDataProvider);
            var viewContext = new ViewContext(actionContext, view, viewData, tempDataDictionary, output, new HtmlHelperOptions());

            view.RenderAsync(viewContext).GetAwaiter().GetResult();

            return output.ToString();
        }
    }
}
