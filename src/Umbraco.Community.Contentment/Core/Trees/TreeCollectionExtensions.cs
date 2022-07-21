/* Copyright © 2021 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

#if NET472 == false
using System;
using System.Linq;
using System.Reflection;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.Trees;
using Umbraco.Cms.Web.BackOffice.Trees;

namespace Umbraco.Extensions
{
    internal static class TreeCollectionExtensions
    {
        public static TreeCollection RemoveTreeController<TController>(this TreeCollection collection)
            where TController : TreeControllerBase
        {
            var controllerType = typeof(TController);
            var type = typeof(BuilderCollectionBase<Tree>);

            // https://github.com/umbraco/Umbraco-CMS/blob/release-9.0.0/src/Umbraco.Core/Composing/BuilderCollectionBase.cs#L14
            var field = type.GetField("_items", BindingFlags.GetField | BindingFlags.NonPublic | BindingFlags.Instance);
            if (field == null)
            {
                return collection;
            }

            var lazy = (LazyReadOnlyCollection<Tree>)field.GetValue(collection);
            if (lazy?.Value == null)
            {
                return collection;
            }

            var trees = lazy.Value.ToArray();

            if (typeof(TreeControllerBase).IsAssignableFrom(controllerType) == false)
            {
                throw new ArgumentException($"Type {controllerType} does not inherit from {nameof(TreeControllerBase)}.");
            }

            var idx = Array.FindIndex(trees, x => x.TreeControllerType == controllerType);
            if (idx > -1)
            {
                var tmp = new Tree[trees.Length - 1];

                if (idx > 0)
                {
                    Array.Copy(trees, 0, tmp, 0, idx);
                }

                if (idx < trees.Length - 1)
                {
                    Array.Copy(trees, idx + 1, tmp, idx, trees.Length - idx - 1);
                }

                field.SetValue(collection, new LazyReadOnlyCollection<Tree>(() => tmp));
            }

            return collection;
        }
    }
}

#endif
