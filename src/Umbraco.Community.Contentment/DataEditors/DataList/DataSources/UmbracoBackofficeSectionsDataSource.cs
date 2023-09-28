/* Copyright © 2023 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Sections;
using Umbraco.Cms.Core.Services;
using Umbraco.Extensions;

namespace Umbraco.Community.Contentment.DataEditors
{
    public sealed class UmbracoBackofficeSectionsDataSource : IDataListSource, IDataSourceValueConverter
    {
        private readonly ISectionService _sectionService;

        public UmbracoBackofficeSectionsDataSource(ISectionService sectionService)
        {
            _sectionService = sectionService;
        }

        public string Name => "Umbraco Backoffice Sections";

        public string Description => "Use the backoffice sections to populate the data source.";

        public string Icon => "icon-section";

        public string Group => Constants.Conventions.DataSourceGroups.Umbraco;

        public IEnumerable<ConfigurationField> Fields => default;

        public Dictionary<string, object> DefaultValues => default;

        public OverlaySize OverlaySize => OverlaySize.Small;

        public IEnumerable<DataListItem> GetItems(Dictionary<string, object> config)
        {
            return _sectionService
                .GetSections()
                .Select(x => new DataListItem
                {
                    Name = x.Name,
                    Value = x.Alias,
                    Icon = Icon,
                });
        }

        public Type GetValueType(Dictionary<string, object> config) => typeof(ISection);

        public object ConvertValue(Type type, string value) => _sectionService.GetByAlias(value);
    }
}
