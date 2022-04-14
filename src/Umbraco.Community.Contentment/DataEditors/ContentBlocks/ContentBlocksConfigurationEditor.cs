/* Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
#if NET472
using Umbraco.Core;
using Umbraco.Core.IO;
using Umbraco.Core.Models;
using Umbraco.Core.PropertyEditors;
using Umbraco.Core.Services;
using Umbraco.Core.Strings;
using UmbConstants = Umbraco.Core.Constants;
#else
using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Strings;
using Umbraco.Extensions;
using UmbConstants = Umbraco.Cms.Core.Constants;
#endif

namespace Umbraco.Community.Contentment.DataEditors
{
    internal sealed class ContentBlocksConfigurationEditor : ConfigurationEditor
    {
        // TODO: [LK:2021-08-16] expire the local cache `_elementTypes` when a new element type is added.
        private readonly Dictionary<Guid, IContentType> _elementTypes;
        private readonly Lazy<ILookup<int, IContent>> _elementBlueprints;
        private readonly IIOHelper _ioHelper;
        private readonly ConfigurationEditorUtility _utility;

        internal const string DisplayMode = "displayMode";
        internal const string ReusableContent = "reusableContent";

        public ContentBlocksConfigurationEditor(
            IContentService contentService,
            IContentTypeService contentTypeService,
            ConfigurationEditorUtility utility,
            IShortStringHelper shortStringHelper,
            IIOHelper ioHelper)
            : base()
        {
            _ioHelper = ioHelper;
            _utility = utility;

            // NOTE: Gets all the elementTypes and blueprints upfront, rather than several hits inside the loop.
            _elementTypes = contentTypeService.GetAllElementTypes().ToDictionary(x => x.Key);
            _elementBlueprints = new Lazy<ILookup<int, IContent>>(() => contentService.GetBlueprintsForContentTypes(_elementTypes.Values.Select(x => x.Id).ToArray()).ToLookup(x => x.ContentTypeId));

            var displayModes = utility.GetConfigurationEditorModels<IContentBlocksDisplayMode>(shortStringHelper);

            // NOTE: Sets the default display mode to be the Blocks.
            var defaultDisplayMode = displayModes.FirstOrDefault(x => x.Key.InvariantEquals(typeof(BlocksDisplayMode).GetFullNameWithAssembly()));
            if (defaultDisplayMode != null)
            {
                DefaultConfiguration.Add(DisplayMode, new[] { new { key = defaultDisplayMode.Key, value = defaultDisplayMode.DefaultValues } });
            }

            Fields.Add(new ConfigurationField
            {
                Key = DisplayMode,
                Name = "Display mode",
                Description = "Select and configure how to display the blocks in the editor.",
                View = ioHelper.ResolveRelativeOrVirtualUrl(ConfigurationEditorDataEditor.DataEditorViewPath),
                Config = new Dictionary<string, object>()
                {
                    { Constants.Conventions.ConfigurationFieldAliases.AddButtonLabelKey, "contentment_configureDisplayMode" },
                    { Constants.Conventions.ConfigurationFieldAliases.Items, displayModes },
                    { MaxItemsConfigurationField.MaxItems, 1 },
                    { DisableSortingConfigurationField.DisableSorting, Constants.Values.True },
                    { Constants.Conventions.ConfigurationFieldAliases.OverlayView, ioHelper.ResolveRelativeOrVirtualUrl(ConfigurationEditorDataEditor.DataEditorOverlayViewPath) },
                    { EnableDevModeConfigurationField.EnableDevMode, Constants.Values.True },
                }
            });

            Fields.Add(new ContentBlocksTypesConfigurationField(_elementTypes.Values, ioHelper));

            var reusableContent = new[]
            {
                new ConfigurationEditorModel
                {
                    Key = "reusableContent",
                    Name = "Reusable Content",
                    NameTemplate = "{{name}}",
                    Description = default,
                    DescriptionTemplate = "{{description}}",
                    Icon = UmbConstants.Icons.ContentType,
                    IconTemplate = "{{icon}}",
                    DefaultValues = new Dictionary<string, object>
                    {
                        { "icon", UmbConstants.Icons.ContentType },
                        { "nameTemplate", "{{ udi | ncNodeName }}" },
                        { "overlaySize", "small" },
                        { "enablePreview", Constants.Values.False },
                    },
                    Fields = new[]
                    {
                        new ConfigurationField
                        {
                            Key = "name",
                            Name = "Name",
                            Description = "",
                            View = "textstring",
                        },
                        new ConfigurationField
                        {
                            Key = "description",
                            Name = "Description",
                            Description = "",
                            View = "textstring",
                        },
                        new ConfigurationField
                        {
                            Key = "icon",
                            Name = "Icon",
                            Description = "",
                            View = ioHelper.ResolveRelativeOrVirtualUrl("~/umbraco/views/propertyeditors/listview/icon.prevalues.html"),
                        },
                        new ConfigurationField
                        {
                            Key = "parentNode",
                            Name = "Parent node",
                            Description = "Set a parent node to use its child nodes as reusable content.",
                            View =  ioHelper.ResolveRelativeOrVirtualUrl(ContentPickerDataEditor.DataEditorSourceViewPath),
                        },
                        new ConfigurationField
                        {
                            Key = "nameTemplate",
                            Name = "Name template",
                            Description = "Enter an AngularJS expression to evaluate against each block for its name.",
                            View = "textstring",
                        },
                        new ConfigurationField
                        {
                            Key = "overlaySize",
                            Name = "Editor overlay size",
                            Description = "Select the size of the overlay editing panel. By default this is set to 'small'. However if the editor fields require a wider panel, please select 'medium' or 'large'.",
                            View = _ioHelper.ResolveRelativeOrVirtualUrl(RadioButtonListDataListEditor.DataEditorViewPath),
                            Config = new Dictionary<string, object>
                            {
                                { Constants.Conventions.ConfigurationFieldAliases.Items, new[]
                                    {
                                        new DataListItem { Name = "Small", Value = "small" },
                                        new DataListItem { Name = "Medium", Value = "medium" },
                                        new DataListItem { Name = "Large", Value = "large" }
                                    }
                                }
                            }
                        },
                        new ConfigurationField
                        {
                            Key = "enablePreview",
                            Name = "Enable preview?",
                            Description = "Select to enable a rich preview for this content block type.",
                            View = "views/propertyeditors/boolean/boolean.html"
                        }
                    },
                    OverlaySize = OverlaySize.Medium,
                }
            };

            Fields.Add(new ConfigurationField
            {
                Key = ReusableContent,
                Name = "Reusable content",
                Description = "<em>(optional)</em> Configure library repositories to reuse content pages as blocks.",
                View = ioHelper.ResolveRelativeOrVirtualUrl(ConfigurationEditorDataEditor.DataEditorViewPath),
                Config = new Dictionary<string, object>
                {
                    { Constants.Conventions.ConfigurationFieldAliases.AddButtonLabelKey, "defaultdialogs_selectContentStartNode" },
                    { "allowDuplicates", Constants.Values.True },
                    { Constants.Conventions.ConfigurationFieldAliases.Items, reusableContent },
                    { Constants.Conventions.ConfigurationFieldAliases.OverlayView, ioHelper.ResolveRelativeOrVirtualUrl(ConfigurationEditorDataEditor.DataEditorOverlayViewPath) },
                    { EnableDevModeConfigurationField.EnableDevMode, Constants.Values.True },
                }
            });

            Fields.Add(new EnableFilterConfigurationField());
            Fields.Add(new MaxItemsConfigurationField(ioHelper));
            Fields.Add(new DisableSortingConfigurationField());
            Fields.Add(new EnableDevModeConfigurationField());
        }

        public override IDictionary<string, object> ToConfigurationEditor(object configuration)
        {
            var config = base.ToConfigurationEditor(configuration);

            // NOTE: [LK] Technical debt. This works around the original display mode data just being the view-path (string).
            // This was prior to v1.1.0 release, (when Content Blocks was introduced). It could be removed in the next major version.
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
                    // NOTE: Removing the raw configuration as the display mode may have the same key.
                    config.Remove(DisplayMode);

                    var editorConfig = item1["value"].ToObject<Dictionary<string, object>>();
                    if (editorConfig != null)
                    {
                        foreach (var prop in editorConfig)
                        {
                            if (config.ContainsKey(prop.Key) == false)
                            {
                                config.Add(prop.Key, prop.Value);
                            }
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

            if (config.TryGetValueAs(ReusableContent, out JArray array3) == true && array3.Count > 0)
            {
                var items3 = new List<JToken>();

                for (var i = 0; i < array3.Count; i++)
                {
                    items3.Add(array3[i]["value"]);
                }

                config[ReusableContent] = items3;
            }

            if (config.ContainsKey(Constants.Conventions.ConfigurationFieldAliases.OverlayView) == false)
            {
                config.Add(Constants.Conventions.ConfigurationFieldAliases.OverlayView, _ioHelper.ResolveRelativeOrVirtualUrl(ContentBlocksDataEditor.DataEditorOverlayViewPath));
            }

            return config;
        }
    }
}
