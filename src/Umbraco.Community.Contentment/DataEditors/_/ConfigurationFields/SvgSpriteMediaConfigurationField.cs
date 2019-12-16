/* Copyright © 2019 Marc Stöcker.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Umbraco.Core.PropertyEditors;

namespace Umbraco.Community.Contentment.DataEditors
{
    internal sealed class SvgSpriteMediaConfigurationField : ConfigurationField
    {
        internal const string SpriteMedia = "svgSpriteMedia";

        public SvgSpriteMediaConfigurationField()
            : base()
        {
            Key = SpriteMedia;
            Name = "SVG Sprite";
            Description = "Choose the SVG sprite media file to use.";
            View = "mediapicker";
        }
    }
}
