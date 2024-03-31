///* Copyright © 2023 Lee Kelleher.
// * This Source Code Form is subject to the terms of the Mozilla Public
// * License, v. 2.0. If a copy of the MPL was not distributed with this
// * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

//using Umbraco.Cms.Core.PropertyEditors;
//using Umbraco.Cms.Core.Sections;
//using Umbraco.Cms.Core.Services;
//using Umbraco.Extensions;

//namespace Umbraco.Community.Contentment.DataEditors
//{
//    public sealed class UmbracoBackofficeSectionsDataSource : DataListToDataPickerSourceBridge, IDataListSource, IDataSourceValueConverter
//    {
//        private readonly ISectionService _sectionService;

//        public UmbracoBackofficeSectionsDataSource(ISectionService sectionService)
//        {
//            _sectionService = sectionService;
//        }

//        public override string Name => "Umbraco Backoffice Sections";

//        public override string Description => "Use the backoffice sections to populate the data source.";

//        public override string Icon => "icon-section";

//        public override string Group => Constants.Conventions.DataSourceGroups.Umbraco;

//        public override IEnumerable<ConfigurationField> Fields => Enumerable.Empty<ConfigurationField>();

//        public override Dictionary<string, object>? DefaultValues => default;

//        public override OverlaySize OverlaySize => OverlaySize.Small;

//        public override IEnumerable<DataListItem> GetItems(Dictionary<string, object> config)
//        {
//            return _sectionService
//                .GetSections()
//                .Select(x => new DataListItem
//                {
//                    Name = x.Name,
//                    Value = x.Alias,
//                    Icon = Icon,
//                });
//        }

//        public Type? GetValueType(Dictionary<string, object>? config) => typeof(ISection);

//        public object? ConvertValue(Type type, string value) => _sectionService.GetByAlias(value);
//    }
//}
