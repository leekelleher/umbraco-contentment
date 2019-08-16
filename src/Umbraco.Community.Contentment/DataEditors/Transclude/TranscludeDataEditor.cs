/* Copyright © 2019 Lee Kelleher, Umbrella Inc and other contributors.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Umbraco.Core.Logging;
using Umbraco.Core.PropertyEditors;

namespace Umbraco.Community.Contentment.DataEditors
{
    [DataEditor(
        DataEditorAlias,
#if DEBUG
        EditorType.PropertyValue, // NOTE: IsWorkInProgress [LK]
#else
        EditorType.Nothing,
#endif
        DataEditorName,
        DataEditorViewPath,
        ValueType = ValueTypes.String,
        Group = Constants.Conventions.PropertyGroups.Common,
        Icon = DataEditorIcon)]
    public class TranscludeDataEditor : DataEditor
    {
        internal const string DataEditorAlias = Constants.Internals.DataEditorAliasPrefix + "Transclude";
        internal const string DataEditorName = Constants.Internals.DataEditorNamePrefix + "Transclude";
        internal const string DataEditorViewPath = Constants.Internals.EditorsPathRoot + "transclude.html";
        internal const string DataEditorIcon = "icon-globe-alt";

        public TranscludeDataEditor(ILogger logger)
            : base(logger)
        { }

        // TODO: [LK:2019-07-09] Consider enhanced UI for this editor.
        // Marc did one for RSS feeds... https://our.umbraco.com/packages/backoffice-extensions/rss-feed-url/
        // It could handle RSS feeds, Markdown transformations, XML/XSLT?
        // or just return the string contents and let the frontend implementor deal with it?

        protected override IDataValueEditor CreateValueEditor() => new HideLabelDataValueEditor(Attribute);
    }
}
