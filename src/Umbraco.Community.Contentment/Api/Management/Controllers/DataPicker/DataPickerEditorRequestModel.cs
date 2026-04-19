// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

namespace Umbraco.Community.Contentment.Api.Management;

public sealed class DataPickerEditorRequestModel
{
    public string? Alias { get; set; }

    public ConfigurationEditorItemRequestModel? DataSource { get; set; }

    public Guid DataTypeKey { get; set; }

    public ConfigurationEditorItemRequestModel? DisplayMode { get; set; }

    public required string Id { get; set; }

    public bool? IsNew { get; set; }

    public string? ParentId { get; set; }

    public IEnumerable<string>? Values { get; set; }

    public string? Variant { get; set; }
}
