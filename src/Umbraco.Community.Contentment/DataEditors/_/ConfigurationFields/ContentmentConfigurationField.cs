namespace Umbraco.Cms.Core.PropertyEditors;

// TODO: [LK] This is a temporary class to allow the project to build.
// I'll review it later. [LK]
public class ContentmentConfigurationField : ConfigurationField
{
    public string? Name { get; set; }

    public string? Description { get; set; }

    public string? View { get; set; }

    public bool HideLabel { get; set; }

    // TODO: [LK] Add `PropertyEditorUiAlias` property.
}
