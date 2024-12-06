// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

namespace Umbraco.Community.Contentment.DataEditors;

public interface IDataSourceDeliveryApiValueConverter : IDataSourceValueConverter
{
    Type? GetDeliveryApiValueType(Dictionary<string, object>? config);

    object? ConvertToDeliveryApiValue(Type type, string value, bool expanding = false);
}
