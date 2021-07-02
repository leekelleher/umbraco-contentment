/* Copyright © 2021 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Core;
using Umbraco.Core.PropertyEditors;
using Umbraco.Web;

namespace Umbraco.Community.Contentment.DataEditors
{
    public sealed class UmbracoTagsDataListSource : IDataListSource
    {
        private readonly Lazy<ITagQuery> _tagQuery;

        public UmbracoTagsDataListSource(Lazy<ITagQuery> tagQuery)
        {
            _tagQuery = tagQuery;
        }

        public string Name => "Umbraco Tags";

        public string Description => "Use Umbraco Tags as a data source.";

        public string Icon => "icon-tags";

        public string Group => default;

        public OverlaySize OverlaySize => OverlaySize.Small;

        public IEnumerable<ConfigurationField> Fields => new[]
        {
            new ConfigurationField
            {
                Key = "tagGroup",
                Name = "Tag group",
                Description = "Enter a tag group, or leave empty to use all groups.",
                View = "textstring",
            },
        };

        public Dictionary<string, object> DefaultValues => new Dictionary<string, object>
        {
            { "tagGroup", "default" },
        };

        public IEnumerable<DataListItem> GetItems(Dictionary<string, object> config)
        {
            var tagGroup = config.GetValueAs("tagGroup", defaultValue: string.Empty);

            return _tagQuery.Value
                .GetAllTags(tagGroup)
                .OrderBy(x => x.Text)
                .Select(x => new DataListItem
                {
                    Name = x.Text,
                    Value = x.Text,
                    Icon = Icon,
                    Description = string.Empty,
                });
        }
    }
}
