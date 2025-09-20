// SPDX-License-Identifier: MPL-2.0
// Copyright © 2025 Lee Kelleher

using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Services;
using Umbraco.Extensions;

namespace Umbraco.Community.Contentment.DataEditors;

public sealed class UmbracoDataTypesDataListSource : DataListToDataPickerSourceBridge, IContentmentDataSource, IDataSourceValueConverter
{
    private readonly IDataTypeService _dataTypeService;

    public UmbracoDataTypesDataListSource(IDataTypeService dataTypeService)
    {
        _dataTypeService = dataTypeService;
    }

    public override string Name => "Umbraco Data Types";

    public override string Description => "Populate the data source with Data Types.";

    public override string Icon => UmbConstants.Icons.DataType;

    public override string Group => Constants.Conventions.DataSourceGroups.Umbraco;

    public override IEnumerable<ContentmentConfigurationField> Fields =>
    [
        new ContentmentConfigurationField
        {
            Key = "editorAlias",
            Name = "Editor alias",
            Description = "Enter the schema alias for the data types of a specific property editor, or leave empty to get all data types.",
            PropertyEditorUiAlias = "Umb.PropertyEditorUi.TextBox",
        }
    ];

    public override Dictionary<string, object>? DefaultValues => default;

    public override OverlaySize OverlaySize => OverlaySize.Small;

    public override IEnumerable<DataListItem> GetItems(Dictionary<string, object> config)
    {
        DataListItem mapDataType(IDataType dataType) => new()
        {
            Name = dataType.Name ?? dataType.EditorAlias,
            Value = Udi.Create(UmbConstants.UdiEntityType.DataType, dataType.Key).ToString(),
            Icon = this.Icon,
            Description = $"{dataType.EditorUiAlias} | {dataType.EditorAlias}",
        };

        if (config.TryGetValueAs("editorAlias", out string? editorAlias) == true &&
            string.IsNullOrWhiteSpace(editorAlias) == false)
        {
            return _dataTypeService
                .GetByEditorAliasAsync(editorAlias).GetAwaiter().GetResult()?
                .Select(mapDataType) ?? Enumerable.Empty<DataListItem>();
        }

        return _dataTypeService.GetAllAsync().GetAwaiter().GetResult().Select(mapDataType);
    }

    public Type? GetValueType(Dictionary<string, object>? config) => typeof(IDataType);

    public object? ConvertValue(Type type, string value)
    {
        return UdiParser.TryParse(value, out GuidUdi? udi) == true && udi?.Guid.Equals(Guid.Empty) == false
            ? _dataTypeService.GetAsync(udi.Guid).GetAwaiter().GetResult()
            : default;
    }
}
