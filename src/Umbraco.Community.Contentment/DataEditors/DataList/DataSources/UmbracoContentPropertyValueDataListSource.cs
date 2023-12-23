/* Copyright © 2022 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */
/* Inspiration for this feature came from Tim Geyssens' article:
 * https://dev.to/timgeyssens/content-editor-defined-dropdowns-checkboxlists-and-radiobuttonlists-in-umbraco-v8-with-contentment-123f
 * The source code was made available at: (license unknown, assumed MIT licensed)
 * https://gist.github.com/TimGeyssens/5e9e156d66c3d85d0bfb24a1ae9a7504
 * My own fork and modifications were made at:
 * https://gist.github.com/leekelleher/d786f4b72e7c16f37fb22d4d23c1b516
 * Modified under the permissions of the MIT License.
 * Modifications are licensed under the Mozilla Public License.
 */

using Umbraco.Cms.Core;
using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Web;
using Umbraco.Community.Contentment.Composing;
using Umbraco.Community.Contentment.Services;
using Umbraco.Extensions;

namespace Umbraco.Community.Contentment.DataEditors.DataList.DataSources
{
    public sealed class UmbracoContentPropertyValueDataListSource : IDataListSource
    {
        private readonly ContentmentDataListItemPropertyValueConverterCollection _converters;
        private readonly IContentmentContentContext _contentmentContentContext;
        private readonly IIOHelper _ioHelper;
        private readonly IUmbracoContextAccessor _umbracoContextAccessor;

        public UmbracoContentPropertyValueDataListSource(
            ContentmentDataListItemPropertyValueConverterCollection converters,
            IContentmentContentContext contentmentContentContext,
            IIOHelper ioHelper,
            IUmbracoContextAccessor umbracoContextAccessor)
        {
            _converters = converters;
            _contentmentContentContext = contentmentContentContext;
            _ioHelper = ioHelper;
            _umbracoContextAccessor = umbracoContextAccessor;
        }

        public string Name => "Umbraco Content Property Values";

        public string Description => "Use Umbraco content property values to populate a data source.";

        public string Icon => "icon-umbraco";

        public OverlaySize OverlaySize => OverlaySize.Small;

        public IEnumerable<ConfigurationField> Fields => new ConfigurationField[]
        {
            new ConfigurationField
            {
                Key = "contentNode",
                Name = "Content node",
                Description = "Set the content node to take the property value from.",
                View =  _ioHelper.ResolveRelativeOrVirtualUrl(ContentPickerDataEditor.DataEditorSourceViewPath),
            },
            new ConfigurationField
            {
                Key = "propertyAlias",
                Name = "Property alias",
                Description = "Set the property alias to populate the data source with, (from the content node).",
                View = "textstring",
            },
        };

        public Dictionary<string, object>? DefaultValues => default;

        public string Group => Constants.Conventions.DataSourceGroups.Umbraco;

        public IEnumerable<DataListItem> GetItems(Dictionary<string, object> config)
        {
            var contentNode = config.GetValueAs("contentNode", string.Empty);
            var propertyAlias = config.GetValueAs("propertyAlias", string.Empty);

            if (string.IsNullOrWhiteSpace(contentNode) == false &&
                string.IsNullOrWhiteSpace(propertyAlias) == false &&
                _umbracoContextAccessor.TryGetUmbracoContext(out var umbracoContext) == true)
            {
                var preview = true;
                var startNode = default(IPublishedContent);

                if (contentNode.InvariantStartsWith("umb://document/") == false)
                {
                    IEnumerable<string> getPath(int id) => umbracoContext.Content?.GetById(preview, id)?.Path.ToDelimitedList().Reverse() ?? UmbConstants.System.RootString.AsEnumerableOfOne();
                    bool publishedContentExists(int id) => umbracoContext.Content?.GetById(preview, id) != null;

                    var parsed = _contentmentContentContext.ParseXPathQuery(contentNode, getPath, publishedContentExists);

                    if (string.IsNullOrWhiteSpace(parsed) == false && parsed.StartsWith('$') == false)
                    {
#pragma warning disable CS0618 // Type or member is obsolete
                        startNode = umbracoContext.Content?.GetSingleByXPath(preview, parsed);
#pragma warning restore CS0618 // Type or member is obsolete
                    }
                }
                else if (UdiParser.TryParse(contentNode, out GuidUdi? udi) == true && udi is not null && udi.Guid != Guid.Empty)
                {
                    startNode = umbracoContext.Content?.GetById(preview, udi.Guid);
                }

                if (startNode != null)
                {
                    var property = startNode.GetProperty(propertyAlias);
                    if (property is not null)
                    {
                        foreach (var converter in _converters)
                        {
                            if (converter.IsConverter(property.PropertyType) == true)
                            {
                                return converter.ConvertTo(property);
                            }
                        }
                    }
                }
            }

            return Enumerable.Empty<DataListItem>();
        }
    }
}
