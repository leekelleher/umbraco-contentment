using System;
using System.Collections.Generic;
using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;
using Umbraco.Community.Contentment.Web.Serialization;

namespace Umbraco.Cms.Web.Common.Controllers
{
    [Route("api/[action]")]
    public class ContentmentApiController : UmbracoApiController
    {
        private readonly IDataTypeService _dataTypeService;
        private readonly IUmbracoContextAccessor _umbracoContextAccessor;
        private readonly PropertyEditorCollection _propertyEditors;

        public ContentmentApiController(
            IDataTypeService dataTypeService,
            IUmbracoContextAccessor umbracoContextAccessor,
            PropertyEditorCollection propertyEditors)
        {
            _dataTypeService = dataTypeService;
            _umbracoContextAccessor = umbracoContextAccessor;
            _propertyEditors = propertyEditors;
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

            return NotFound();
        }

        public ActionResult GetDataSourceItemsByDataType(Guid? key)
        {
            if (key.HasValue == true &&
                _dataTypeService.GetDataType(key.Value) is IDataType dataType &&
                dataType?.EditorAlias.InvariantEquals("Umbraco.Community.Contentment.DataList") == true &&
                dataType.Configuration is Dictionary<string, object> config &&
                _propertyEditors.TryGet(dataType.EditorAlias, out var propertyEditor) == true)
            {
                var configurationEditor = propertyEditor.GetConfigurationEditor();
                var valueEditorConfig = configurationEditor.ToValueEditor(config);

                return Ok(valueEditorConfig?["items"]);
            }

            return NotFound();
        }
    }
}
