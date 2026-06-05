// SPDX-License-Identifier: MIT
// Copyright © 2025 Lee Kelleher

namespace Umbraco.Community.Contentment.DataEditors;

internal sealed class InputListValueModel : List<InputListRowModel>
{ }

internal sealed class InputListRowModel
{
    public Guid Key { get; init; } = Guid.Empty;

    public List<InputListItemValueModel> Values { get; set; } = [];
}

internal sealed class InputListItemValueModel
{
    public Guid Key { get; init; } = Guid.Empty;

    public Guid DataType { get; init; } = UmbConstants.DataTypes.Guids.LabelStringGuid;

    public object? Value { get; set; }
}
