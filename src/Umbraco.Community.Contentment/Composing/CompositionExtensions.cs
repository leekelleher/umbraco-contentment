/* Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Umbraco.Community.Contentment.Composing;
using Umbraco.Community.Contentment.DataEditors;

// NOTE: This extension method class is deliberately using the Umbraco namespace,
// as to reduce namespace imports and ease the developer experience. [LK]
namespace Umbraco.Core.Composing
{
    // TODO: [LK:2021-03-04] Rename class to `CompositionExtensions` v2.0.0. For consistency with the other `Composition` extension classes.
    public static partial class ContentmentCompositionExtensions
    {
        public static ContentmentListItemCollectionBuilder ContentmentListItems(this Composition composition)
        {
            return composition.WithCollectionBuilder<ContentmentListItemCollectionBuilder>();
        }

        public static Composition UnlockContentment(this Composition composition)
        {
            composition
                .WithCollectionBuilder<ContentmentListItemCollectionBuilder>()
                    // Data List - Data Sources
                    .Add<CountriesDataListSource>()
                    .Add<CurrenciesDataListSource>()
                    .Add<TimeZoneDataListSource>()
                    .Add<uCssClassNameDataListSource>()
                    .Add<UmbracoContentPropertiesDataListSource>()
                    .Add<UmbracoContentXPathDataListSource>()
                    .Add<UmbracoDictionaryDataListSource>()
                    .Add<UmbracoEntityDataListSource>()
                    .Add<UmbracoImageCropDataListSource>()
                    .Add<UmbracoMemberGroupDataListSource>()
                    .Add<UserDefinedDataListSource>()
            ;

            return composition;
        }
    }
}
