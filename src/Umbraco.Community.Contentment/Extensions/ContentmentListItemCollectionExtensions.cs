// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

using Umbraco.Community.Contentment.Composing;
using Umbraco.Community.Contentment.DataEditors;
using InternalConstants = Umbraco.Community.Contentment.Constants.Internals;

namespace Umbraco.Extensions;

internal static class ContentmentListItemCollectionExtensions
{
    public static IEnumerable<object> GetExtensionsForManifest(
        this ContentmentListItemCollection listItems,
        ConfigurationEditorUtility utility)
    {
        var extensions = new List<object>();

        foreach (var listItem in listItems.OfType<IContentmentEditorItem>())
        {
            var suffix = listItem switch
            {
                IContentmentListEditor => "ListEditor",
                IContentmentDataSource => "DataSource",
                IDataPickerSource => "DataSource", // TODO: [LK] Remove this once I've figured out how `IContentmentDataSource` can support search.
                _ => null,
            };

            if (string.IsNullOrWhiteSpace(suffix))
            {
                continue;
            }

            var itemType = listItem.GetType();

            var type = InternalConstants.ProjectAlias + suffix;
            var name = InternalConstants.DataEditorNamePrefix + listItem.Name;
            var alias = InternalConstants.ManifestAliasPrefix + suffix + "." + itemType.Name;
            var meta = utility.GetConfigurationEditorModel(listItem);

            extensions.Add(new { type, alias, name, meta });
        }

        return extensions;
    }
}
