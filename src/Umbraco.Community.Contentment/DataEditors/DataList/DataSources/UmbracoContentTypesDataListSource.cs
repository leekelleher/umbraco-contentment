﻿/* Copyright © 2020 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
#if NET472
using Umbraco.Core;
using Umbraco.Core.Dictionary;
using Umbraco.Core.IO;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Core.PropertyEditors;
using Umbraco.Core.Services;
using Umbraco.Core.Xml;
using Umbraco.Web;
using UmbConstants = Umbraco.Core.Constants;
#else
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Dictionary;
using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;
using Umbraco.Extensions;
using UmbConstants = Umbraco.Cms.Core.Constants;
#endif

namespace Umbraco.Community.Contentment.DataEditors
{
    public sealed class UmbracoContentTypesDataListSource : IDataListSource, IDataSourceValueConverter
    {
        private readonly IContentTypeService _contentTypeService;
        private readonly IUmbracoContextAccessor _umbracoContextAccessor;
        private readonly IIOHelper _ioHelper;
        private readonly ILocalizedTextService _localizedTextService;
        private readonly ICultureDictionary _cultureDictionary;

        public UmbracoContentTypesDataListSource(
            IContentTypeService contentTypeService,
            IUmbracoContextAccessor umbracoContextAccessor,
            ILocalizedTextService localizedTextService,
#if NET472 == false
            ICultureDictionary cultureDictionary,
#endif
            IIOHelper ioHelper)
        {
            _contentTypeService = contentTypeService;
            _umbracoContextAccessor = umbracoContextAccessor;
            _localizedTextService = localizedTextService;
#if NET472
            _cultureDictionary = Core.Composing.Current.CultureDictionaryFactory.CreateDictionary();
#else
            _cultureDictionary = cultureDictionary;
#endif
            _ioHelper = ioHelper;
        }

        public string Name => "Umbraco Content Types";

        public string Description => "Populate the data source using Content Types.";

        public string Icon => UmbConstants.Icons.ContentType;

        public string Group => Constants.Conventions.DataSourceGroups.Umbraco;

        public OverlaySize OverlaySize => OverlaySize.Small;

        public IEnumerable<ConfigurationField> Fields => new[]
        {
            new ConfigurationField
            {
                Key = "contentTypes",
                Name = "Content types",
                View = _ioHelper.ResolveRelativeOrVirtualUrl(CheckboxListDataListEditor.DataEditorViewPath),
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
                    { ShowDescriptionsConfigurationField.ShowDescriptions, Constants.Values.True },
                    { ShowIconsConfigurationField.ShowIcons, Constants.Values.True },
                }
            },
        };

        public Dictionary<string, object> DefaultValues => default;

        public IEnumerable<DataListItem> GetItems(Dictionary<string, object> config)
        {
            var types = config.GetValueAs("contentTypes", defaultValue: default(JArray))?.ToObject<IEnumerable<string>>();

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
                    Description = string.Join(", ", x.AllowedTemplates.Select(t => t.Alias)),
                });
        }

        public Type GetValueType(Dictionary<string, object> config) => typeof(IPublishedContentType);

        public object ConvertValue(Type type, string value)
        {
            if (UdiParser.TryParse(value, out GuidUdi udi) == true && ContentTypeCacheHelper.TryGetAlias(udi.Guid, out var alias, _contentTypeService) == true)
            {
                return _umbracoContextAccessor.GetRequiredUmbracoContext().Content.GetContentType(alias);
            }

            return default;
        }
    }
}
