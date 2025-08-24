/* Copyright © 2020 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Umbraco.Cms.Core;
using Umbraco.Cms.Core.DeliveryApi;
using Umbraco.Cms.Core.Models.DeliveryApi;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.PublishedCache;
using Umbraco.Cms.Core.Web;
using Umbraco.Community.Contentment.Services;
using Umbraco.Extensions;

namespace Umbraco.Community.Contentment.DataEditors
{
    [Obsolete("To be removed in Contentment 8.0")]
    public sealed class UmbracoContentXPathDataListSource
        : DataListToDataPickerSourceBridge, IDataListSource, IDataSourceValueConverter, IDataSourceDeliveryApiValueConverter
    {
        private readonly IApiContentBuilder _apiContentBuilder;
        private readonly IContentmentContentContext _contentmentContentContext;
        private readonly IUmbracoContextAccessor _umbracoContextAccessor;

        public UmbracoContentXPathDataListSource(
            IApiContentBuilder apiContentBuilder,
            IContentmentContentContext contentmentContentContext,
            IUmbracoContextAccessor umbracoContextAccessor)
        {
            _apiContentBuilder = apiContentBuilder;
            _contentmentContentContext = contentmentContentContext;
            _umbracoContextAccessor = umbracoContextAccessor;
        }

        public override string Name => "Umbraco Content by XPath";

        public override string Description => "Use an XPath query to select Umbraco content to use as the data source.";

        public override string Icon => "icon-block color-red";

        public override OverlaySize OverlaySize => OverlaySize.Small;

        public override IEnumerable<ContentmentConfigurationField> Fields =>
        [
            new ContentmentConfigurationField
            {
                Key = "deprecated",
                Name = "Deprecated",
                PropertyEditorUiAlias = EditorNotesDataEditor.DataEditorUiAlias,
                Config = new Dictionary<string, object>
                {
                    { "alertType", "warning" },
                    { "icon", "icon-alert" },
                    { "heading", "Umbraco Content by XPath has been deprecated" },
                    { "message", "<p><em>Support for XPath was deprecated in Umbraco 14.</em></p><p>Please consider using the <strong>Umbraco Content</strong> data-source with the Dynamic Root feature.</p>" },
                    { "hideLabel", true },
                },
            },
            new ContentmentConfigurationField
            {
                Key = "xpath",
                Name = "XPath",
                Description = "Enter the XPath expression to select the content.",
                PropertyEditorUiAlias = "Umb.PropertyEditorUi.TextBox",
            },
        ];

        public override Dictionary<string, object>? DefaultValues => new()
        {
            { "xpath", "/root/*[@level = 1]/*[@isDoc]" },
        };

        public override string Group => Constants.Conventions.DataSourceGroups.Umbraco;

        public override IEnumerable<DataListItem> GetItems(Dictionary<string, object> config)
        {
            var xpath = config.GetValueAs("xpath", string.Empty);

            if (string.IsNullOrWhiteSpace(xpath) == false &&
                _umbracoContextAccessor.TryGetUmbracoContext(out var umbracoContext) == true &&
                umbracoContext.Content is IPublishedContentCache contentCache)
            {
                var preview = true;

                IEnumerable<string> getPath(int id) => contentCache.GetById(preview, id)?.Path.ToDelimitedList().Reverse() ?? UmbConstants.System.RootString.AsEnumerableOfOne();
                bool publishedContentExists(int id) => contentCache.GetById(preview, id) != null;

                var parsed = _contentmentContentContext.ParseXPathQuery(xpath, getPath, publishedContentExists);

                if (string.IsNullOrWhiteSpace(parsed) == false && parsed.StartsWith('$') == false)
                {
                    return Enumerable.Empty<DataListItem>();
                }
            }

            return Enumerable.Empty<DataListItem>();
        }

        public Type? GetValueType(Dictionary<string, object>? config) => typeof(IPublishedContent);

        public object? ConvertValue(Type type, string value)
        {
            return UdiParser.TryParse(value, out GuidUdi? udi) == true
                && udi is not null
                && _umbracoContextAccessor.TryGetUmbracoContext(out var umbracoContext) == true
                ? umbracoContext.Content?.GetById(udi.Guid)
                : default;
        }

        public Type? GetDeliveryApiValueType(Dictionary<string, object>? config) => typeof(IApiContent);

        public object? ConvertToDeliveryApiValue(Type type, string value, bool expanding = false)
            => ConvertValue(type, value) is IPublishedContent content ? _apiContentBuilder.Build(content) : default;
    }
}
