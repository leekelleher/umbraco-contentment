/* Copyright © 2021 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Extensions;

namespace Umbraco.Community.Contentment.DataEditors
{
    public sealed class NumberRangeDataListSource : IDataListSource, IDataSourceValueConverter
    {
        private readonly IIOHelper _ioHelper;

        public NumberRangeDataListSource(IIOHelper ioHelper)
        {
            _ioHelper = ioHelper;
        }

        public string Name => "Number Range";

        public string Description => "Generates a sequence of numbers within a specified range.";

        public string Icon => "icon-fa fa-sort-numeric-asc";

        public string Group => Constants.Conventions.DataSourceGroups.Data;

        public IEnumerable<ConfigurationField> Fields => new[]
        {
            new ConfigurationField
            {
                Key = "start",
                Name = "Start",
                Description = "The value of the first number in the sequence.",
                View = _ioHelper.ResolveRelativeOrVirtualUrl(NumberInputDataEditor.DataEditorViewPath),
                Config = new Dictionary<string, object>
                {
                    { "step", 0.1D },
                    { UmbConstants.PropertyEditors.ConfigurationKeys.DataValueType, nameof(Decimal).ToUpper() }
                },
            },
            new ConfigurationField
            {
                Key = "end",
                Name = "End",
                Description = "The value of the last number in the sequence.",
                View = _ioHelper.ResolveRelativeOrVirtualUrl(NumberInputDataEditor.DataEditorViewPath),
                Config = new Dictionary<string, object>
                {
                    { "step", 0.1D },
                    { UmbConstants.PropertyEditors.ConfigurationKeys.DataValueType, nameof(Decimal).ToUpper() }
                },
            },
            new ConfigurationField
            {
                Key = "step",
                Name = "Step",
                Description = "The number of steps between each number.",
                View = _ioHelper.ResolveRelativeOrVirtualUrl(NumberInputDataEditor.DataEditorViewPath),
                Config = new Dictionary<string, object>
                {
                    { "step", 0.1D },
                    { UmbConstants.PropertyEditors.ConfigurationKeys.DataValueType, nameof(Decimal).ToUpper() }
                },
            },
            new ConfigurationField
            {
                Key = "decimals",
                Name = "Decimal places",
                Description = "How many decimal places would you like?",
                View = _ioHelper.ResolveRelativeOrVirtualUrl("~/umbraco/views/propertyeditors/slider/slider.html"),
                Config = new Dictionary<string, object>
                {
                    { "initVal1", 0 },
                    { "minVal", 0 },
                    { "maxVal", 10 },
                    { "step", 1 }
                }
            }
        };

        public Dictionary<string, object> DefaultValues => new Dictionary<string, object>
        {
            { "start", 1 },
            { "end", 10 },
            { "step", 1 },
            { "decimals", 0 },
        };

        public OverlaySize OverlaySize => OverlaySize.Small;

        public IEnumerable<DataListItem> GetItems(Dictionary<string, object> config)
        {
            var start = config.GetValueAs("start", defaultValue: default(double));
            var end = config.GetValueAs("end", defaultValue: default(double));
            var step = config.GetValueAs("step", defaultValue: default(double));
            var decimals = config.GetValueAs("decimals", defaultValue: default(int));
            var format = string.Concat("N", decimals);

            DataListItem newItem(double i) => new DataListItem { Name = i.ToString(format), Value = i.ToString(format) };

            if (step <= default(double))
            {
                step = step == default ? 1D : -step;
            }

            if (start <= end)
            {
                for (var i = start; i <= end; i += step)
                {
                    yield return newItem(i);
                }
            }
            else
            {
                for (var i = start; i >= end; i -= step)
                {
                    yield return newItem(i);
                }
            }
        }

        public Type GetValueType(Dictionary<string, object> config) => typeof(double);

        public object ConvertValue(Type type, string value) => value.TryConvertTo(type).ResultOr(default(double));
    }
}
