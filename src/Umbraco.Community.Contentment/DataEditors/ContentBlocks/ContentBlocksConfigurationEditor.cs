/* Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Umbraco.Core;
using Umbraco.Core.IO;
using Umbraco.Core.Models;
using Umbraco.Core.PropertyEditors;
using Umbraco.Core.Services;

namespace Umbraco.Community.Contentment.DataEditors
{
    internal sealed class ContentBlocksConfigurationEditor : ConfigurationEditor
    {
        private readonly Dictionary<Guid, IContentType> _elementTypes;
        private readonly Lazy<ILookup<int, IContent>> _elementBlueprints;

        internal const string OverlayView = "overlayView";

        public ContentBlocksConfigurationEditor(IContentService contentService, IContentTypeService contentTypeService)
            : base()
        {
            // NOTE: Gets all the elementTypes and blueprints upfront, rather than several hits inside the loop.
            _elementTypes = contentTypeService.GetAllElementTypes().ToDictionary(x => x.Key);
            _elementBlueprints = new Lazy<ILookup<int, IContent>>(() => contentService.GetBlueprintsForContentTypes(_elementTypes.Values.Select(x => x.Id).ToArray()).ToLookup(x => x.ContentTypeId));

            Fields.Add(new ContentBlocksTypesConfigurationField(_elementTypes.Values));
            Fields.Add(new ContentBlocksDisplayModeConfigurationField());
            Fields.Add(new EnableFilterConfigurationField());
            Fields.Add(new MaxItemsConfigurationField());
            Fields.Add(new DisableSortingConfigurationField());
            Fields.Add(new HideLabelConfigurationField());
            Fields.Add(new EnableDevModeConfigurationField());
        }

        public override IDictionary<string, object> ToValueEditor(object configuration)
        {
            var config = base.ToValueEditor(configuration);

            if (config.TryGetValue(ContentBlocksTypesConfigurationField.ContentBlockTypes, out var tmp) && tmp is JArray array && array.Count > 0)
            {
                var elementTypes = new List<ContentBlockType>();

                for (var i = 0; i < array.Count; i++)
                {
                    if (Guid.TryParse(array[i].Value<string>("type"), out var guid) && _elementTypes.ContainsKey(guid))
                    {
                        var elementType = _elementTypes[guid];

                        var settings = array[i]["value"].ToObject<Dictionary<string, object>>();

                        var blueprints = _elementBlueprints.Value.Contains(elementType.Id)
                            ? _elementBlueprints.Value[elementType.Id].Select(x => new ContentBlockType.BlueprintItem { Id = x.Id, Name = x.Name })
                            : Enumerable.Empty<ContentBlockType.BlueprintItem>();

                        elementTypes.Add(new ContentBlockType
                        {
                            Alias = elementType.Alias,
                            Name = elementType.Name,
                            Description = elementType.Description,
                            Icon = elementType.Icon,
                            Key = elementType.Key,
                            Blueprints = blueprints,
                            NameTemplate = settings?.ContainsKey("nameTemplate") == true ? settings["nameTemplate"].ToString() : null,
                            OverlaySize = settings?.ContainsKey("overlaySize") == true ? settings["overlaySize"].ToString() : null,
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
