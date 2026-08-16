// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

using Umbraco.Cms.Api.Management.ViewModels.DataType;

namespace Umbraco.Community.Contentment.Api.Management;

public sealed class DataListEditorResponseModel
{
    public IEnumerable<DataTypePropertyPresentationModel>? Config { get; set; }

    public string? PropertyEditorUiAlias { get; set; }
}
