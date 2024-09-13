// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

using Umbraco.Cms.Api.Management.ViewModels.DataType;

namespace Umbraco.Cms.Api.Management.Models.Contentment;

public sealed class DataPickerEditorResponseModel
{
    public string? PropertyEditorUiAlias { get; set; }

    public IEnumerable<DataTypePropertyPresentationModel>? Config { get; set; }
}
