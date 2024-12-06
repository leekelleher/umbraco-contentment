/* Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Umbraco.Cms.Core.PropertyEditors;

namespace Umbraco.Community.Contentment.DataEditors
{
    internal class NotesConfigurationField : ContentmentConfigurationField
    {
        internal const string Notes = "notes";

        public NotesConfigurationField(string notes, bool hideLabel = true)
        {
            Key = Notes;
            Name = nameof(Notes);
            PropertyEditorUiAlias = "Umb.Contentment.PropertyEditorUi.Notes";
            Config = new Dictionary<string, object> { { Notes, notes }, { nameof(hideLabel), hideLabel } };
        }
    }
}
