/* Copyright © 2021 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Umbraco.Cms.Core;
using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Services;
using Umbraco.Extensions;

namespace Umbraco.Community.Contentment.DataEditors
{
    public sealed class UmbracoImageCropDataListSource : DataListToDataPickerSourceBridge, IDataListSource
    {
        private readonly IDataTypeService _dataTypeService;
        private readonly IIOHelper _ioHelper;

        public UmbracoImageCropDataListSource(
            IDataTypeService dataTypeService,
            IIOHelper ioHelper)
        {
            _dataTypeService = dataTypeService;
            _ioHelper = ioHelper;
        }

        public override string Name => "Umbraco Image Crops";

        public override string Description => "Select an Image Cropper data-type to use its defined crops to populate the data source.";

        public override string Icon => "icon-crop";

        public override string Group => Constants.Conventions.DataSourceGroups.Umbraco;

        public override IEnumerable<ConfigurationField> Fields
        {
            get
            {
                var items = _dataTypeService
                    .GetByEditorAlias(UmbConstants.PropertyEditors.Aliases.ImageCropper)
                    .Select(x => new DataListItem
                    {
                        Icon = Icon,
                        Name = x.Name ?? x.EditorAlias,
                        Value = Udi.Create(UmbConstants.UdiEntityType.DataType, x.Key).ToString(),
                    });

                return new ConfigurationField[]
                {
                    new ConfigurationField
                    {
                        Key = "imageCropper",
                        Name = "Image Cropper",
                        Description = "Select a Data Type that uses the Image Cropper.",
                        View = _ioHelper.ResolveRelativeOrVirtualUrl(RadioButtonListDataListEditor.DataEditorViewPath),
                        Config = new Dictionary<string, object>
                        {
                            { Constants.Conventions.ConfigurationFieldAliases.Items, items },
                            { ShowIconsConfigurationField.ShowIcons, Constants.Values.True },
                            { Constants.Conventions.ConfigurationFieldAliases.DefaultValue, items.FirstOrDefault()?.Value ?? string.Empty }
                        }
                    }
                };
            }
        }

        public override Dictionary<string, object>? DefaultValues => default;

        public override OverlaySize OverlaySize => OverlaySize.Small;

        public override IEnumerable<DataListItem> GetItems(Dictionary<string, object> config)
        {
            if (config.TryGetValue("imageCropper", out var obj) == true &&
                obj is string str &&
                string.IsNullOrWhiteSpace(str) == false &&
                UdiParser.TryParse(str, out GuidUdi? udi) == true &&
                udi is not null)
            {
                return _dataTypeService
                    .GetDataType(udi.Guid)?
                    .ConfigurationAs<ImageCropperConfiguration>()?
                    .Crops?
                    .Select(x => new DataListItem
                    {
                        Name = x.Alias,
                        Value = x.Alias,
                        Icon = this.Icon,
                        Description = $"{x.Width}px × {x.Height}px"
                    }) ?? Enumerable.Empty<DataListItem>();
            }

            return Enumerable.Empty<DataListItem>();
        }
    }
}
