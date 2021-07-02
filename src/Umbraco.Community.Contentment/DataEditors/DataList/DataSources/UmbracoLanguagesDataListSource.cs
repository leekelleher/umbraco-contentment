/* Copyright © 2021 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Collections.Generic;
using System.Linq;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Services;
using UmbConstants = Umbraco.Cms.Core.Constants;

namespace Umbraco.Community.Contentment.DataEditors
{
    public sealed class UmbracoLanguagesDataListSource : IDataListSource
    {
        private readonly ILocalizationService _localizationService;

        public UmbracoLanguagesDataListSource(ILocalizationService localizationService)
        {
            _localizationService = localizationService;
        }

        public string Name => "Umbraco Languages";

        public string Description => "Populate the data source with langauges configured in Umbraco.";

        public string Icon => UmbConstants.Icons.Language;

        public string Group => Constants.Conventions.DataSourceGroups.Umbraco;

        public IEnumerable<ConfigurationField> Fields => default;

        public Dictionary<string, object> DefaultValues => default;

        public OverlaySize OverlaySize => OverlaySize.Small;

        public IEnumerable<DataListItem> GetItems(Dictionary<string, object> config)
        {
            return _localizationService
                .GetAllLanguages()
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
