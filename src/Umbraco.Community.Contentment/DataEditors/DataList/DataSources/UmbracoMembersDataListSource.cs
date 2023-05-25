/* Copyright © 2021 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
#if NET472
using Umbraco.Core;
using Umbraco.Core.IO;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Core.Persistence.DatabaseModelDefinitions;
using Umbraco.Core.Persistence.Querying;
using Umbraco.Core.PropertyEditors;
using Umbraco.Core.Services;
using Umbraco.Web;
using Umbraco.Web.PublishedCache;
using UmbConstants = Umbraco.Core.Constants;
#else
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Persistence.Querying;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.PublishedCache;
using Umbraco.Cms.Core.Services;
using Umbraco.Extensions;
using UmbConstants = Umbraco.Cms.Core.Constants;
#endif

namespace Umbraco.Community.Contentment.DataEditors
{
    public sealed class UmbracoMembersDataListSource : IDataListSource, IDataPickerSource, IDataSourceValueConverter
    {
        private readonly IMemberTypeService _memberTypeService;
        private readonly IMemberService _memberService;
        private readonly IPublishedSnapshotAccessor _publishedSnapshotAccessor;
        private readonly IIOHelper _ioHelper;

        public UmbracoMembersDataListSource(
            IMemberTypeService memberTypeService,
            IMemberService memberService,
            IPublishedSnapshotAccessor publishedSnapshotAccessor,
            IIOHelper ioHelper)
        {
            _memberTypeService = memberTypeService;
            _memberService = memberService;
            _publishedSnapshotAccessor = publishedSnapshotAccessor;
            _ioHelper = ioHelper;
        }

        public string Name => "Umbraco Members";

        public string Description => "Use Umbraco members to populate the data source.";

        public string Icon => UmbConstants.Icons.Member;

        public string Group => Constants.Conventions.DataSourceGroups.Umbraco;

        public IEnumerable<ConfigurationField> Fields
        {
            get
            {
                var items = _memberTypeService
                    .GetAll()
                    .Select(x => new DataListItem
                    {
                        Icon = x.Icon,
                        Description = x.Description,
                        Name = x.Name,
                        Value = Udi.Create(UmbConstants.UdiEntityType.MemberType, x.Key).ToString(),
                    })
                    .ToList();

                return new[]
                {
                    // TODO: [LK] Move this note to the Data List preview field, when there are over 30 items, then suggest using the Data Picker editor instead.
                    new NotesConfigurationField(_ioHelper, $@"<details class=""alert alert-danger"">
<summary><strong>Important note about Umbraco Members.</strong></summary>
<p>This data source is ideal for smaller number of members, e.g. under 50. Upwards of that, you will notice an unpleasant editor experience and rapidly diminished performance.</p>
<p>Remember...</p>
<blockquote cite=""https://en.wikipedia.org/wiki/With_great_power_comes_great_responsibility"">
<p>&ldquo;With great power comes great responsibility!&rdquo;</p>
</blockquote>
<p class=""text-right"">—Benjamin Franklin Parker</p>
</details>", true),
                    new ConfigurationField
                    {
                        Key = "memberType",
                        Name = "Member Type",
                        Description = "Select a member type to filter the members by. If left empty, all members will be used.",
                        View = _ioHelper.ResolveRelativeOrVirtualUrl(ItemPickerDataListEditor.DataEditorViewPath),
                        Config = new Dictionary<string, object>
                        {
                            { "addButtonLabelKey", "defaultdialogs_selectMemberType" },
                            { "enableFilter", items.Count > 5 ? Constants.Values.True : Constants.Values.False },
                            { Constants.Conventions.ConfigurationFieldAliases.Items, items },
                            { "listType", "list" },
                            { Constants.Conventions.ConfigurationFieldAliases.OverlayView, _ioHelper.ResolveRelativeOrVirtualUrl(ItemPickerDataListEditor.DataEditorOverlayViewPath) },
                            { MaxItemsConfigurationField.MaxItems, 1 },
                        }
                    }
                };
            }
        }

        public Dictionary<string, object> DefaultValues => default;

        public OverlaySize OverlaySize => OverlaySize.Small;

        public IEnumerable<DataListItem> GetItems(Dictionary<string, object> config)
        {
            var memberType = GetMemberType(config);

            return memberType != null
                ? _memberService.GetMembersByMemberType(memberType.Id).Select(ToDataListItem)
                : _memberService.GetAllMembers().Select(ToDataListItem);
        }

        public Task<IEnumerable<DataListItem>> GetItemsAsync(Dictionary<string, object> config, IEnumerable<string> values)
        {
            if (values?.Any() == true)
            {
                return Task.FromResult(values
                    .Select(x => UdiParser.TryParse(x, out GuidUdi udi) == true ? udi : null)
                    .WhereNotNull()
                    .Select(x => _memberService.GetByKey(x.Guid))
                    .WhereNotNull()
                    .Select(ToDataListItem));
            }

            return Task.FromResult(Enumerable.Empty<DataListItem>());
        }

        public Task<PagedResult<DataListItem>> SearchAsync(Dictionary<string, object> config, int pageNumber = 1, int pageSize = 12, string query = "")
        {
            var totalRecords = -1L;
            var pageIndex = pageNumber - 1;
            var memberType = GetMemberType(config);
            var items = _memberService.GetAll(pageIndex, pageSize, out totalRecords, "LoginName", Direction.Ascending, memberType?.Alias, query);

            if (items?.Any() == true)
            {
                var offset = pageIndex * pageSize;
                var results = new PagedResult<DataListItem>(totalRecords, pageNumber, pageSize)
                {
                    Items = items
                        .Skip(offset)
                        .Take(pageSize)
                        .Select(ToDataListItem)
                };

                return Task.FromResult(results);
            }

            return Task.FromResult(new PagedResult<DataListItem>(totalRecords, pageNumber, pageSize));
        }

        public Type GetValueType(Dictionary<string, object> config) => typeof(IPublishedContent);

        public object ConvertValue(Type type, string value)
        {
            if (UdiParser.TryParse(value, out GuidUdi udi) == true && udi.Guid.Equals(Guid.Empty) == false)
            {
#if NET472
                return _publishedSnapshotAccessor.GetRequiredPublishedSnapshot()?.Members.GetByProviderKey(udi.Guid);
#else
                var member = _memberService.GetByKey(udi.Guid);
                if (member != null)
                {
                    return _publishedSnapshotAccessor.GetRequiredPublishedSnapshot()?.Members.Get(_memberService.GetByKey(udi.Guid));
                }
#endif
            }

            return default(IPublishedContent);
        }

        private IMemberType GetMemberType(Dictionary<string, object> config)
        {
            if (config.TryGetValueAs("memberType", out JArray array) == true &&
               array.Count > 0 &&
               array[0].Value<string>() is string str &&
               string.IsNullOrWhiteSpace(str) == false &&
               UdiParser.TryParse(str, out GuidUdi udi) == true)
            {
                return _memberTypeService.Get(udi.Guid);
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
