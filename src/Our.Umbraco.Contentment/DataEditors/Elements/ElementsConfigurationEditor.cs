/* Copyright © 2019 Lee Kelleher, Umbrella Inc and other contributors.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Collections.Generic;
using System.Linq;
using Umbraco.Core;
using Umbraco.Core.Composing;
using Umbraco.Core.IO;
using Umbraco.Core.PropertyEditors;

namespace Our.Umbraco.Contentment.DataEditors
{
    public class ElementsConfigurationEditor : ConfigurationEditor
    {
        public const string Items = "items";
        public const string OverlayView = "overlayView";

        public ElementsConfigurationEditor()
            : base()
        {
            var defaultFields = new List<ConfigurationField>
            {
                new ConfigurationField
                {
                    Key = "displayName",
                    Name = "Display name",
                    Description= "Enter a friendly name for this configuration.",
                    View = "textstring"
                },
                new ConfigurationField
                {
                    Key = "nameTemplate",
                    Name = "Name template",
                    Description= "Enter an AngularJs expression to evaluate against each item for its name.",
                    View = "textstring"
                },
                new ConfigurationField
                {
                    Key = "enablePreview",
                    Name = "Enable preview?",
                    View = "boolean"
                }
            };

            // TODO: [LK:2019-07-01] I'm not completely happy with this approach.
            // I'd like to be able to select a doctype, then override it's name/description/icon.
            // The we could have multiple instances of a doctype.
            // Or is that overkill? Should we work like Umbraco, just use the exact names/icons? Don't over-complicate it.

            var items = Current.Services.ContentTypeService
                .GetAll()
                .Where(x => x.IsElement)
                .OrderBy(x => x.Name)
                .Select(x => new ConfigurationEditorModel
                {
                    Description = x.Description,
                    Icon = x.Icon,
                    Name = x.Name,
                    Type = x.GetUdi().ToString(),
                    Fields = defaultFields,
                    DefaultValues = new Dictionary<string, object>
                    {
                        { "displayName", x.Name },
                        { "nameTemplate", $"{x.Name} #{{{{$index + 1}}}}" },
                    },
                    NameTemplate = "{{displayName || name}}",
                });

            Fields.Add(
                Items,
                nameof(Items),
                "Select the element types to use.",
                IOHelper.ResolveUrl(ConfigurationEditorDataEditor.DataEditorViewPath),
                new Dictionary<string, object>
                {
                    { AllowDuplicatesConfigurationField.AllowDuplicates, Constants.Values.False },
                    { ConfigurationEditorConfigurationEditor.Items, items },
                    { ConfigurationEditorConfigurationEditor.OverlaySize, ConfigurationEditorConfigurationEditor.OverlaySizeConfigurationField.Large },
                    { ConfigurationEditorConfigurationEditor.OverlayView, IOHelper.ResolveUrl(ConfigurationEditorDataEditor.DataEditorOverlayViewPath) },
                    { ConfigurationEditorConfigurationEditor.EnableFilter, Constants.Values.True },
                    { ConfigurationEditorConfigurationEditor.EnableDevMode, Constants.Values.False },
                });

            Fields.AddMaxItems();
            Fields.AddDisableSorting();
            Fields.AddHideLabel();
        }

        public override IDictionary<string, object> ToValueEditor(object configuration)
        {
            var config = base.ToValueEditor(configuration);

            if (config.ContainsKey(OverlayView) == false)
            {
                config.Add(OverlayView, IOHelper.ResolveUrl(ElementsDataEditor.DataEditorOverlayViewPath));
            }

            return config;
        }
    }
}
