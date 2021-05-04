/* Copyright © 2021 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Core;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.Entities;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Services;
using UmbConstants = Umbraco.Cms.Core.Constants;
using Umbraco.Extensions;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Strings;
using Umbraco.Cms.Core.IO;

namespace Umbraco.Community.Contentment.DataEditors
{
    [Cms.Core.Composing.HideFromTypeFinder]
    public sealed class UmbracoEntityDataListSource : IDataListSource, IDataListSourceValueConverter
    {
        internal static Dictionary<string, UmbracoObjectTypes> SupportedEntityTypes = new Dictionary<string, UmbracoObjectTypes>
        {
            { nameof(UmbracoObjectTypes.DataType), UmbracoObjectTypes.DataType },
            { nameof(UmbracoObjectTypes.Document), UmbracoObjectTypes.Document },
            { nameof(UmbracoObjectTypes.DocumentBlueprint), UmbracoObjectTypes.DocumentBlueprint },
            { nameof(UmbracoObjectTypes.DocumentType), UmbracoObjectTypes.DocumentType },
            { nameof(UmbracoObjectTypes.Media), UmbracoObjectTypes.Media },
            { nameof(UmbracoObjectTypes.MediaType), UmbracoObjectTypes.MediaType },
            { nameof(UmbracoObjectTypes.Member), UmbracoObjectTypes.Member },
            { nameof(UmbracoObjectTypes.MemberType), UmbracoObjectTypes.MemberType },
        };

        internal static Dictionary<string, string> EntityTypeIcons = new Dictionary<string, string>
        {
            { nameof(UmbracoObjectTypes.DataType), UmbConstants.Icons.DataType },
            { nameof(UmbracoObjectTypes.Document), UmbConstants.Icons.Content },
            { nameof(UmbracoObjectTypes.DocumentBlueprint), Constants.Icons.ContentTemplate },
            { nameof(UmbracoObjectTypes.DocumentType), UmbConstants.Icons.ContentType },
            { nameof(UmbracoObjectTypes.Media), UmbConstants.Icons.MediaImage },
            { nameof(UmbracoObjectTypes.MediaType), UmbConstants.Icons.MediaType },
            { nameof(UmbracoObjectTypes.Member), UmbConstants.Icons.Member },
            { nameof(UmbracoObjectTypes.MemberType), UmbConstants.Icons.MemberType },
        };

        private readonly IEntityService _entityService;
        private readonly IShortStringHelper _shortStringHelper;
        private readonly IIOHelper _ioHelper;

        public UmbracoEntityDataListSource(
            IEntityService entityService,
            IShortStringHelper shortStringHelper,
            IIOHelper ioHelper)
        {
            _entityService = entityService;
            _shortStringHelper = shortStringHelper;
            _ioHelper = ioHelper;
        }

        public string Name => "Umbraco Entities";

        public string Description => "Select an Umbraco entity type to populate the data source.";

        public string Icon => "icon-umbraco";

        public string Group => default;

        public IEnumerable<ConfigurationField> Fields => new ConfigurationField[]
        {
            new NotesConfigurationField(_ioHelper, @"<details class=""well well-small"">
<summary><strong>A note about supported Umbraco entity types.</strong></summary>
<p>Umbraco's <code>EntityService</code> API (currently) has limited support for querying entity types by <abbr title=""Globally Unique Identifier"">GUID</abbr> or <abbr title=""Umbraco Data Identifier"">UDI</abbr>.</p>
<p>Supported entity types are available in the list below.</p>
</details>", true),
            new ConfigurationField
            {
                Key = "entityType",
                Name = "Entity type",
                Description = "Select the Umbraco entity type to use.",
                View = DropdownListDataListEditor.DataEditorViewPath,
                Config = new Dictionary<string, object>()
                {
                    { "allowEmpty", Constants.Values.False },
                    { "items", SupportedEntityTypes.Keys.Select(x => new DataListItem { Name = x.SplitPascalCasing(_shortStringHelper), Value = x }) },
                }
            }
        };

        public Dictionary<string, object> DefaultValues => default;

        public OverlaySize OverlaySize => OverlaySize.Small;

        public IEnumerable<DataListItem> GetItems(Dictionary<string, object> config)
        {
            if (config.TryGetValueAs("entityType", out string entityType) == true && SupportedEntityTypes.TryGetValue(entityType, out var objectType) == true)
            {
                var icon = EntityTypeIcons.GetValueAs(entityType, UmbConstants.Icons.DefaultIcon);

                return _entityService
                    .GetAll(objectType)
                    .OrderBy(x => x.Name)
                    .Select(x => new DataListItem
                    {
                        Icon = icon,
                        Name = x.Name,
                        Value = Udi.Create(UmbConstants.UdiEntityType.FromUmbracoObjectType(objectType), x.Key).ToString(),
                    });
            }

            return Enumerable.Empty<DataListItem>();
        }

        public Type GetValueType(Dictionary<string, object> config) => typeof(IEntitySlim);

        public object ConvertValue(Type type, string value)
        {
            return UdiParser.TryParse(value, out GuidUdi udi) == true && udi.Guid.Equals(Guid.Empty) == false
                ? _entityService.Get(udi.Guid)
                : default;
        }
    }
}
