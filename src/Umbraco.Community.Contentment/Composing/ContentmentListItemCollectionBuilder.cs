﻿/* Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using Umbraco.Community.Contentment.DataEditors;
using Umbraco.Core;
using Umbraco.Core.Composing;

namespace Umbraco.Community.Contentment.Composing
{
    public sealed class ContentmentListItemCollectionBuilder
        : LazyCollectionBuilderBase<ContentmentListItemCollectionBuilder, ContentmentListItemCollection, IContentmentListItem>
    {
        protected override ContentmentListItemCollectionBuilder This => this;
    }

    public sealed class ContentmentListItemCollection : BuilderCollectionBase<IContentmentListItem>
    {
        private readonly Dictionary<string, IContentmentListItem> _lookup;

        public ContentmentListItemCollection(IEnumerable<IContentmentListItem> items)
            : base(items)
        {
            _lookup = new Dictionary<string, IContentmentListItem>(StringComparer.OrdinalIgnoreCase);

            foreach (var item in this)
            {
                var typeName = item.GetType().GetFullNameWithAssembly();
                if (_lookup.ContainsKey(typeName) == false)
                {
                    _lookup.Add(typeName, item);
                }
            }
        }

        internal bool TryGet(string typeName, out IContentmentListItem item)
        {
            return _lookup.TryGetValue(typeName, out item);
        }
    }
}
