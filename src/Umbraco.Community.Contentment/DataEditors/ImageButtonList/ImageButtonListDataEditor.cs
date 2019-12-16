/* Copyright © 2019 Lee Kelleher and Marc Stöcker.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Umbraco.Core.Logging;
using Umbraco.Core.PropertyEditors;

namespace Umbraco.Community.Contentment.DataEditors
{
    [DataEditor(
        DataEditorAlias,
        EditorType.PropertyValue,
        DataEditorName,
        DataEditorViewPath,
        ValueType = ValueTypes.String,
        Group = Core.Constants.PropertyEditors.Groups.Lists,
#if DEBUG
        Icon = "icon-block color-red"
#else
        Icon = DataEditorIcon
#endif
        )]
    internal sealed class ImageButtonListDataEditor : DataEditor
    {
        internal const string DataEditorAlias = Constants.Internals.DataEditorAliasPrefix + "ImageButtonList";
        internal const string DataEditorName = Constants.Internals.DataEditorNamePrefix + "Image Button List";
        internal const string DataEditorViewPath = Constants.Internals.EditorsPathRoot + "image-button-list.html";
        internal const string DataEditorIcon = "icon-columns"; //icon-pictures-alt

        public ImageButtonListDataEditor(ILogger logger)
            : base(logger)
        { }

        protected override IConfigurationEditor CreateConfigurationEditor() => new ImageButtonListConfigurationEditor();
    }
}
