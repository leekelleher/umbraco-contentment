/* Copyright © 2021 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Newtonsoft.Json.Linq;
using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.Models.Membership;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Services;
using Umbraco.Extensions;

namespace Umbraco.Community.Contentment.DataEditors
{
    public sealed class UmbracoUsersDataListSource
        : DataListToDataPickerSourceBridge, IDataListSource, IDataSourceValueConverter, IDataSourceDeliveryApiValueConverter
    {
        private readonly IIOHelper _ioHelper;
        private readonly IUserService _userService;

        public UmbracoUsersDataListSource(IIOHelper ioHelper, IUserService userService)
        {
            _ioHelper = ioHelper;
            _userService = userService;
        }

        public override string Name => "Umbraco Users";

        public override string Description => "Use Umbraco users to populate the data source.";

        public override string Icon => UmbConstants.Icons.User;

        public override string Group => Constants.Conventions.DataSourceGroups.Umbraco;

        public override IEnumerable<ConfigurationField> Fields
        {
            get
            {
                var items = _userService
                    .GetAllUserGroups()
                    .Select(x => new DataListItem
                    {
                        Name = x.Name,
                        Value = x.Alias,
                        Icon = x.Icon ?? UmbConstants.Icons.UserGroup,
                        Description = string.Join(", ", x.AllowedSections)
                    })
                    .ToList();

                return new[]
                {
                    new ConfigurationField
                    {
                        Key = "userGroup",
                        Name = "User Group",
                        Description = "Select a user group to filter the users by. If left empty, all users will be used.",
                        View = _ioHelper.ResolveRelativeOrVirtualUrl(ItemPickerDataListEditor.DataEditorViewPath),
                        Config = new Dictionary<string, object>
                        {
                            { "enableFilter", items.Count > 5 ? Constants.Values.True : Constants.Values.False },
                            { Constants.Conventions.ConfigurationFieldAliases.Items, items },
                            { "listType", "list" },
                            { Constants.Conventions.ConfigurationFieldAliases.OverlayView, _ioHelper.ResolveRelativeOrVirtualUrl(ItemPickerDataListEditor.DataEditorOverlayViewPath) ?? string.Empty },
                            { MaxItemsConfigurationField.MaxItems, 1 },
                        }
                    }
                };
            }
        }

        public override Dictionary<string, object>? DefaultValues => default;

        public override OverlaySize OverlaySize => OverlaySize.Small;

        public override IEnumerable<DataListItem> GetItems(Dictionary<string, object> config)
        {
            DataListItem mapUser(IUser user) => new()
            {
                Name = user.Name ?? user.Username,
                Value = user.Username,
                Icon = Icon,
                Description = user.Username,
            };

            if (config.TryGetValueAs("userGroup", out JArray? array) == true &&
                array?.Count > 0 &&
                array[0].Value<string>() is string alias &&
                string.IsNullOrWhiteSpace(alias) == false)
            {
                var userGroup = _userService.GetUserGroupByAlias(alias);
                if (userGroup != null)
                {
                    return _userService.GetAllInGroup(userGroup.Id).Select(mapUser);
                }
            }

            return _userService.GetAll(0, int.MaxValue, out _).Select(mapUser);
        }

        public Type? GetValueType(Dictionary<string, object>? config) => typeof(IUser);

        public object? ConvertValue(Type type, string value) => _userService.GetByUsername(value);

        public Type? GetDeliveryApiValueType(Dictionary<string, object>? config) => typeof(string);

        public object? ConvertToDeliveryApiValue(Type type, string value, bool expanding = false) => value;
    }
}
