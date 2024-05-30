namespace Umbraco.Cms.Core.PropertyEditors;

// TODO: [LK] This is a temporary class to allow the project to build.
// I'll review it later. [LK]
public class ContentmentConfigurationField
{
    public string? Key { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }

    // TODO: [LK] Deprecate this; migrate to use `PropertyEditorUiAlias`.
    public string? View { get; set; }
    public string? PropertyEditorUiAlias { get; set; }

    public IDictionary<string, object>? Config { get; set; }

    //public bool HideLabel { get; set; }
}
