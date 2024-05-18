/* Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Extensions;

namespace Umbraco.Community.Contentment.DataEditors
{
    internal class NotesConfigurationField : ConfigurationField
    {
        internal const string Notes = "notes";

        public NotesConfigurationField(IIOHelper ioHelper, string notes, bool hideLabel = true)
            : base()
        {
            Key = Notes;
            Name = nameof(Notes);
            View = ioHelper.ResolveRelativeOrVirtualUrl(NotesDataEditor.DataEditorViewPath);
            Config = new Dictionary<string, object> { { Notes, notes } };
            HideLabel = hideLabel;
        }
    }
}
