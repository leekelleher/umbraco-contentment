/* Copyright © 2022 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Extensions;

namespace Umbraco.Community.Contentment.DataEditors
{
    internal sealed class EditorNotesConfigurationEditor : ConfigurationEditor
    {
        internal const string Message = "message";

        public EditorNotesConfigurationEditor(IIOHelper ioHelper)
            : base()
        {
            var format = "<div class=\"umb-editor--small {0}\"><strong>{1}</strong><br>{2}</div>";
            var items = new[]
            {
                new { name = string.Format(format, "alert alert-form","Form","A message, displaying on a white background with a dark border."), value = "alert alert-form" },
                new { name = string.Format(format, "alert alert-info", "Info", "An informational message, displaying on a blue background."), value = "alert alert-info" },
                new { name = string.Format(format, "alert alert-success", "Success", "A success message, displaying on a green background."), value = "alert alert-success" },
                new { name = string.Format(format, "alert alert-warning", "Warning", "A warning message, displaying on an orange background."), value = "alert alert-warning" },
                new { name = string.Format(format, "alert alert-error", "Error", "An error message, displaying on a red background."), value = "alert alert-error" },
                new { name = string.Format(format, "well", "Well", "An informational message, displaying inset on a gray background."), value = "well" },
            };

            //Fields.Add(new ContentmentConfigurationField
            //{
            //    Key = "alertType",
            //    Name = "Alert type",
            //    Description = string.Empty,
            //    View = ioHelper.ResolveRelativeOrVirtualUrl(RadioButtonListDataListEditor.DataEditorViewPath),
            //    Config = new Dictionary<string, object>
            //    {
            //        { "defaultValue", "alert alert-warning" },
            //        { "items", items },
            //    }
            //});

            //Fields.Add(new ContentmentConfigurationField
            //{
            //    Key = "icon",
            //    Name = "Icon",
            //    Description = "Select an icon to be displayed next to the message.",
            //    View = ioHelper.ResolveRelativeOrVirtualUrl(IconPickerDataEditor.DataEditorViewPath),
            //    Config = new Dictionary<string, object>
            //    {
            //        { "size", "small" },
            //    }
            //});

            //Fields.Add(new ContentmentConfigurationField
            //{
            //    Key = "heading",
            //    Name = "Heading",
            //    Description = string.Empty,
            //    View = "textstring"
            //});

            //Fields.Add(new ContentmentConfigurationField
            //{
            //    Key = Message,
            //    Name = nameof(Message),
            //    Description = "Enter the notes to be displayed for the content editor.",
            //    View = ioHelper.ResolveRelativeOrVirtualUrl(RichTextEditorDataEditor.DataEditorViewPath),
            //    Config = new Dictionary<string, object>
            //    {
            //        { "editor", Constants.Conventions.DefaultConfiguration.RichTextEditor }
            //    }
            //});

            //Fields.Add(new HideLabelConfigurationField());
            //Fields.Add(new HidePropertyGroupConfigurationField());
        }
    }
}
