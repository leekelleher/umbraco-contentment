/* Copyright © 2021 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Serialization;
using Umbraco.Extensions;

namespace Umbraco.Community.Contentment.DataEditors
{
    public sealed class NumberRangeDataListSource : DataListToDataPickerSourceBridge, IContentmentDataSource, IDataSourceValueConverter
    {
        private readonly IJsonSerializer _jsonSerializer;

        public NumberRangeDataListSource(IJsonSerializer jsonSerializer)
        {
            _jsonSerializer = jsonSerializer;
        }

        public override string Name => "Number Range";

        public override string Description => "Generates a sequence of numbers within a specified range.";

        public override string Icon => "icon-fa-arrow-down-1-9";

        public override string Group => Constants.Conventions.DataSourceGroups.Data;

        public override IEnumerable<ContentmentConfigurationField> Fields => new[]
        {
            new ContentmentConfigurationField
            {
                Key = "start",
                Name = "Start",
                Description = "The value of the first number in the sequence.",
                PropertyEditorUiAlias = NumberInputDataEditor.DataEditorUiAlias,
                Config = new Dictionary<string, object>
                {
                    { "step", 0.1D },
                    { UmbConstants.PropertyEditors.ConfigurationKeys.DataValueType, nameof(Decimal).ToUpper() }
                },
            },
            new ContentmentConfigurationField
            {
                Key = "end",
                Name = "End",
                Description = "The value of the last number in the sequence.",
                PropertyEditorUiAlias = NumberInputDataEditor.DataEditorUiAlias,
                Config = new Dictionary<string, object>
                {
                    { "step", 0.1D },
                    { UmbConstants.PropertyEditors.ConfigurationKeys.DataValueType, nameof(Decimal).ToUpper() }
                },
            },
            new ContentmentConfigurationField
            {
                Key = "step",
                Name = "Step",
                Description = "The number of steps between each number.",
                PropertyEditorUiAlias = NumberInputDataEditor.DataEditorUiAlias,
                Config = new Dictionary<string, object>
                {
                    { "step", 0.1D },
                    { UmbConstants.PropertyEditors.ConfigurationKeys.DataValueType, nameof(Decimal).ToUpper() }
                },
            },
            new ContentmentConfigurationField
            {
                Key = "decimals",
                Name = "Decimal places",
                Description = "How many decimal places would you like?",
                PropertyEditorUiAlias = "Umb.PropertyEditorUi.Slider",
                Config = new Dictionary<string, object>
                {
                    { "initVal1", 0 },
                    { "minVal", 0 },
                    { "maxVal", 10 },
                    { "step", 1 }
                }
            }
        };

        public override Dictionary<string, object> DefaultValues => new()
        {
            { "start", 1 },
            { "end", 10 },
            { "step", 1 },
            { "decimals", 0 },
        };

        public override OverlaySize OverlaySize => OverlaySize.Small;

        private class SliderValue
        {
            public int? From { get; set; }
            public int? To { get; set; }
        }

        public override IEnumerable<DataListItem> GetItems(Dictionary<string, object> config)
        {
            var start = config.GetValueAs("start", defaultValue: default(double));
            var end = config.GetValueAs("end", defaultValue: default(double));
            var step = config.GetValueAs("step", defaultValue: default(double));

            var decimals = 0;
            var str = config.GetValueAs("decimals", defaultValue: default(string));
            if (string.IsNullOrWhiteSpace(str) == false)
            {
                decimals = int.TryParse(str, out var i) == true ? i : _jsonSerializer.Deserialize<SliderValue>(str)?.From ?? 0;
            }

            var format = string.Concat("N", decimals);

            DataListItem newItem(double i) => new() { Name = i.ToString(format), Value = i.ToString(format) };

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

        public Type? GetValueType(Dictionary<string, object>? config) => typeof(double);

        public object? ConvertValue(Type type, string value) => value.TryConvertTo(type).ResultOr(default(double));
    }
}
