/* Copyright © 2021 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Umbraco.Cms.Core.Models.Membership;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Services;

namespace Umbraco.Community.Contentment.DataEditors
{
    public sealed class UmbracoUserGroupDataListSource : DataListToDataPickerSourceBridge, IDataListSource, IDataSourceValueConverter
    {
        private readonly IUserService _userService;

        public UmbracoUserGroupDataListSource(IUserService userService)
        {
            _userService = userService;
        }

        public override string Name => "Umbraco User Groups";

        public override string Description => "Populate the data source with Umbraco user groups.";

        public override string Icon => UmbConstants.Icons.UserGroup;

        public override string Group => Constants.Conventions.DataSourceGroups.Umbraco;

        public override IEnumerable<ConfigurationField> Fields => Enumerable.Empty<ConfigurationField>();

        public override Dictionary<string, object>? DefaultValues => default;

        public override OverlaySize OverlaySize => OverlaySize.Small;

        public override IEnumerable<DataListItem> GetItems(Dictionary<string, object> config)
        {
            return _userService
                .GetAllUserGroups()
                .Select(x => new DataListItem
                {
                    Name = x.Name,
                    Value = x.Alias,
                    Icon = x.Icon ?? UmbConstants.Icons.UserGroup,
                    Description = string.Join(", ", x.AllowedSections)
                });
        }

        public Type? GetValueType(Dictionary<string, object>? config) => typeof(IUserGroup);

        public object? ConvertValue(Type type, string value) => _userService.GetUserGroupByAlias(value);
    }
}
