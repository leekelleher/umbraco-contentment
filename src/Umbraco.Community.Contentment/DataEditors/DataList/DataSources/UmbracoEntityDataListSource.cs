/* Copyright © 2021 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.Entities;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Strings;
using Umbraco.Extensions;

namespace Umbraco.Community.Contentment.DataEditors
{
    public sealed class UmbracoEntityDataListSource : DataListToDataPickerSourceBridge, IContentmentDataSource, IDataSourceValueConverter
    {
        internal static Dictionary<string, UmbracoObjectTypes> SupportedEntityTypes = new()
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

        internal static Dictionary<string, string> EntityTypeIcons = new()
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

        private readonly Lazy<IEntityService> _entityService;
        private readonly IShortStringHelper _shortStringHelper;

        public UmbracoEntityDataListSource(
            Lazy<IEntityService> entityService,
            IShortStringHelper shortStringHelper)
        {
            _entityService = entityService;
            _shortStringHelper = shortStringHelper;
        }

        public override string Name => "Umbraco Entities";

        public override string Description => "Select an Umbraco entity type to populate the data source.";

        public override string Icon => "icon-lab";

        public override string Group => Constants.Conventions.DataSourceGroups.Umbraco;

        public override IEnumerable<ContentmentConfigurationField> Fields => new ContentmentConfigurationField[]
        {
            new NotesConfigurationField(@"<details class=""well well-small"">
<summary><strong>A note about supported Umbraco entity types.</strong></summary>
<p>Umbraco's <a href=""https://github.com/umbraco/Umbraco-CMS/blob/release-8.17.0/src/Umbraco.Core/Services/Implement/EntityService.cs"" target=""_blank""><code>EntityService</code></a> API (currently) has limited support for querying entity types by <abbr title=""Globally Unique Identifier"">GUID</abbr> or <abbr title=""Umbraco Data Identifier"">UDI</abbr>.</p>
<p>Supported entity types are available in the list below.</p>
</details>", true),
            new ContentmentConfigurationField
            {
                Key = "entityType",
                Name = "Entity type",
                Description = "Select the Umbraco entity type to use.",
                PropertyEditorUiAlias = RadioButtonListDataListEditor.DataEditorUiAlias,
                Config = new Dictionary<string, object>()
                {
                    { "allowEmpty", false },
                    { "items", SupportedEntityTypes.Keys.Select(x => new DataListItem
                        {
                            Name = x.SplitPascalCasing(_shortStringHelper),
                            Value = x
                        })
                    },
                }
            }
        };

        public override Dictionary<string, object>? DefaultValues => default;

        public override OverlaySize OverlaySize => OverlaySize.Small;

        public override IEnumerable<DataListItem> GetItems(Dictionary<string, object> config)
        {
            if (config.TryGetValueAs("entityType", out string? entityType) == true &&
                string.IsNullOrWhiteSpace(entityType) == false &&
                SupportedEntityTypes.TryGetValue(entityType, out var objectType) == true)
            {
                var icon = EntityTypeIcons.GetValueAs(entityType, UmbConstants.Icons.DefaultIcon);

                return _entityService
                    .Value
                    .GetAll(objectType)
                    .OrderBy(x => x.Name)
                    .Select(x => new DataListItem
                    {
                        Icon = icon,
                        Name = x.Name,
                        Value = Udi.Create(objectType.GetUdiType(), x.Key).ToString(),
                    });
            }

            return Enumerable.Empty<DataListItem>();
        }

        public Type? GetValueType(Dictionary<string, object>? config) => typeof(IEntitySlim);

        public object? ConvertValue(Type type, string value)
        {
            return UdiParser.TryParse(value, out GuidUdi? udi) == true && udi?.Guid.Equals(Guid.Empty) == false
                ? _entityService.Value.Get(udi.Guid)
                : default;
        }
    }
}
