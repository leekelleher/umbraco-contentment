/* Copyright © 2019 Lee Kelleher and Marc Stöcker.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Collections.Generic;
using Umbraco.Core;
using Umbraco.Core.IO;
using Umbraco.Core.PropertyEditors;

namespace Umbraco.Community.Contentment.DataEditors
{
    internal sealed class ImageButtonListDataListEditor : IDataListEditor
    {
        public string Name => "Image Button List";

        public string Description => "Select a single image from a list of image buttons";

        public string Icon => ImageButtonListDataEditor.DataEditorIcon;

        public Dictionary<string, object> DefaultValues => default;

        public Dictionary<string, object> DefaultConfig => default;

        public string View => IOHelper.ResolveUrl(ImageButtonListDataEditor.DataEditorViewPath);

        [ConfigurationField(typeof(ShowDescriptionsConfigurationField))]
        public bool ShowDescriptions { get; set; }

        [ConfigurationField(typeof(SvgSpriteMediaConfigurationField))]
        public Udi SvgSpriteMedia { get; set; }
    }
}
