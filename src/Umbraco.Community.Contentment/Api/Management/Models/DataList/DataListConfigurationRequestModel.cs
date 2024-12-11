// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

namespace Umbraco.Community.Contentment.Api.Management;

public sealed class DataListConfigurationRequestModel
{
    public ConfigurationEditorItemRequestModel? DataSource { get; set; }

    public ConfigurationEditorItemRequestModel? ListEditor { get; set; }
}
