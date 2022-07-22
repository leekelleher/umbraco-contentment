/* Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

#if NET472
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Newtonsoft.Json.Linq;
using Umbraco.Community.Contentment.Web.PublishedCache;
using Umbraco.Core.Logging;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Web.Editors;
using Umbraco.Web.Mvc;
using Umbraco.Web.WebApi;
#else
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
#endif

namespace Umbraco.Community.Contentment.DataEditors
{
#if NET472
    [PluginController(Constants.Internals.PluginControllerName), IsBackOffice]
    public sealed class ContentBlocksApiController : UmbracoAuthorizedJsonController
    {
        private readonly ILogger _logger;
        private readonly IPublishedModelFactory _publishedModelFactory;

        public ContentBlocksApiController(ILogger logger, IPublishedModelFactory publishedModelFactory)
        {
            _logger = logger;
            _publishedModelFactory = publishedModelFactory;
        }

        [HttpPost]
        public HttpResponseMessage GetPreviewMarkup([FromBody] JObject item, int elementIndex, Guid elementKey, int contentId)
        {
            var preview = true;

            var content = UmbracoContext.Content.GetById(true, contentId);
            if (content == null)
            {
                _logger.Debug<ContentBlocksApiController>($"Unable to retrieve content for ID '{contentId}', it is most likely a new unsaved page.");
            }

            var element = default(IPublishedElement);
            var block = item.ToObject<ContentBlock>();
            if (block != null && block.ElementType.Equals(Guid.Empty) == false)
            {
                if (ContentTypeCacheHelper.TryGetAlias(block.ElementType, out var alias, Services.ContentTypeService) == true)
                {
                    var contentType = UmbracoContext.PublishedSnapshot.Content.GetContentType(alias);
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

            var viewData = new System.Web.Mvc.ViewDataDictionary(element)
            {
                { nameof(content), content },
                { nameof(element), element },
                { nameof(elementIndex), elementIndex },
            };

            if (ContentTypeCacheHelper.TryGetIcon(content.ContentType.Alias, out var contentIcon, Services.ContentTypeService) == true)
            {
                viewData.Add(nameof(contentIcon), contentIcon);
            }

            if (ContentTypeCacheHelper.TryGetIcon(element.ContentType.Alias, out var elementIcon, Services.ContentTypeService) == true)
            {
                viewData.Add(nameof(elementIcon), elementIcon);
            }

            string markup;

            try
            {
                markup = ContentBlocksViewHelper.RenderPartial(element.ContentType.Alias, viewData);
            }
            catch (InvalidCastException icex)
            {
                // NOTE: This type of exception happens on a new (unsaved) page, when the context becomes the parent page,
                // and the preview view is strongly typed to the current page's model type.
                markup = "<p class=\"text-center mt4\">Unable to render the preview until the page has been saved.</p>";

                _logger.Error<ContentBlocksApiController>(icex, "Error rendering preview view.");
            }
            catch (Exception ex)
            {
                markup = $"<pre class=\"error\"><code>{ex}</code></pre>";

                _logger.Error<ContentBlocksApiController>(ex, "Error rendering preview view.");
            }

            return Request.CreateResponse(HttpStatusCode.OK, new { elementKey, markup });
        }
    }
#else
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
            var contentCache = _umbracoContextAccessor.GetRequiredUmbracoContext().Content;

            var content = contentCache.GetById(true, contentId);
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
                    var contentType = contentCache.GetContentType(alias);
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
        // https://weblog.west-wind.com/posts/2022/Jun/21/Back-to-Basics-Rendering-Razor-Views-to-String-in-ASPNET-Core
        // https://gist.github.com/Matthew-Wise/80626bbf9c9590228fc317774f15222a
        private string RenderPartialViewToString(string viewName, ViewDataDictionary viewData)
        {
            IView view = default;

            // HACK: I couldn't figure out how to add custom view locations to the Razor view engine, so this is my hack.
            // If anyone knows of a better approach, the code is open to contributions. [LK:2022-04-15]
            var locations = new[]
            {
                $"/Views/Partials/Blocks/{viewName}.cshtml",
                "/Views/Partials/Blocks/Default.cshtml",
                "/App_Plugins/Contentment/render/ContentBlockPreview.cshtml",
            };

            var actionContext = new ActionContext(HttpContext, new RouteData(), new ActionDescriptor());
            var findViewResult = _viewEngine.FindView(actionContext, viewName, isMainPage: true);
            if (findViewResult.Success)
            {
                view = findViewResult.View;
            }
            else
            {
                foreach (var location in locations)
                {
                    var getViewResult = _viewEngine.GetView(executingFilePath: null, viewPath: location, isMainPage: true);
                    if (getViewResult.Success)
                    {
                        view = getViewResult.View;
                        break;
                    }
                }
            }

            if (view == default)
            {
                var messages = new List<string> { $"Unable to find view '{viewName}'. The following locations were searched:" };
                messages.AddRange(findViewResult.SearchedLocations);
                messages.AddRange(locations);

                throw new InvalidOperationException(string.Join(Environment.NewLine, messages));
            }

            using var output = new StringWriter();

            var tempDataDictionary = new TempDataDictionary(HttpContext, _tempDataProvider);
            var viewContext = new ViewContext(actionContext, view, viewData, tempDataDictionary, output, new HtmlHelperOptions());

            view.RenderAsync(viewContext).GetAwaiter().GetResult();

            return output.ToString();
        }
    }
#endif
}
