/* Copyright © 2019 Lee Kelleher, Umbrella Inc and other contributors.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Collections.Generic;
using System.Linq;
using Umbraco.Core;
using Umbraco.Core.Composing;
using Umbraco.Core.IO;
using Umbraco.Core.Models;
using Umbraco.Core.PropertyEditors;
using Umbraco.Core.Services;
using Umbraco.Web.Models.ContentEditing;
using UmbracoConstants = Umbraco.Core.Constants;

namespace Umbraco.Community.Contentment.DataEditors
{
    internal class UmbracoEntityDataListSource : IDataListSource
    {
        internal static Dictionary<string, UmbracoObjectTypes> SupportedEntityTypes = new Dictionary<string, UmbracoObjectTypes>
        {
            { nameof(UmbracoObjectTypes.DataType), UmbracoObjectTypes.DataType },
            { nameof(UmbracoObjectTypes.Document), UmbracoObjectTypes.Document },
            { nameof(UmbracoObjectTypes.DocumentType), UmbracoObjectTypes.DocumentType },
            { nameof(UmbracoObjectTypes.Media), UmbracoObjectTypes.Media },
            { nameof(UmbracoObjectTypes.MediaType), UmbracoObjectTypes.MediaType },
            { nameof(UmbracoObjectTypes.Member), UmbracoObjectTypes.Member },
            { nameof(UmbracoObjectTypes.MemberType), UmbracoObjectTypes.MemberType },
        };

        internal static Dictionary<string, string> EntityTypeIcons = new Dictionary<string, string>
        {
            { nameof(UmbracoObjectTypes.DataType), UmbracoConstants.Icons.DataType },
            { nameof(UmbracoObjectTypes.DocumentType), UmbracoConstants.Icons.ContentType },
            { nameof(UmbracoObjectTypes.MediaType), UmbracoConstants.Icons.MediaType },
            { nameof(UmbracoObjectTypes.Member),  UmbracoConstants.Icons.Member },
            { nameof(UmbracoObjectTypes.MemberType),  UmbracoConstants.Icons.MemberType },
        };

        private readonly IEntityService _entityService;

        public UmbracoEntityDataListSource()
            : this(Current.Services.EntityService)
        { }

        public UmbracoEntityDataListSource(IEntityService entityService)
        {
            _entityService = entityService;
        }

        public string Name => "Umbraco Entity";

        public string Description => "Select an Umbraco entity type to populate the data source.";

        public string Icon => "icon-science";

        [ConfigurationField(typeof(UmbracoEntityNotesConfigurationField))]
        public string Notes { get; set; }

        [ConfigurationField(typeof(EntityTypeConfigurationField))]
        public string EntityType { get; set; }

        public IEnumerable<DataListItem> GetItems()
        {
            if (SupportedEntityTypes.TryGetValue(EntityType, out var objectType))
            {
                var icon = EntityTypeIcons.ContainsKey(EntityType) ? EntityTypeIcons[EntityType] : this.Icon;

                return _entityService
                    .GetAll(objectType)
                    .OrderBy(x => x.Name)
                    .Select(x => new DataListItem
                    {
                        Icon = icon,
                        Name = x.Name,
                        Value = Udi.Create(UmbracoConstants.UdiEntityType.FromUmbracoObjectType(objectType), x.Key).ToString(),
                    });
            }

            return null;
        }

        class UmbracoEntityNotesConfigurationField : NotesConfigurationField
        {
            public UmbracoEntityNotesConfigurationField()
                : base(@"<p class=""alert alert-warning""><strong>A note about supported Umbraco entity types.</strong><br>
The Umbraco `EntityService` API has support for a limited set of entity types. Typically, these are entities that would be given a unique Id, (e.g. a Guid or UDI).<br>
Unsupported entity types have been removed from the list below.</p>", true)
            { }
        }

        class EntityTypeConfigurationField : ConfigurationField
        {
            public const string EntityType = "entityType";

            public EntityTypeConfigurationField()
                : base()
            {
                var items = SupportedEntityTypes.Keys.Select(x => new DataListItem { Name = x.SplitPascalCasing(), Value = x });

                Key = EntityType;
                Name = "Entity type";
                Description = "Select the Umbraco entity type to use.";
                View = IOHelper.ResolveUrl(DropdownListDataEditor.DataEditorViewPath);
                Config = new Dictionary<string, object>()
                {
                    { AllowEmptyConfigurationField.AllowEmpty, Constants.Values.False },
                    { DropdownListConfigurationEditor.Items, items },
                };
            }
        }
    }
}
