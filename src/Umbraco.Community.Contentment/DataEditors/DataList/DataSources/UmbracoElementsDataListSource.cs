// SPDX-License-Identifier: MPL-2.0
// Copyright © 2025 Lee Kelleher

using Umbraco.Cms.Api.Common.ViewModels.Pagination;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.DeliveryApi;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.DeliveryApi;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.PublishedCache;
using Umbraco.Cms.Core.Serialization;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Services.Navigation;
using Umbraco.Extensions;

namespace Umbraco.Community.Contentment.DataEditors
{
    public sealed class UmbracoElementsDataListSource
        : IContentmentDataSource, IDataPickerSource, IDataSourceValueConverter, IDataSourceDeliveryApiValueConverter
    {
        private readonly IApiElementBuilder _apiElementBuilder;
        private readonly IContentTypeService _contentTypeService;
        private readonly IElementNavigationQueryService _elementNavigationQueryService;
        private readonly IEntityService _entityService;
        private readonly IJsonSerializer _jsonSerializer;
        private readonly IPublishedElementCache _publishedElementCache;

        private const string DefaultImageAlias = "image";

        public UmbracoElementsDataListSource(
            IApiElementBuilder apiElementBuilder,
            IContentTypeService contentTypeService,
            IElementNavigationQueryService elementNavigationQueryService,
            IEntityService entityService,
            IJsonSerializer jsonSerializer,
            IPublishedElementCache publishedElementCache)
        {
            _apiElementBuilder = apiElementBuilder;
            _contentTypeService = contentTypeService;
            _elementNavigationQueryService = elementNavigationQueryService;
            _entityService = entityService;
            _jsonSerializer = jsonSerializer;
            _publishedElementCache = publishedElementCache;
        }

        public string Name => "Umbraco Elements";

        public string Description => "Select an element folder to use its elements as the data source.";

        public string Icon => "icon-umbraco";

        public Dictionary<string, object>? DefaultValues => default;

        public IEnumerable<ContentmentConfigurationField> Fields => new ContentmentConfigurationField[]
        {
            new ContentmentConfigurationField
            {
                Key = "folder",
                Name = "Element folder",
                Description = "Set an element folder to use its elements as the data source items.",
                PropertyEditorUiAlias = "Umb.Contentment.PropertyEditorUi.ElementFolderPicker",
            },
            new ContentmentConfigurationField
            {
                Key = "elementTypes",
                Name = "Element type filter",
                Description = "Select one or more element types to filter the folder elements. By default, all elements are returned regardless of their type.",
                PropertyEditorUiAlias = "Umb.PropertyEditorUi.DocumentTypePicker",
                Config = new Dictionary<string, object>
                {
                    { "onlyPickElementTypes", true },
                },
            },
            new ContentmentConfigurationField
            {
                Key = "imageAlias",
                Name = "Image alias",
                Description = $"When using the Cards display mode, you can set a thumbnail image by entering the property alias of a media picker. The default alias is '{DefaultImageAlias}'.",
                PropertyEditorUiAlias = "Umb.PropertyEditorUi.TextBox",
            },
            new ContentmentConfigurationField
            {
                Key = "sortAlphabetically",
                Name = "Sort alphabetically?",
                Description = "Select to sort the elements in alphabetical order.<br>By default, the order is defined by the Umbraco element sort order.",
                PropertyEditorUiAlias = "Umb.PropertyEditorUi.Toggle",
            },
        };

        public string Group => Constants.Conventions.DataSourceGroups.Umbraco;

        public OverlaySize OverlaySize => OverlaySize.Medium;

        public IEnumerable<DataListItem> GetItems(Dictionary<string, object> config)
        {
            var preview = true;
            var folderKey = GetStartFolderKey(config);
            var imageAlias = config.GetValueAs("imageAlias", DefaultImageAlias) ?? DefaultImageAlias;
            var elementTypeKeys = GetElementTypeFilter(config);

            var items = GetFolderElements(folderKey, preview)
                .Where(x => IsElementTypeMatch(x, elementTypeKeys))
                .Select(x => ToDataListItem(x, imageAlias));

            if (config.TryGetValueAs("sortAlphabetically", out bool sortAlphabetically) == true && sortAlphabetically == true)
            {
                return items.OrderBy(x => x.Name, StringComparer.InvariantCultureIgnoreCase);
            }

            return items;
        }

        public Task<IEnumerable<DataListItem>> GetItemsAsync(Dictionary<string, object> config, IEnumerable<string> values)
        {
            if (values?.Any() == true)
            {
                var preview = true;
                var imageAlias = config.GetValueAs("imageAlias", DefaultImageAlias) ?? DefaultImageAlias;
                var elementTypeKeys = GetElementTypeFilter(config);

                var elements = values
                    .Select(x => UdiParser.TryParse(x, out GuidUdi? udi) == true ? udi : null)
                    .WhereNotNull()
                    .Where(x => x.EntityType == UmbConstants.UdiEntityType.Element && x.Guid.Equals(Guid.Empty) == false)
                    .Select(x => _publishedElementCache.GetById(preview, x.Guid))
                    .WhereNotNull()
                    .Where(x => IsElementTypeMatch(x, elementTypeKeys));

                return Task.FromResult(elements.Select(x => ToDataListItem(x, imageAlias)));
            }

            return Task.FromResult(Enumerable.Empty<DataListItem>());
        }

        public Task<PagedViewModel<DataListItem>> SearchAsync(Dictionary<string, object> config, int pageNumber = 1, int pageSize = 12, string query = "")
        {
            var preview = true;
            var folderKey = GetStartFolderKey(config);
            var elementTypeKeys = GetElementTypeFilter(config);

            {
                var items = GetFolderElements(folderKey, preview)
                    .Where(x => IsElementTypeMatch(x, elementTypeKeys));

                if (string.IsNullOrWhiteSpace(query) == false)
                {
                    items = items.Where(x => x.Name?.Contains(query, StringComparison.InvariantCultureIgnoreCase) == true);
                }

                if (items?.Any() == true)
                {
                    var imageAlias = config.GetValueAs("imageAlias", DefaultImageAlias) ?? DefaultImageAlias;
                    var offset = (pageNumber - 1) * pageSize;

                    if (config.TryGetValueAs("sortAlphabetically", out bool sortAlphabetically) == true && sortAlphabetically == true)
                    {
                        items = items.OrderBy(x => x.Name, StringComparer.InvariantCultureIgnoreCase);
                    }

                    var results = new PagedViewModel<DataListItem>
                    {
                        Items = items.Skip(offset).Take(pageSize).Select(x => ToDataListItem(x, imageAlias)),
                        Total = pageSize > 0 ? (long)Math.Ceiling(items.Count() / (decimal)pageSize) : 1,
                    };

                    return Task.FromResult(results);
                }
            }

            return Task.FromResult(PagedViewModel<DataListItem>.Empty());
        }

        public Type? GetValueType(Dictionary<string, object>? config) => typeof(IPublishedElement);

        public object? ConvertValue(Type type, string value)
        {
            return UdiParser.TryParse(value, out GuidUdi? udi) == true &&
                   udi is not null &&
                   udi.EntityType == UmbConstants.UdiEntityType.Element &&
                   udi.Guid.Equals(Guid.Empty) == false
                ? _publishedElementCache.GetById(false, udi.Guid)
                : default;
        }

        public Type? GetDeliveryApiValueType(Dictionary<string, object>? config) => typeof(IApiElement);

        public object? ConvertToDeliveryApiValue(Type type, string value, bool expanding = false)
            => ConvertValue(type, value) is IPublishedElement element ? _apiElementBuilder.Build(element) : default;

        private Guid? GetStartFolderKey(Dictionary<string, object> config)
        {
            // The Element Picker (with folderOnly: true) stores its value as a JSON array of bare GUIDs.
            var raw = config.GetValueAs("folder", string.Empty);
            if (string.IsNullOrWhiteSpace(raw) == true)
            {
                return null;
            }

            var keys = _jsonSerializer.Deserialize<IEnumerable<Guid>>(raw);
            var key = keys?.FirstOrDefault();
            return key.HasValue == true && key.Value.Equals(Guid.Empty) == false ? key : null;
        }

        private IEnumerable<IPublishedElement> GetFolderElements(Guid? folderKey, bool preview)
        {
            IEnumerable<Guid> keys;

            if (folderKey is null)
            {
                // No folder configured — use the root of the Element Library.
                // TryGetRootKeys returns both element and folder keys; the element cache
                // returns null for folder keys, so WhereNotNull() filters them out.
                if (_elementNavigationQueryService.TryGetRootKeys(out var rootKeys) == false)
                {
                    return Enumerable.Empty<IPublishedElement>();
                }

                keys = rootKeys;
            }
            else
            {
                // Enumerate elements inside the folder, excluding sub-folders.
                // Passing both object types as parentObjectTypes lets IEntityService resolve
                // a folder (ElementContainer) key; restricting childObjectTypes to Element
                // ensures only elements are returned.
                var entities = _entityService.GetPagedChildren(
                    folderKey,
                    new[] { UmbracoObjectTypes.Element, UmbracoObjectTypes.ElementContainer },
                    new[] { UmbracoObjectTypes.Element },
                    skip: 0,
                    take: int.MaxValue,
                    trashed: false,
                    out _);

                keys = entities.Select(x => x.Key);
            }

            return keys
                .Select(key => _publishedElementCache.GetById(preview, key))
                .WhereNotNull();
        }

        private IReadOnlyList<Guid>? GetElementTypeFilter(Dictionary<string, object> config)
        {
            // The Document Type Picker stores its value as a comma-separated string of GUIDs.
            // We compare them directly against IPublishedElement.ContentType.Key.
            var value = config.GetValueAs("elementTypes", string.Empty);
            if (string.IsNullOrWhiteSpace(value) == true)
            {
                return null;
            }

            var keys = new List<Guid>();

            foreach (var token in value.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            {
                if (Guid.TryParse(token, out var key) == true)
                {
                    keys.Add(key);
                }
            }

            return keys.Count > 0 ? keys : null;
        }

        private static bool IsElementTypeMatch(IPublishedElement element, IReadOnlyList<Guid>? elementTypeKeys)
            => elementTypeKeys is null || elementTypeKeys.Contains(element.ContentType.Key) == true;

        private DataListItem ToDataListItem(IPublishedElement element, string imageAlias = DefaultImageAlias)
        {
            var contentTypeName = _contentTypeService.Get(element.ContentType.Key)?.Name ?? element.ContentType.Alias;

            return new DataListItem
            {
                Name = element.Name,
                Description = contentTypeName,
                //Disabled = element.IsPublished() == false,
                Icon = element.ContentType.GetIcon(_contentTypeService),
                Properties = new Dictionary<string, object>
                {
                    { DefaultImageAlias, element.Value<IPublishedContent>(imageAlias)?.Url() ?? string.Empty },
                },
                Value = element.GetUdi().ToString(),
            };
        }
    }
}
