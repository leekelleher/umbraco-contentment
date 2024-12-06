/* Copyright © 2021 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.PublishedCache;
using Umbraco.Extensions;

namespace Umbraco.Community.Contentment.DataEditors
{
    public sealed class UmbracoTagsDataListSource : DataListToDataPickerSourceBridge, IDataListSource
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UmbracoTagsDataListSource(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public override string Name => "Umbraco Tags";

        public override string Description => "Populate the data source using already defined tags.";

        public override string Icon => "icon-tags";

        public override string Group => Constants.Conventions.DataSourceGroups.Umbraco;

        public override OverlaySize OverlaySize => OverlaySize.Small;

        public override IEnumerable<ContentmentConfigurationField> Fields => new[]
        {
            new ContentmentConfigurationField
            {
                Key = "tagGroup",
                Name = "Tag group",
                Description = "Enter a tag group, or leave empty to use all groups.",
                PropertyEditorUiAlias = "Umb.PropertyEditorUi.TextBox",
            },
        };

        public override Dictionary<string, object>? DefaultValues => new()
        {
            { "tagGroup", "default" },
        };

        public override IEnumerable<DataListItem> GetItems(Dictionary<string, object> config)
        {
            var tagGroup = config.GetValueAs("tagGroup", defaultValue: string.Empty);

            // NOTE: Code smell. Turns out that `ITagQuery` is scoped, so unable to use it within a singleton.
            // Otherwise we get an error: `Cannot resolve scoped service 'Umbraco.Cms.Core.PublishedCache.ITagQuery' from root provider.`
            var tagQuery = _httpContextAccessor.HttpContext?.RequestServices.GetRequiredService<ITagQuery>();

            return tagQuery?
                .GetAllTags(tagGroup)
                .OrderBy(x => x?.Text)
                .Select(x => new DataListItem
                {
                    Name = x?.Text,
                    Value = x?.Text,
                    Icon = Icon,
                    Description = string.Empty,
                }) ?? Enumerable.Empty<DataListItem>();
        }
    }
}
