/* Copyright © 2019 Lee Kelleher, Umbrella Inc and other contributors.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using Umbraco.Core;
using Umbraco.Core.IO;
using Umbraco.Core.PropertyEditors;
using Umbraco.Core.Services;
using UmbracoIcons = Umbraco.Core.Constants.Icons;

namespace Umbraco.Community.Contentment.DataEditors
{
    public class MacroPickerConfigurationEditor : ConfigurationEditor
    {
        public const string AllowedMacros = "allowedMacros";

        private readonly IMacroService _macroService;

        public MacroPickerConfigurationEditor(IMacroService macroService)
            : base()
        {
            _macroService = macroService;

            var macros = macroService
                .GetAll()
                .Select(x => new DataListItem
                {
                    Icon = UmbracoIcons.Macro,
                    Name = x.Name,
                    Description = x.Alias,
                    Value = x.GetUdi().ToString()
                });

            Fields.Add(
                AllowedMacros,
                "Allowed macros",
                "Restrict the macros that can be picked.",
                IOHelper.ResolveUrl(ItemPickerDataEditor.DataEditorViewPath),
                new Dictionary<string, object>
                {
                    { AllowDuplicatesConfigurationField.AllowDuplicates, Constants.Values.False },
                    { ItemPickerConfigurationEditor.Items, macros },
                    { ItemPickerTypeConfigurationField.ListType, ItemPickerTypeConfigurationField.List },
                    { ItemPickerConfigurationEditor.OverlayView, IOHelper.ResolveUrl(ItemPickerDataEditor.DataEditorOverlayViewPath) },
                    { ItemPickerConfigurationEditor.EnableDevMode, Constants.Values.False },
                });

            Fields.AddMaxItems();
            Fields.AddDisableSorting();
            Fields.AddHideLabel();
        }

        public override IDictionary<string, object> ToValueEditor(object configuration)
        {
            var config = base.ToValueEditor(configuration);

            if (config.TryGetValue(AllowedMacros, out var tmp1) && tmp1 is JArray array)
            {
                var ids = new List<Guid>();
                foreach (var token in array)
                {
                    if (GuidUdi.TryParse(token.Value<string>(), out var udi))
                    {
                        ids.Add(udi.Guid);
                    }
                }

                // TODO: [LK:2019-07-19] Once the MacroService bug patch is available, we can uncomment the line below.
                // Issue: https://github.com/umbraco/Umbraco-CMS/issues/5956 // PR: https://github.com/umbraco/Umbraco-CMS/pull/5962 by @kjac
                //config.Add("availableMacros", _macroService.GetAll(ids).Select(x => x.Alias));
                config.Add("availableMacros", _macroService.GetAll().Where(x => ids.Contains(x.Key)).Select(x => x.Alias));
                config.Remove(AllowedMacros);
            }

            return config;
        }
    }
}
