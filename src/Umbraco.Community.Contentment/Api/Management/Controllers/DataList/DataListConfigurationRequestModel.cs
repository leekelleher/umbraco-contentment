// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

namespace Umbraco.Community.Contentment.Api.Management;

public sealed class DataListConfigurationRequestModel
{
    public string? Alias { get; set; }

    public Guid? ContentTypeKey { get; set; }

    public ConfigurationEditorItemRequestModel? DataSource { get; set; }

    // When set, the configuration is read from the saved Data Type, (the `DataSource` and `ListEditor` are ignored).
    public Guid? DataTypeKey { get; set; }

    public string? Id { get; set; }

    public bool? IsNew { get; set; }

    public ConfigurationEditorItemRequestModel? ListEditor { get; set; }

    public string? ParentId { get; set; }

    public string? Variant { get; set; }
}
