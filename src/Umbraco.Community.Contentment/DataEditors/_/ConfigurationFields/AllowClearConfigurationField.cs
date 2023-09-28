/* Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Umbraco.Cms.Core.PropertyEditors;

namespace Umbraco.Community.Contentment.DataEditors
{
    internal sealed class AllowClearConfigurationField : ConfigurationField
    {
        internal const string AllowClear = "allowClear";

        public AllowClearConfigurationField()
            : base()
        {
            Key = AllowClear;
            Name = "Allow clear?";
            Description = "Select to enable the 'Clear selection' property action.";
            View = "views/propertyeditors/boolean/boolean.html";
            Config = new Dictionary<string, object>
            {
                { "default", Constants.Values.False }
            };
        }
    }
}
