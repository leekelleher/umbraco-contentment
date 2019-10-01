/* Copyright © 2019 Lee Kelleher, Umbrella Inc and other contributors.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using Umbraco.Core;
using Umbraco.Core.IO;
using Umbraco.Core.Models;
using Umbraco.Core.PropertyEditors;
using Umbraco.Core.Services;

namespace Umbraco.Community.Contentment.DataEditors
{
    internal sealed class ElementConfigurationEditor : ConfigurationEditor
    {
        private readonly IContentService _contentService;
        private readonly IContentTypeService _contentTypeService;
        private readonly IdkMap _idkMap;

        internal const string OverlayView = "overlayView";

        public ElementConfigurationEditor(IContentService contentService, IContentTypeService contentTypeService, IdkMap idkMap)
            : base()
        {
            _contentService = contentService;
            _contentTypeService = contentTypeService;
            _idkMap = idkMap;

            Fields.Add(new ElementTypesConfigurationField(contentTypeService));
            Fields.Add(new EnableFilterConfigurationField());
            Fields.Add(new OverlaySizeConfigurationField { Name = "Editor overlay size" });
            Fields.Add(new MaxItemsConfigurationField());
            Fields.Add(new DisableSortingConfigurationField());
        }

        public override IDictionary<string, object> ToValueEditor(object configuration)
        {
            var config = base.ToValueEditor(configuration);

            if (config.TryGetValue(ElementTypesConfigurationField.ElementTypes, out var tmp) && tmp is JArray array && array.Count > 0)
            {
                var ids = new int[array.Count];

                for (var i = 0; i < array.Count; i++)
                {
                    // NOTE: [LK:2019-08-05] Why `IContentTypeService` doesn't support UDIs, I do not know!?
                    // Thought v8 was meant to be "GUID ALL THE THINGS!!1"? ¯\_(ツ)_/¯

                    if (GuidUdi.TryParse(array[i].Value<string>(), out var udi))
                    {
                        var attempt = _idkMap.GetIdForKey(udi.Guid, UmbracoObjectTypes.DocumentType);
                        if (attempt.Success)
                        {
                            ids[i] = attempt.Result;
                        }
                    }
                }

                var elementTypes = new List<ElementTypeItem>();

                if (ids.Length > 0)
                {
                    var contentTypes = _contentTypeService.GetAll(ids);

                    // NOTE: Gets all the blueprints in one hit, rather than several inside the loop.
                    var allBlueprints = _contentService.GetBlueprintsForContentTypes(ids).ToLookup(x => x.ContentTypeId);

                    foreach (var contentType in contentTypes)
                    {
                        var blueprints = allBlueprints[contentType.Id]?.Select(x => new ElementTypeItem.BlueprintItem
                        {
                            Id = x.Id,
                            Name = x.Name
                        }) ?? Enumerable.Empty<ElementTypeItem.BlueprintItem>();

                        elementTypes.Add(new ElementTypeItem
                        {
                            Alias = contentType.Alias,
                            Name = contentType.Name,
                            Description = contentType.Description,
                            Icon = contentType.Icon,
                            Key = contentType.Key,
                            Blueprints = blueprints
                        });
                    }
                }

                config[ElementTypesConfigurationField.ElementTypes] = elementTypes;
            }

            if (config.ContainsKey(OverlayView) == false)
            {
                config.Add(OverlayView, IOHelper.ResolveUrl(ElementDataEditor.DataEditorOverlayViewPath));
            }

            return config;
        }
    }
}
