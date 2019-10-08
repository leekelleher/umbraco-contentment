/* Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Collections.Generic;
using Umbraco.Core.PropertyEditors;

namespace Umbraco.Community.Contentment.DataEditors
{
    internal sealed class AllowEmptyConfigurationField : ConfigurationField
    {
        internal const string AllowEmpty = "allowEmpty";

        public AllowEmptyConfigurationField()
            : base()
        {
            Key = AllowEmpty;
            Name = "Allow Empty";
            Description = "Enable to allow an empty option at the top of the dropdown list.";
            View = "views/propertyeditors/boolean/boolean.html";
            Config = new Dictionary<string, object>
            {
                { "default", Constants.Values.True }
            };
        }
    }
}
