/* Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Collections.Generic;
using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Extensions;

namespace Umbraco.Community.Contentment.DataEditors
{
    internal sealed class NotesConfigurationEditor : ConfigurationEditor
    {
        internal const string Notes = NotesConfigurationField.Notes;

        public NotesConfigurationEditor()
            : base()
        {
            Fields.Add(new ConfigurationField
            {
                Key = Notes,
                Name = nameof(Notes),
                Description = "Enter the notes to be displayed for the content editor.",
                View = IOHelper.ResolveUrl("~/umbraco/views/propertyeditors/rte/rte.html"),
                Config = new Dictionary<string, object>
                {
                    { "editor", Constants.Conventions.DefaultConfiguration.RichTextEditor }
                }
            });

            Fields.Add(new HideLabelConfigurationField());
        }
    }
}
