/* Copyright © 2020 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using Umbraco.Core;
using Umbraco.Core.IO;
using Umbraco.Core.Models;
using Umbraco.Core.PropertyEditors;
using Umbraco.Core.Services;
using UmbConstants = Umbraco.Core.Constants;

namespace Umbraco.Community.Contentment.DataEditors
{
    [Core.Composing.HideFromTypeFinder]
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

        private IEnumerable<ConfigurationField> _fields;

        public IEnumerable<ConfigurationField> Fields
        {
            get
            {
                if (_fields == null)
                {
                    var items = _dataTypeService
                        .GetByEditorAlias(UmbConstants.PropertyEditors.Aliases.ImageCropper)
                        .Select(x => new DataListItem
                        {
                            Icon = this.Icon,
                            Description = x.EditorAlias,
                            Name = x.Name,
                            Value = Udi.Create(UmbConstants.UdiEntityType.DataType, x.Key).ToString(),
                        });

                    _fields = new ConfigurationField[]
                    {
                        new ConfigurationField
                        {
                            Key = "imageCropper",
                            Name = "Image Cropper",
                            Description = "Select a Data Type that uses the Image Cropper.",
                            View = IOHelper.ResolveUrl(ItemPickerDataEditor.DataEditorViewPath),
                            Config = new Dictionary<string, object>
                            {
                                { EnableFilterConfigurationField.EnableFilter, items.Count() > 5 ? Constants.Values.True : Constants.Values.False },
                                { ItemPickerConfigurationEditor.Items, items },
                                { ItemPickerTypeConfigurationField.ListType, ItemPickerTypeConfigurationField.List },
                                { ItemPickerConfigurationEditor.OverlayView, IOHelper.ResolveUrl(ItemPickerDataEditor.DataEditorOverlayViewPath) },
                                { MaxItemsConfigurationField.MaxItems, 1 },
                            }
                        }
                    };
                }

                return _fields;
            }
        }

        public Dictionary<string, object> DefaultValues => default;

        public IEnumerable<DataListItem> GetItems(Dictionary<string, object> config)
        {
            if (config.TryGetValue("imageCropper", out var v) &&
                v is JArray a &&
                a.Count > 0 &&
                a[0].Value<string>() is string s &&
                string.IsNullOrWhiteSpace(s) == false &&
                GuidUdi.TryParse(s, out var udi))
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
