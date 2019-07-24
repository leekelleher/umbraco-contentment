/* Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Umbraco.Core.IO;
using Umbraco.Core.Models;
using Umbraco.Core.PropertyEditors;
using Umbraco.Core.Services;

namespace Umbraco.Community.Contentment.DataEditors
{
    internal sealed class ContentBlocksConfigurationEditor : ConfigurationEditor
    {
        private readonly IContentService _contentService;
        private readonly IContentTypeService _contentTypeService;
        private readonly IdkMap _idkMap;

        internal const string OverlayView = "overlayView";

        public ContentBlocksConfigurationEditor(IContentService contentService, IContentTypeService contentTypeService, IdkMap idkMap)
            : base()
        {
            _contentService = contentService;
            _contentTypeService = contentTypeService;
            _idkMap = idkMap;

            Fields.Add(new ContentBlocksTypesConfigurationField(contentTypeService, new ConfigurationEditorService()));
            Fields.Add(new ContentBlocksDisplayModeConfigurationField());
            Fields.Add(new EnableFilterConfigurationField());
            Fields.Add(new MaxItemsConfigurationField());
            Fields.Add(new DisableSortingConfigurationField());
            Fields.Add(new HideLabelConfigurationField());
        }

        public override IDictionary<string, object> ToValueEditor(object configuration)
        {
            var config = base.ToValueEditor(configuration);

            if (config.TryGetValue(ContentBlocksTypesConfigurationField.ContentBlockTypes, out var tmp) && tmp is JArray array && array.Count > 0)
            {
                var lookup = new Dictionary<int, ContentBlocksTypeConfiguration>();

                for (var i = 0; i < array.Count; i++)
                {
                    // NOTE: [LK:2019-08-05] Why `IContentTypeService.GetAll` doesn't support GUIDs/UDIs, I do not know!?
                    // Thought v8 was meant to be "GUID ALL THE THINGS!!1"? ¯\_(ツ)_/¯

                    if (Guid.TryParse(array[i].Value<string>("type"), out var guid))
                    {
                        var attempt = _idkMap.GetIdForKey(guid, UmbracoObjectTypes.DocumentType);
                        if (attempt.Success)
                        {
                            var id = attempt.Result;
                            if (lookup.ContainsKey(id) == false)
                            {
                                var serializer = JsonSerializer.CreateDefault(new Serialization.ConfigurationFieldJsonSerializerSettings());
                                var cbtc = array[i]["value"].ToObject<ContentBlocksTypeConfiguration>(serializer);
                                if (cbtc != null)
                                {
                                    lookup.Add(id, cbtc);
                                }
                            }
                        }
                    }
                }

                var elementTypes = new List<ContentBlockType>();

                if (lookup.Count > 0)
                {
                    var ids = lookup.Keys.ToArray();
                    var contentTypes = _contentTypeService.GetAll(ids);

                    // NOTE: Gets all the blueprints in one hit, rather than several inside the loop.
                    var allBlueprints = _contentService.GetBlueprintsForContentTypes(ids).ToLookup(x => x.ContentTypeId);

                    // TODO: [LK:2019-11-22] Refactor to order the elementTypes as they are in the original array.

                    foreach (var contentType in contentTypes)
                    {
                        var settings = lookup[contentType.Id];

                        var blueprints = allBlueprints.Contains(contentType.Id)
                            ? allBlueprints[contentType.Id].Select(x => new ContentBlockType.BlueprintItem { Id = x.Id, Name = x.Name })
                            : Enumerable.Empty<ContentBlockType.BlueprintItem>();

                        elementTypes.Add(new ContentBlockType
                        {
                            Alias = contentType.Alias,
                            Name = contentType.Name,
                            Description = contentType.Description,
                            Icon = contentType.Icon,
                            Key = contentType.Key,
                            Blueprints = blueprints,
                            NameTemplate = settings?.NameTemplate,
                            OverlaySize = settings?.OverlaySize,
                            PreviewEnabled = settings?.EnablePreview == true,
                        });
                    }
                }

                config[ContentBlocksTypesConfigurationField.ContentBlockTypes] = elementTypes;
            }

            if (config.ContainsKey(OverlayView) == false)
            {
                config.Add(OverlayView, IOHelper.ResolveUrl(ContentBlocksDataEditor.DataEditorOverlayViewPath));
            }

            return config;
        }
    }
}
