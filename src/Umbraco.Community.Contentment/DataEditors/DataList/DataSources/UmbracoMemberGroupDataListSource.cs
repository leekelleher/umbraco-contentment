/* Copyright © 2021 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Umbraco.Cms.Core;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Services;

namespace Umbraco.Community.Contentment.DataEditors
{
    public sealed class UmbracoMemberGroupDataListSource : IDataListSource, IDataSourceValueConverter
    {
        private readonly IMemberGroupService _memberGroupService;

        public UmbracoMemberGroupDataListSource(IMemberGroupService memberGroupService)
        {
            _memberGroupService = memberGroupService;
        }

        public string Name => "Umbraco Member Groups";

        public string Description => "Populate the data source with Umbraco member groups.";

        public string Icon => UmbConstants.Icons.MemberGroup;

        public string Group => Constants.Conventions.DataSourceGroups.Umbraco;

        public IEnumerable<ConfigurationField> Fields => Enumerable.Empty<ConfigurationField>();

        public Dictionary<string, object>? DefaultValues => default;

        public OverlaySize OverlaySize => OverlaySize.Small;

        public IEnumerable<DataListItem> GetItems(Dictionary<string, object> config)
        {
            return _memberGroupService
                .GetAll()
                .Select(x => new DataListItem
                {
                    Name = x.Name,
                    Value = x.Key.ToString(),
                    Icon = UmbConstants.Icons.MemberGroup,
                    Description = Udi.Create(UmbConstants.UdiEntityType.MemberGroup, x.Key).ToString()
                });
        }

        public Type GetValueType(Dictionary<string, object>? config) => typeof(Guid);

        public object? ConvertValue(Type type, string value) => Guid.TryParse(value, out var guid) == true ? guid : Guid.Empty;
    }
}
