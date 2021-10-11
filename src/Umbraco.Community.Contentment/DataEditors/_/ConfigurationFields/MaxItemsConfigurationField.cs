/* Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

#if NET472
using Umbraco.Core.IO;
using Umbraco.Core.PropertyEditors;
#else
using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Extensions;
#endif

namespace Umbraco.Community.Contentment.DataEditors
{
    internal sealed class MaxItemsConfigurationField : ConfigurationField
    {
        internal const string MaxItems = "maxItems";

        public MaxItemsConfigurationField(IIOHelper ioHelper)
            : base()
        {
            Key = MaxItems;
            Name = "Maximum items";
            Description = "Enter the number for the maximum items allowed.<br>Use '0' for an unlimited amount.";
            View = ioHelper.ResolveRelativeOrVirtualUrl(NumberInputDataEditor.DataEditorViewPath);
        }
    }
}
