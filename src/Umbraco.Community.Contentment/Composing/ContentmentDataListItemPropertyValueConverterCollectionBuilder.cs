/* Copyright © 2022 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.ComponentModel;
using Umbraco.Cms.Core.Composing;
using Umbraco.Community.Contentment.DataEditors;

namespace Umbraco.Community.Contentment.Composing
{
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public sealed class ContentmentDataListItemPropertyValueConverterCollectionBuilder
        : LazyCollectionBuilderBase<ContentmentDataListItemPropertyValueConverterCollectionBuilder, ContentmentDataListItemPropertyValueConverterCollection, IDataListItemPropertyValueConverter>
    {
        protected override ContentmentDataListItemPropertyValueConverterCollectionBuilder This => this;
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public sealed class ContentmentDataListItemPropertyValueConverterCollection : BuilderCollectionBase<IDataListItemPropertyValueConverter>
    {
        public ContentmentDataListItemPropertyValueConverterCollection(Func<IEnumerable<IDataListItemPropertyValueConverter>> items)
            : base(items)
        { }
    }
}
