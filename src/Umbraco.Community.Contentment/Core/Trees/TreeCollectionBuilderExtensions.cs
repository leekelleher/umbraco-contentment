/* Copyright © 2013-present Umbraco.
 * This Source Code has been derived from Umbraco CMS.
 * https://github.com/umbraco/Umbraco-CMS/blob/release-8.4.0/src/Umbraco.Web/Trees/TreeCollectionBuilder.cs
 * Modified under the permissions of the MIT License.
 * Modifications are licensed under the Mozilla Public License.
 * Copyright © 2020 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Umbraco.Cms.Core.Trees;
using Umbraco.Cms.Web.BackOffice.Trees;

namespace Umbraco.Cms.Web
{
    public static class TreeCollectionBuilderExtensions
    {
        public static TreeCollectionBuilder RemoveTreeController<TController>(this TreeCollectionBuilder collection)
            where TController : TreeControllerBase
        {
            return RemoveTreeController(collection, typeof(TController));
        }

        public static TreeCollectionBuilder RemoveTreeController(this TreeCollectionBuilder collection, Type controllerType)
        {
            var type = typeof(TreeCollectionBuilder);

            // https://github.com/umbraco/Umbraco-CMS/blob/release-8.4.0/src/Umbraco.Web/Trees/TreeCollectionBuilder.cs#L13
            var field = type.GetField("_trees", BindingFlags.GetField | BindingFlags.NonPublic | BindingFlags.Instance);
            if (field == null)
            {
                return collection;
            }

            var trees = (List<Tree>)field.GetValue(collection);
            if (trees == null)
            {
                return collection;
            }

            if (typeof(TreeControllerBase).IsAssignableFrom(controllerType) == false)
            {
                throw new ArgumentException($"Type {controllerType} does not inherit from {nameof(TreeControllerBase)}.");
            }

            var exists = trees.FirstOrDefault(x => x.TreeControllerType == controllerType);
            if (exists != null)
            {
                trees.Remove(exists);
            }

            return collection;
        }
    }
}
