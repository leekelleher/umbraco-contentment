/* Copyright © 2019 Lee Kelleher, Umbrella Inc and other contributors.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using Umbraco.Core.IO;
using Umbraco.Core.PropertyEditors;

namespace Our.Umbraco.Contentment.DataEditors
{
    public class NotesConfigurationEditor : ConfigurationEditor
    {
        public NotesConfigurationEditor()
            : base()
        {
            var editor = new
            {
                maxImageSize = 500,
                mode = "classic",
                stylesheets = Array.Empty<string>(),
                toolbar = new[]
                {
                    "ace",
                    "undo",
                    "redo",
                    "cut",
                    //"styleselect", // TODO: We could write up a custom stylesheet to populate the Formats menu with Headings, etc? [LK]
                    "bold",
                    "italic",
                    "alignleft",
                    "aligncenter",
                    "alignright",
                    "bullist",
                    "numlist",
                    "link",
                    "umbmediapicker",
                    "umbmacro",
                    "umbembeddialog"
                },
            };

            Fields.Add(
                "notes",
                "Notes",
                "Enter the notes to be displayed for the content editor.",
                 IOHelper.ResolveUrl("~/umbraco/views/propertyeditors/rte/rte.html"),
                new Dictionary<string, object> { { "editor", editor } });
            Fields.AddHideLabel();
        }
    }
}
