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
                "Enter the notes for the content editor.", // TODO: Make description friendlier [LK]
                 IOHelper.ResolveUrl("~/umbraco/views/propertyeditors/rte/rte.html"),
                new Dictionary<string, object> { { "editor", editor } });
            Fields.AddHideLabel();
        }
    }
}
