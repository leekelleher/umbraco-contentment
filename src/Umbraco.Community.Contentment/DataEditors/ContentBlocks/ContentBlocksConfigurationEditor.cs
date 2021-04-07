/* Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
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
    internal sealed class ContentBlocksConfigurationEditor : ConfigurationEditor
    {
        private readonly Dictionary<Guid, IContentType> _elementTypes;
        private readonly Lazy<ILookup<int, IContent>> _elementBlueprints;
        private readonly ConfigurationEditorUtility _utility;

        internal const string DisplayMode = "displayMode";

        public ContentBlocksConfigurationEditor(
            IContentService contentService,
            IContentTypeService contentTypeService,
            ConfigurationEditorUtility utility)
            : base()
        {
            _utility = utility;

            // NOTE: Gets all the elementTypes and blueprints upfront, rather than several hits inside the loop.
            _elementTypes = contentTypeService.GetAllElementTypes().ToDictionary(x => x.Key);
            _elementBlueprints = new Lazy<ILookup<int, IContent>>(() => contentService.GetBlueprintsForContentTypes(_elementTypes.Values.Select(x => x.Id).ToArray()).ToLookup(x => x.ContentTypeId));

            var displayModes = utility.GetConfigurationEditorModels<IContentBlocksDisplayMode>();

            // NOTE: Sets the default display mode to be the Stack.
            var defaultDisplayMode = displayModes.FirstOrDefault(x => x.Key.InvariantEquals(typeof(StackDisplayMode).GetFullNameWithAssembly()));
            if (defaultDisplayMode != null)
            {
                DefaultConfiguration.Add(DisplayMode, new[] { new { key = defaultDisplayMode.Key, value = defaultDisplayMode.DefaultValues } });
            }

            Fields.Add(
                DisplayMode,
                "Display mode",
                "Select and configure how to display the blocks in the editor.",
                IOHelper.ResolveUrl(ConfigurationEditorDataEditor.DataEditorViewPath),
                new Dictionary<string, object>()
                {
                    { Constants.Conventions.ConfigurationFieldAliases.AddButtonLabelKey, "contentment_configureDisplayMode" },
                    { Constants.Conventions.ConfigurationFieldAliases.Items, displayModes },
                    { MaxItemsConfigurationField.MaxItems, 1 },
                    { DisableSortingConfigurationField.DisableSorting, Constants.Values.True },
                    { Constants.Conventions.ConfigurationFieldAliases.OverlayView, IOHelper.ResolveUrl(ConfigurationEditorDataEditor.DataEditorOverlayViewPath) },
                    { EnableDevModeConfigurationField.EnableDevMode, Constants.Values.True },
                });

            Fields.Add(new ContentBlocksTypesConfigurationField(_elementTypes.Values));
            Fields.Add(new EnableFilterConfigurationField());
            Fields.Add(new MaxItemsConfigurationField());
            Fields.Add(new DisableSortingConfigurationField());
            Fields.Add(new EnableDevModeConfigurationField());
        }

        public override IDictionary<string, object> ToConfigurationEditor(object configuration)
        {
            var config = base.ToConfigurationEditor(configuration);

            if (config.TryGetValueAs(DisplayMode, out string str1) == true && str1?.InvariantStartsWith(Constants.Internals.EditorsPathRoot) == true)
            {
                var mode = _utility.FindConfigurationEditor<IContentBlocksDisplayMode>(x => str1.InvariantEquals(x.View) == true);
                if (mode != null)
                {
                    config[DisplayMode] = new
                    {
                        key = mode.GetType().GetFullNameWithAssembly(),
                        value = mode.DefaultConfig
                    };
                }
            }

            return config;
        }

        public override IDictionary<string, object> ToValueEditor(object configuration)
        {
            var config = base.ToValueEditor(configuration);

            if (config.TryGetValueAs(DisplayMode, out JArray array1) == true && array1.Count > 0 && array1[0] is JObject item1)
            {
                var displayMode = _utility.GetConfigurationEditor<IContentBlocksDisplayMode>(item1.Value<string>("key"));
                if (displayMode != null)
                {
                    var editorConfig = item1["value"].ToObject<Dictionary<string, object>>();

                    foreach (var prop in editorConfig)
                    {
                        if (config.ContainsKey(prop.Key) == false)
                        {
                            config.Add(prop.Key, prop.Value);
                        }
                    }

                    if (displayMode.DefaultConfig != null)
                    {
                        foreach (var prop in displayMode.DefaultConfig)
                        {
                            if (config.ContainsKey(prop.Key) == false)
                            {
                                config.Add(prop.Key, prop.Value);
                            }
                        }
                    }
                }
            }

            if (config.TryGetValueAs(ContentBlocksTypesConfigurationField.ContentBlockTypes, out JArray array2) && array2.Count > 0)
            {
                var elementTypes = new List<ContentBlockType>();

                for (var i = 0; i < array2.Count; i++)
                {
                    var item = (JObject)array2[i];

                    // NOTE: Patches a breaking-change. I'd renamed `type` to become `key`. [LK:2020-04-03]
                    if (item.ContainsKey("key") == false && item.ContainsKey("type") == true)
                    {
                        item.Add("key", item["type"]);
                        item.Remove("type");
                    }

                    if (Guid.TryParse(item.Value<string>("key"), out var guid) && _elementTypes.ContainsKey(guid) == true)
                    {
                        var elementType = _elementTypes[guid];

                        var settings = item["value"].ToObject<Dictionary<string, object>>();

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
                            PreviewEnabled = settings?.ContainsKey("enablePreview") == true && settings["enablePreview"].TryConvertTo<bool>().ResultOr(false),
                        });
                    }
                }

                config[ContentBlocksTypesConfigurationField.ContentBlockTypes] = elementTypes;
            }

            if (config.ContainsKey(Constants.Conventions.ConfigurationFieldAliases.OverlayView) == false)
            {
                config.Add(Constants.Conventions.ConfigurationFieldAliases.OverlayView, IOHelper.ResolveUrl(ContentBlocksDataEditor.DataEditorOverlayViewPath));
            }

            return config;
        }
    }
}
