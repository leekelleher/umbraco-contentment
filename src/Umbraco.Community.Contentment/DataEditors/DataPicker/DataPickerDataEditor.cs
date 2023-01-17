/* Copyright © 2023 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Collections.Generic;
#if NET472
using Umbraco.Core.IO;
using Umbraco.Core.Logging;
using Umbraco.Core.PropertyEditors;
using Umbraco.Core.Strings;
using UmbConstants = Umbraco.Core.Constants;
#else
using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Strings;
using Umbraco.Extensions;
using UmbConstants = Umbraco.Cms.Core.Constants;
#endif
namespace Umbraco.Community.Contentment.DataEditors
{
    [DataEditor(
        DataEditorAlias,
        EditorType.PropertyValue,
        DataEditorName,
        DataEditorViewPath,
        ValueType = ValueTypes.Json,
        Group = UmbConstants.PropertyEditors.Groups.Lists,
        Icon = DataEditorIcon)]
    internal sealed class DataPickerDataEditor : DataEditor
    {
        internal const string DataEditorAlias = Constants.Internals.DataEditorAliasPrefix + "DataPicker";
        internal const string DataEditorName = Constants.Internals.DataEditorNamePrefix + "Data Picker";
        internal const string DataEditorViewPath = Constants.Internals.EditorsPathRoot + "data-picker.html";
        internal const string DataEditorOverlayViewPath = Constants.Internals.EditorsPathRoot + "data-picker.overlay.html";
        internal const string DataEditorIcon = "icon-fa fa-mouse-pointer";

        private readonly IIOHelper _ioHelper;
        private readonly IShortStringHelper _shortStringHelper;
        private readonly ConfigurationEditorUtility _utility;

#if NET472
        public DataPickerDataEditor(
            IIOHelper ioHelper,
            IShortStringHelper shortStringHelper,
            ConfigurationEditorUtility utility,
            ILogger logger)
            : base(logger)
        {
            _ioHelper = ioHelper;
            _shortStringHelper = shortStringHelper;
            _utility = utility;
        }
#else
        public DataPickerDataEditor(
            IDataValueEditorFactory dataValueEditorFactory,
            IIOHelper ioHelper,
            IShortStringHelper shortStringHelper,
            ConfigurationEditorUtility utility)
            : base(dataValueEditorFactory)
        {
            _ioHelper = ioHelper;
            _shortStringHelper = shortStringHelper;
            _utility = utility;
        }
#endif
        protected override IConfigurationEditor CreateConfigurationEditor() => new DataPickerConfigurationEditor(_ioHelper, _shortStringHelper, _utility);

        public override IDataValueEditor GetValueEditor(object configuration)
        {
            if (configuration is Dictionary<string, object> config)
            {
                config.Add("overlayView", _ioHelper.ResolveRelativeOrVirtualUrl(DataEditorOverlayViewPath) ?? string.Empty);
            }

            return base.GetValueEditor(configuration);
        }
    }
}
