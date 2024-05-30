// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

using Umbraco.Community.Contentment.DataEditors;

namespace Umbraco.Cms.Api.Management.Models.Contentment;

public class ConfigurationEditorModelsResponseModel
{
    public required IEnumerable<ConfigurationEditorModel> Items { get; set; }
}
