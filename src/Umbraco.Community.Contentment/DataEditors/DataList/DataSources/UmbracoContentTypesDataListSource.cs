/* Copyright © 2020 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Dictionary;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.PublishedCache;
using Umbraco.Cms.Core.Services;
using Umbraco.Extensions;

namespace Umbraco.Community.Contentment.DataEditors
{
    public sealed class UmbracoContentTypesDataListSource
        : DataListToDataPickerSourceBridge, IContentmentDataSource, IDataSourceValueConverter, IDataSourceDeliveryApiValueConverter
    {
        private readonly IContentTypeService _contentTypeService;
        private readonly IPublishedContentTypeCache _publishedContentTypeCache;
        private readonly ILocalizedTextService _localizedTextService;
        private readonly ICultureDictionary _cultureDictionary;

        public UmbracoContentTypesDataListSource(
            IContentTypeService contentTypeService,
            IPublishedContentTypeCache publishedContentTypeCache,
            ILocalizedTextService localizedTextService,
            ICultureDictionary cultureDictionary)
        {
            _contentTypeService = contentTypeService;
            _publishedContentTypeCache = publishedContentTypeCache;
            _localizedTextService = localizedTextService;
            _cultureDictionary = cultureDictionary;
        }

        public override string Name => "Umbraco Content Types";

        public override string Description => "Populate the data source using Content Types.";

        public override string Icon => UmbConstants.Icons.ContentType;

        public override string Group => Constants.Conventions.DataSourceGroups.Umbraco;

        public override OverlaySize OverlaySize => OverlaySize.Small;

        public override IEnumerable<ContentmentConfigurationField> Fields => new[]
        {
            new ContentmentConfigurationField
            {
                Key = "contentTypes",
                Name = "Content types",
                PropertyEditorUiAlias = CheckboxListDataListEditor.DataEditorUiAlias,
                Description = "Select the types to use.",
                Config = new Dictionary<string, object>
                {
                    { Constants.Conventions.ConfigurationFieldAliases.Items, new[]
                        {
                            new DataListItem
                            {
                                Name = "Document Types with Template",
                                Value = "documentTypesTemplates",
                                Icon = UmbConstants.Icons.Content,
                                Description = "For content pages that are accessible by a URL.",
                            },
                            new DataListItem
                            {
                                Name = "Document Types",
                                Value = "documentTypes",
                                Icon = UmbConstants.Icons.ContentType,
                                Description = "For content nodes available in the tree, but not accessible by a URL.",
                            },
                            new DataListItem
                            {
                                Name = "Element Types",
                                Value = "elementTypes",
                                Icon = "icon-science",
                                Description = "For inline content, typically used with the 'Block List' property editor.",
                            },
                        }
                    },
                    { Constants.Conventions.ConfigurationFieldAliases.DefaultValue, "documentTypesTemplates" },
                    { ShowDescriptionsConfigurationField.ShowDescriptions, true },
                    { ShowIconsConfigurationField.ShowIcons, true },
                }
            },
        };

        public override Dictionary<string, object>? DefaultValues => default;

        public override IEnumerable<DataListItem> GetItems(Dictionary<string, object> config)
        {
            var types = config.GetValueAs("contentTypes", defaultValue: default(IEnumerable<string>));

            var isDocumentTypeWithTemplate = types?.InvariantContains("documentTypesTemplates") == true;
            var isDocumentType = types?.InvariantContains("documentTypes") == true;
            var isElementType = types?.InvariantContains("elementTypes") == true;

            return _contentTypeService
                .GetAll()
                .Where(x =>
                {
                    var result = false;

                    if (isDocumentTypeWithTemplate == true)
                    {
                        result |= x.IsElement == false && x.AllowedTemplates?.Any() == true;
                    }

                    if (isDocumentType == true)
                    {
                        result |= x.IsElement == false && x.AllowedTemplates?.Any() == false;
                    }

                    if (isElementType == true)
                    {
                        result |= x.IsElement == true;
                    }

                    return result;
                })
                .OrderBy(x => x.Name)
                .Select(x => new DataListItem
                {
                    Name = _localizedTextService.UmbracoDictionaryTranslate(_cultureDictionary, x.Name),
                    Value = Udi.Create(UmbConstants.UdiEntityType.DocumentType, x.Key).ToString(),
                    Icon = x.Icon,
                    Description = string.Join(", ", x.AllowedTemplates?.Select(t => t.Alias) ?? Enumerable.Empty<string>()),
                });
        }

        public Type? GetValueType(Dictionary<string, object>? config) => typeof(IPublishedContentType);

        public object? ConvertValue(Type type, string value)
        {
            if (UdiParser.TryParse(value, out GuidUdi? udi) == true && udi is not null)
            {
                return _publishedContentTypeCache.Get(PublishedItemType.Content, udi.Guid);
            }

            return default;
        }

        public Type? GetDeliveryApiValueType(Dictionary<string, object>? config) => typeof(string);

        public object? ConvertToDeliveryApiValue(Type type, string value, bool expanding = false)
            => ConvertValue(type, value) is IPublishedContentType contentType ? contentType.Alias : value;
    }
}
