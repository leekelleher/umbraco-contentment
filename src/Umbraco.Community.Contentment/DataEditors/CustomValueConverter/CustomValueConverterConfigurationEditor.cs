/* Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Collections.Generic;
using System.Linq;
#if NET472
using Umbraco.Core;
using Umbraco.Core.IO;
using Umbraco.Core.PropertyEditors;
using Umbraco.Core.Services;
using Umbraco.Core.Strings;
using UmbConstants = Umbraco.Core.Constants;
#else
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Strings;
using Umbraco.Extensions;
using UmbConstants = Umbraco.Cms.Core.Constants;
#endif

namespace Umbraco.Community.Contentment.DataEditors
{
    internal sealed class CustomValueConverterConfigurationEditor : ConfigurationEditor
    {
        internal const string DataType = "dataType";

        private readonly IDataTypeService _dataTypeService;
        private readonly PropertyEditorCollection _propertyEditors;

        public CustomValueConverterConfigurationEditor(
            IDataTypeService dataTypeService,
            PropertyEditorCollection propertyEditors,
            PropertyValueConverterCollection propertyValueConverters,
            IShortStringHelper shortStringHelper,
            IIOHelper ioHelper)
            : base()
        {
            _dataTypeService = dataTypeService;
            _propertyEditors = propertyEditors;

            Fields.Add(new ConfigurationField
            {
                Key = DataType,
                Name = "Data type",
                Description = "Select the data type to be wrapped with a value converter.",
                View = "treepicker",
                Config = new Dictionary<string, object>
                {
                    { "multiPicker", false },
                    { "entityType", "DataType" },
                    { "type", UmbConstants.Applications.Settings },
                    { "treeAlias", UmbConstants.Trees.DataTypes },
                    { "idType", "udi" },
                }
            });

            var items = propertyValueConverters.Select(x =>
            {
                var type = x.GetType();

                return new DataListItem
                {
                    Name = type.Name.SplitPascalCasing(shortStringHelper),
                    Value = type.FullName,
                    Description = type.FullName,
                };
            });

            Fields.Add(new ConfigurationField
            {
                Key = "valueConverter",
                Name = "Value converter",
                Description = "Select the value converter to use.",
                View = ioHelper.ResolveRelativeOrVirtualUrl(ItemPickerDataListEditor.DataEditorViewPath),
                Config = new Dictionary<string, object>
                {
                    { "confirmRemoval", Constants.Values.True },
                    { "defaultIcon", CustomValueConverterDataEditor.DataEditorIcon },
                    { "enableFilter", Constants.Values.True },
                    { "listType", "list" },
                    { "overlaySize", "medium" },
                    { Constants.Conventions.ConfigurationFieldAliases.Items, items },
                    { Constants.Conventions.ConfigurationFieldAliases.OverlayView, ioHelper.ResolveRelativeOrVirtualUrl(ItemPickerDataListEditor.DataEditorOverlayViewPath) },
                    { DisableSortingConfigurationField.DisableSorting, Constants.Values.True },
                    { EnableDevModeConfigurationField.EnableDevMode, Constants.Values.True },
                    { MaxItemsConfigurationField.MaxItems, 1 },
                }
            });
        }

        public override IDictionary<string, object> ToValueEditor(object configuration)
        {
            var config = base.ToValueEditor(configuration);

            if (config.TryGetValueAs(DataType, out GuidUdi udi) == true)
            {
                var dataType = _dataTypeService.GetDataType(udi.Guid);
                if (dataType != null && _propertyEditors.TryGet(dataType.EditorAlias, out var dataEditor) == true)
                {
                    return dataEditor.GetConfigurationEditor().ToValueEditor(dataType.Configuration);
                }
            }

            return base.ToValueEditor(configuration);
        }
    }
}
