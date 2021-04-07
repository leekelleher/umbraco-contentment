/* Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using ClientDependency.Core;
using Umbraco.Community.Contentment.Composing;
using Umbraco.Community.Contentment.DataEditors;

// NOTE: This extension method class is deliberately using the Umbraco namespace,
// as to reduce namespace imports and ease the developer experience. [LK]
namespace Umbraco.Core.Composing
{
    // TODO: [LK:2021-03-04] Rename class to `CompositionExtensions` v2.0.0. For consistency with the other `Composition` extension classes.
    public static partial class ContentmentCompositionExtensions
    {
        private readonly static HashSet<string> _lookup = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "CAF5E56A2E38ECDA55EF97C348EDDBCA"
        };

        public static ContentmentListItemCollectionBuilder ContentmentListItems(this Composition composition)
        {
            return composition.WithCollectionBuilder<ContentmentListItemCollectionBuilder>();
        }

        public static Composition UnlockContentment(this Composition composition, params string[] passcodes)
        {
            if (passcodes?.Length == 0)
            {
                return composition;
            }

            foreach (var passcode in passcodes)
            {
                if (string.IsNullOrWhiteSpace(passcode) == true)
                {
                    continue;
                }

                var hashed = passcode.ToLowerInvariant().GenerateMd5();
                if (_lookup.Contains(hashed) == false)
                {
                    continue;
                }

                ContentmentComponent.Unlocked = true;

                composition
                    .WithCollectionBuilder<ContentmentListItemCollectionBuilder>()
                        .Add<CountriesDataListSource>()
                        .Add<TimeZoneDataListSource>()
                        .Add<uCssClassNameDataListSource>()
                        .Add<UmbracoContentPropertiesDataListSource>()
                        .Add<UmbracoContentXPathDataListSource>()
                        .Add<UmbracoEntityDataListSource>()
                        .Add<UmbracoImageCropDataListSource>()
                        .Add<UmbracoMemberGroupDataListSource>()
                ;
            }

            return composition;
        }
    }
}
