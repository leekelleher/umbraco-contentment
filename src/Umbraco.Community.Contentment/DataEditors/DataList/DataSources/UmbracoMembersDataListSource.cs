/* Copyright © 2021 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using Umbraco.Core;
using Umbraco.Core.IO;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Core.PropertyEditors;
using Umbraco.Core.Services;
using Umbraco.Web.PublishedCache;
using UmbConstants = Umbraco.Core.Constants;

namespace Umbraco.Community.Contentment.DataEditors
{
    public sealed class UmbracoMembersDataListSource : IDataListSource, IDataListSourceValueConverter
    {
        private readonly IMemberTypeService _memberTypeService;
        private readonly IMemberService _memberService;
        private readonly IPublishedSnapshotAccessor _publishedSnapshotAccessor;

        public UmbracoMembersDataListSource(IMemberTypeService memberTypeService, IMemberService memberService, IPublishedSnapshotAccessor publishedSnapshotAccessor)
        {
            _memberTypeService = memberTypeService;
            _memberService = memberService;
            _publishedSnapshotAccessor = publishedSnapshotAccessor;
        }

        public string Name => "Umbraco Members";

        public string Description => "Populate a data source with Umbraco members.";

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
                    new NotesConfigurationField($@"<details class=""alert alert-danger"">
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
                        View = ItemPickerDataListEditor.DataEditorViewPath,
                        Config = new Dictionary<string, object>
                        {
                            { "addButtonLabelKey", "defaultdialogs_selectMemberType" },
                            { "enableFilter", items.Count > 5 ? Constants.Values.True : Constants.Values.False },
                            { "items", items },
                            { "listType", "list" },
                            { "overlayView", IOHelper.ResolveUrl(ItemPickerDataListEditor.DataEditorOverlayViewPath) },
                            { "maxItems", 1 },
                        }
                    }
                };
            }
        }

        public Dictionary<string, object> DefaultValues => default;

        public OverlaySize OverlaySize => OverlaySize.Small;

        public IEnumerable<DataListItem> GetItems(Dictionary<string, object> config)
        {
            DataListItem mapMember(IMember member)
            {
                var guidUdi = Udi.Create(UmbConstants.UdiEntityType.Member, member.Key).ToString();
                return new DataListItem
                {
                    Name = member.Name,
                    Value = guidUdi,
                    Icon = member.ContentType.Icon ?? UmbConstants.Icons.Member,
                    Description = guidUdi,
                };
            };

            if (config.TryGetValueAs("memberType", out JArray array) == true &&
                array.Count > 0 &&
                array[0].Value<string>() is string str &&
                string.IsNullOrWhiteSpace(str) == false &&
                GuidUdi.TryParse(str, out var udi) == true)
            {
                var memberType = _memberTypeService.Get(udi.Guid);
                if (memberType != null)
                {
                    return _memberService.GetMembersByMemberType(memberType.Id).Select(mapMember);
                }
            }
            else
            {
                return _memberService.GetAllMembers().Select(mapMember);
            }

            return Enumerable.Empty<DataListItem>();
        }

        public Type GetValueType(Dictionary<string, object> config) => typeof(IPublishedContent);

        public object ConvertValue(Type type, string value)
        {
            return GuidUdi.TryParse(value, out var udi) == true && udi.Guid.Equals(Guid.Empty) == false
                ? _publishedSnapshotAccessor.PublishedSnapshot.Members.GetByProviderKey(udi.Guid)
                : default;
        }
    }
}
