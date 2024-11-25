// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

namespace Umbraco.Community.Contentment.Api.Management;

public sealed class DataPickerEditorRequestModel
{
    public required string Id { get; set; }

    public Guid DataTypeKey { get; set; }

    public IEnumerable<ConfigurationEditorItemRequestModel>? DataSource { get; set; }

    public IEnumerable<ConfigurationEditorItemRequestModel>? DisplayMode { get; set; }

    public IEnumerable<string>? Values { get; set; }
}
