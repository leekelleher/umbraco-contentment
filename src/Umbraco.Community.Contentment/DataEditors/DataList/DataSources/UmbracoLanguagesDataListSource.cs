/* Copyright © 2021 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Services;

namespace Umbraco.Community.Contentment.DataEditors
{
    public sealed class UmbracoLanguagesDataListSource : DataListToDataPickerSourceBridge, IContentmentDataSource
    {
        private readonly ILanguageService _languageService;

        public UmbracoLanguagesDataListSource(ILanguageService languageService)
        {
            _languageService = languageService;
        }

        public override string Name => "Umbraco Languages";

        public override string Description => "Populate the data source with languages configured in Umbraco.";

        public override string Icon => UmbConstants.Icons.Language;

        public override string Group => Constants.Conventions.DataSourceGroups.Umbraco;

        public override IEnumerable<ContentmentConfigurationField> Fields => Enumerable.Empty<ContentmentConfigurationField>();

        public override Dictionary<string, object>? DefaultValues => default;

        public override OverlaySize OverlaySize => OverlaySize.Small;

        public override IEnumerable<DataListItem> GetItems(Dictionary<string, object> config)
        {
            return _languageService.GetAllAsync().GetAwaiter().GetResult()
                .Select(x => new DataListItem
                {
                    Name = x.CultureName,
                    Value = x.IsoCode,
                    Icon = UmbConstants.Icons.Language,
                    Description = x.IsoCode,
                });
        }
    }
}
