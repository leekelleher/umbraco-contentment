// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

using Umbraco.Cms.Core.Strings;
using Umbraco.Community.Contentment.Composing;
using Umbraco.Community.Contentment.DataEditors;
using InternalConstants = Umbraco.Community.Contentment.Constants.Internals;

namespace Umbraco.Extensions;

internal static class ContentmentListItemCollectionExtensions
{
    public static IEnumerable<object> GetExtensionsForManifest(
        this ContentmentListItemCollection listItems,
        ConfigurationEditorUtility utility,
        IShortStringHelper shortStringHelper)
    {
        var extensions = new List<object>();

        foreach (var listItem in listItems.OfType<IContentmentEditorItem>())
        {
            var suffix = listItem switch
            {
                IDataListSource => "DataSource",
                IContentmentListEditor => "ListEditor",
                IContentmentDisplayMode => "DisplayMode",
                _ => null,
            };

            if (string.IsNullOrWhiteSpace(suffix))
            {
                continue;
            }

            var itemType = listItem.GetType();

            var type = InternalConstants.ProjectAlias + suffix;
            var name = InternalConstants.DataEditorNamePrefix + itemType.Name.SplitPascalCasing(shortStringHelper);
            var alias = InternalConstants.ManifestAliasPrefix + suffix + "." + itemType.Name;
            var meta = utility.GetConfigurationEditorModel(listItem);

            extensions.Add(new { type, alias, name, meta });
        }

        return extensions;
    }
}
