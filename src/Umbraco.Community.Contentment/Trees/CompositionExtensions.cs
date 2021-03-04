/* Copyright © 2021 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Umbraco.Community.Contentment.Trees;
using Umbraco.Web;

namespace Umbraco.Core.Composing
{
    public static partial class CompositionExtensions
    {
        public static Composition DisableContentmentTree(this Composition composition)
        {
            composition
                .Trees()
                    .RemoveTreeController<ContentmentTreeController>()
            ;

            return composition;
        }
    }
}
