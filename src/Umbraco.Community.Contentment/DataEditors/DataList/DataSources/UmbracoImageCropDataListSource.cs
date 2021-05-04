/* Copyright © 2021 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Collections.Generic;
using System.Linq;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Services;
using Umbraco.Extensions;
using UmbConstants = Umbraco.Cms.Core.Constants;

namespace Umbraco.Community.Contentment.DataEditors
{
    [Cms.Core.Composing.HideFromTypeFinder]
    public sealed class UmbracoImageCropDataListSource : IDataListSource
    {
        private readonly IDataTypeService _dataTypeService;

        public UmbracoImageCropDataListSource(IDataTypeService dataTypeService)
        {
            _dataTypeService = dataTypeService;
        }

        public string Name => "Umbraco Image Crops";

        public string Description => "Select an Umbraco Image Cropper to populate the data source.";

        public string Icon => "icon-crop";

        public string Group => default;

        public IEnumerable<ConfigurationField> Fields
        {
            get
            {
                var items = _dataTypeService
                    .GetByEditorAlias(UmbConstants.PropertyEditors.Aliases.ImageCropper)
                    .Select(x => new DataListItem
                    {
                        Icon = Icon,
                        Name = x.Name,
                        Value = Udi.Create(UmbConstants.UdiEntityType.DataType, x.Key).ToString(),
                    });

                return new ConfigurationField[]
                {
                    new ConfigurationField
                    {
                        Key = "imageCropper",
                        Name = "Image Cropper",
                        Description = "Select a Data Type that uses the Image Cropper.",
                        View = RadioButtonListDataListEditor.DataEditorViewPath,
                        Config = new Dictionary<string, object>
                        {
                            { Constants.Conventions.ConfigurationFieldAliases.Items, items },
                            { ShowIconsConfigurationField.ShowIcons, Constants.Values.True },
                            { Constants.Conventions.ConfigurationFieldAliases.DefaultValue, items.FirstOrDefault()?.Value }
                        }
                    }
                };
            }
        }

        public Dictionary<string, object> DefaultValues => default;

        public OverlaySize OverlaySize => OverlaySize.Small;

        public IEnumerable<DataListItem> GetItems(Dictionary<string, object> config)
        {
            if (config.TryGetValue("imageCropper", out var obj) == true &&
                obj is string str &&
                string.IsNullOrWhiteSpace(str) == false &&
                UdiParser.TryParse(str, out GuidUdi udi) == true)
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
                    });
            }

            return Enumerable.Empty<DataListItem>();
        }
    }
}
