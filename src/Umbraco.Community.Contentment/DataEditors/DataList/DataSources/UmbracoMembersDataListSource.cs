/* Copyright © 2021 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Umbraco.Cms.Api.Common.ViewModels.Pagination;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.DeliveryApi;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.DeliveryApi;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.PublishedCache;
using Umbraco.Cms.Core.Services;
using Umbraco.Extensions;

namespace Umbraco.Community.Contentment.DataEditors
{
    public sealed class UmbracoMembersDataListSource
        : IContentmentDataSource, IDataPickerSource, IDataSourceValueConverter, IDataSourceDeliveryApiValueConverter
    {
        private readonly IApiElementBuilder _apiElementBuilder;
        private readonly IMemberTypeService _memberTypeService;
        private readonly IMemberService _memberService;
        private readonly IPublishedMemberCache _publishedMemberCache;

        public UmbracoMembersDataListSource(
            IApiElementBuilder apiElementBuilder,
            IMemberTypeService memberTypeService,
            IMemberService memberService,
            IPublishedMemberCache publishedMemberCache)
        {
            _apiElementBuilder = apiElementBuilder;
            _memberTypeService = memberTypeService;
            _memberService = memberService;
            _publishedMemberCache = publishedMemberCache;
        }

        public string Name => "Umbraco Members";

        public string Description => "Use Umbraco members to populate the data source.";

        public string Icon => UmbConstants.Icons.Member;

        public string Group => Constants.Conventions.DataSourceGroups.Umbraco;

        public IEnumerable<ContentmentConfigurationField> Fields
        {
            get
            {
                return new[]
                {
                    new ContentmentConfigurationField
                    {
                        Key = "memberType",
                        Name = "Member Type",
                        Description = "Select a member type to filter the members by. If left empty, all members will be used.",
                        PropertyEditorUiAlias = Constants.Internals.DataEditorUiAliasPrefix + "MemberTypePicker",
                     }
                };
            }
        }

        public Dictionary<string, object>? DefaultValues => default;

        public OverlaySize OverlaySize => OverlaySize.Small;

        public IEnumerable<DataListItem> GetItems(Dictionary<string, object> config)
        {
            var memberType = GetMemberType(config);

            return memberType is not null
                ? _memberService.GetMembersByMemberType(memberType.Id).Select(ToDataListItem)
                : _memberService.GetAllMembers().Select(ToDataListItem);
        }

        public Task<IEnumerable<DataListItem>> GetItemsAsync(Dictionary<string, object> config, IEnumerable<string> values)
        {
            if (values?.Any() == true)
            {
                return Task.FromResult(values
                    .Select(x => UdiParser.TryParse(x, out GuidUdi? udi) == true ? udi : null)
                    .WhereNotNull()
                    .Select(x => _memberService.GetByKey(x.Guid))
                    .WhereNotNull()
                    .Select(ToDataListItem));
            }

            return Task.FromResult(Enumerable.Empty<DataListItem>());
        }

        public Task<PagedViewModel<DataListItem>> SearchAsync(Dictionary<string, object> config, int pageNumber = 1, int pageSize = 12, string query = "")
        {
            var totalRecords = -1L;
            var pageIndex = pageNumber - 1;
            var memberType = GetMemberType(config);
            var items = _memberService.GetAll(pageIndex, pageSize, out totalRecords, "LoginName", Direction.Ascending, memberType?.Alias, query);

            if (items?.Any() == true)
            {
                var offset = pageIndex * pageSize;
                var results = new PagedViewModel<DataListItem>
                {
                    Items = items.Skip(offset).Take(pageSize).Select(ToDataListItem),
                    Total = pageSize > 0 ? (long)Math.Ceiling(totalRecords / (decimal)pageSize) : 1,
                };

                return Task.FromResult(results);
            }

            return Task.FromResult(PagedViewModel<DataListItem>.Empty());
        }

        public Type? GetValueType(Dictionary<string, object>? config) => typeof(IPublishedContent);

        public object? ConvertValue(Type type, string value)
        {
            if (UdiParser.TryParse(value, out GuidUdi? udi) == true &&
                udi is not null &&
                udi.Guid.Equals(Guid.Empty) == false)
            {
                var member = _memberService.GetByKey(udi.Guid);
                if (member != null)
                {
                    return _publishedMemberCache.Get(member);
                }
            }

            return default(IPublishedContent);
        }

        public Type? GetDeliveryApiValueType(Dictionary<string, object>? config) => typeof(IApiElement);

        public object? ConvertToDeliveryApiValue(Type type, string value, bool expanding = false)
            => ConvertValue(type, value) is IPublishedContent content ? _apiElementBuilder.Build(content) : default;

        private IMemberType? GetMemberType(Dictionary<string, object> config)
        {
            if (config.TryGetValueAs("memberType", out string? guid) == true &&
               string.IsNullOrWhiteSpace(guid) == false &&
               Guid.TryParse(guid, out var key) == true &&
               key.Equals(Guid.Empty) == false)
            {
                return _memberTypeService.Get(key);
            }

            return default;
        }

        private DataListItem ToDataListItem(IMember member)
        {
            var guidUdi = member.GetUdi().ToString();

            return new DataListItem
            {
                Name = member.Name,
                Value = guidUdi,
                Icon = member.ContentType.Icon ?? UmbConstants.Icons.Member,
                Description = guidUdi,
            };
        }
    }
}
