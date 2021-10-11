/* Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

#if NET472
using Umbraco.Core.PropertyEditors;
#else
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Serialization;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Strings;
#endif

namespace Umbraco.Community.Contentment.DataEditors
{
    internal sealed class ReadOnlyDataValueEditor : DataValueEditor
    {
#if NET472 == false
        public ReadOnlyDataValueEditor(
            ILocalizedTextService localizedTextService,
            IShortStringHelper shortStringHelper,
            IJsonSerializer jsonSerializer)
            : base(localizedTextService, shortStringHelper, jsonSerializer)
        { }
#endif

        public override bool IsReadOnly => true;
    }
}
