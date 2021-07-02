/* Copyright © 2021 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Core;
using Umbraco.Core.IO;
using Umbraco.Core.PropertyEditors;
using UmbConstants = Umbraco.Core.Constants;

namespace Umbraco.Community.Contentment.DataEditors
{
    [Core.Composing.HideFromTypeFinder]
    public sealed class NumberRangeDataListSource : IDataListSource, IDataListSourceValueConverter
    {
        public string Name => "Number Range";

        public string Description => "Generates a sequence of numbers within a specified range.";

        public string Icon => "icon-fa fa-sort-numeric-asc";

        public string Group => default;

        public IEnumerable<ConfigurationField> Fields => new[]
        {
            new ConfigurationField
            {
                Key = "start",
                Name = "Start",
                Description = "The value of the first number in the sequence.",
                View = IOHelper.ResolveUrl(NumberInputDataEditor.DataEditorViewPath),
                Config = new Dictionary<string, object>
                {
                    { "step", 0.1D },
                    { UmbConstants.PropertyEditors.ConfigurationKeys.DataValueType, "DECIMAL" }
                },
            },
            new ConfigurationField
            {
                Key = "end",
                Name = "End",
                Description = "The value of the last number in the sequence.",
                View = IOHelper.ResolveUrl(NumberInputDataEditor.DataEditorViewPath),
                Config = new Dictionary<string, object>
                {
                    { "step", 0.1D },
                    { UmbConstants.PropertyEditors.ConfigurationKeys.DataValueType, "DECIMAL" }
                },
            },

            new ConfigurationField
            {
                Key = "step",
                Name = "Step",
                Description = "The number of steps between each number.",
                View = IOHelper.ResolveUrl(NumberInputDataEditor.DataEditorViewPath),
                Config = new Dictionary<string, object>
                {
                    { "step", 0.1D },
                    { UmbConstants.PropertyEditors.ConfigurationKeys.DataValueType, "DECIMAL" }
                },
            },
        };

        public Dictionary<string, object> DefaultValues => new Dictionary<string, object>
        {
            { "start", 1 },
            { "end", 10 },
            { "step", 1 },
        };

        public OverlaySize OverlaySize => OverlaySize.Small;

        public IEnumerable<DataListItem> GetItems(Dictionary<string, object> config)
        {
            var start = config.GetValueAs("start", defaultValue: default(double));
            var end = config.GetValueAs("end", defaultValue: default(double));
            var step = config.GetValueAs("step", defaultValue: default(double));

            return GetRange(start, end, step).Select(x => new DataListItem { Name = x.ToString(), Value = x.ToString() });
        }

        public Type GetValueType(Dictionary<string, object> config) => typeof(double);

        public object ConvertValue(Type type, string value) => value.TryConvertTo(type).ResultOr(default(double));

        private IEnumerable<double> GetRange(double start, double end, double step)
        {
            if (step <= default(double))
            {
                step = step == default ? 1D : -step;
            }

            if (start <= end)
            {
                for (var i = start; i <= end; i += step)
                {
                    yield return i;
                }
            }
            else
            {
                for (var i = start; i >= end; i -= step)
                {
                    yield return i;
                }
            }
        }
    }
}
