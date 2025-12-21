// SPDX-License-Identifier: MIT
// Copyright © 2025 Lee Kelleher

using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Serialization;
using Umbraco.Cms.Core.Strings;

namespace Umbraco.Community.Contentment.DataEditors;

public sealed class InputListDataEditor : IDataEditor
{
    internal const string DataEditorAlias = Constants.Internals.DataEditorAliasPrefix + "InputList";
    internal const string DataEditorName = Constants.Internals.DataEditorNamePrefix + "Input List";
    internal const string DataEditorIcon = "icon-fa-list";
    internal const string DataEditorUiAlias = Constants.Internals.DataEditorUiAliasPrefix + "InputList";

    private readonly IShortStringHelper _shortStringHelper;
    private readonly IJsonSerializer _jsonSerializer;

    public InputListDataEditor(
        IJsonSerializer jsonSerializer,
        IShortStringHelper shortStringHelper)
    {
        _jsonSerializer = jsonSerializer;
        _shortStringHelper = shortStringHelper;
    }

    public string Alias => DataEditorAlias;

    public string Name => DataEditorName;

    public string Icon => DataEditorIcon;

    public string Group => UmbConstants.PropertyEditors.Groups.Lists;

    public bool IsDeprecated => false;

    public IDictionary<string, object>? DefaultConfiguration => default;

    public IPropertyIndexValueFactory PropertyIndexValueFactory => new DefaultPropertyIndexValueFactory();

    public IConfigurationEditor GetConfigurationEditor() => new ConfigurationEditor();

    public IDataValueEditor GetValueEditor()
    {
        return new DataValueEditor(_shortStringHelper, _jsonSerializer)
        {
            ValueType = ValueTypes.Json,
        };
    }

    public IDataValueEditor GetValueEditor(object? configuration)
    {
        return new DataValueEditor(_shortStringHelper, _jsonSerializer)
        {
            ConfigurationObject = configuration,
            ValueType = ValueTypes.Json,
        };
    }
}
