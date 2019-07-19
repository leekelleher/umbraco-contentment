/* Copyright © 2019 Lee Kelleher, Umbrella Inc and other contributors.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;

namespace Our.Umbraco.Contentment.DataEditors
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class DataListItemAttribute : Attribute
    {
        public DataListItemAttribute()
            : base()
        { }

        public string Description { get; set; }

        public string Icon { get; set; }

        public string Name { get; set; }

        public string Value { get; set; }
    }
}
