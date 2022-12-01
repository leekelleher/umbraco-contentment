using System;
using System.Collections.Generic;
using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Web;
using Umbraco.Community.Contentment.Web.Serialization;
using Umbraco.Extensions;

namespace Umbraco.Cms.Web.Common.Controllers
{
    [Route("api/[action]")]
    public class ContentmentApiController : UmbracoApiController
    {
        private readonly IUmbracoContextAccessor _umbracoContextAccessor;

        public ContentmentApiController(IUmbracoContextAccessor umbracoContextAccessor)
        {
            _umbracoContextAccessor = umbracoContextAccessor;
        }

        public ActionResult Json(Guid? key)
        {
            if (key.HasValue == true &&
                _umbracoContextAccessor.TryGetUmbracoContext(out var ctx) == true &&
                ctx is not null &&
                ctx.Content is not null)
            {
                var content = ctx.Content.GetById(key.Value);

                var settings = new JsonSerializerSettings
                {
                    Formatting = Formatting.Indented,
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    Converters = new List<JsonConverter>
                    {
                        new HtmlEncodedStringJsonConverter(),
                        new PublishedContentTypeJsonConverter(),
                        new PublishedPropertyTypeJsonConverter(),
                    },
                    ContractResolver = new PublishedContentContractResolver
                    {
                        SystemPropertyNamePrefix = "_",
                        PropertiesToIgnore = new[]
                        {
                            nameof(IPublishedContent.CreateDate),
                            nameof(IPublishedContent.Id),
                            nameof(IPublishedContent.ItemType),
                            nameof(IPublishedContent.Level),
                            nameof(IPublishedContent.Path),
                            nameof(IPublishedContent.SortOrder),
                            nameof(IPublishedContent.UpdateDate),
                            nameof(IPublishedContent.UrlSegment),
                            nameof(FriendlyPublishedContentExtensions.CreatorName),
                            nameof(FriendlyPublishedContentExtensions.WriterName),
                        },
                    },
                };

                var json = JsonConvert.SerializeObject(content, settings);

                return Content(json, new MediaTypeHeaderValue(MediaTypeNames.Application.Json));
            }

            return Content(string.Empty);
        }
    }
}
