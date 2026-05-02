using Umbraco.Cms.Infrastructure.Migrations.Upgrade.V_15_0_0.LocalLinks;
using Umbraco.Community.Contentment.DataEditors;

namespace Umbraco.Community.Contentment.Migrations.Upgrade.V_6_1_3;

/// <summary>
/// Handles the processing and migration of local links within Content Blocks
/// as part of the Umbraco upgrade process to version 15.0.0.
/// </summary>
/// <remarks>
/// Content Blocks stores its data as a JSON array of <see cref="ContentBlock"/> objects,
/// where each block's <see cref="ContentBlock.Value"/> dictionary contains the property
/// values for that block's element type. This processor iterates through all blocks and
/// their property values, delegating to the appropriate nested processor (e.g. RTE, Block List)
/// for each property value.
/// </remarks>
[Obsolete("To be removed in Contentment 7.0, as `ITypedLocalLinkProcessor` will be removed in Umbraco 18.0.")]
public sealed class ContentBlocksLocalLinkProcessor : ITypedLocalLinkProcessor
{
    /// <summary>
    /// Gets the type of the property editor value, which is <see cref="List{ContentBlock}"/>.
    /// This matches the runtime type returned by <see cref="ContentBlocksDataValueEditor.ToEditor"/>
    /// when deserializing the Content Blocks JSON value.
    /// </summary>
    public Type PropertyEditorValueType => typeof(List<ContentBlock>);

    /// <summary>
    /// Gets the collection of property editor aliases that this processor supports.
    /// </summary>
    public IEnumerable<string> PropertyEditorAliases => [ContentBlocksDataEditor.DataEditorAlias];

    /// <summary>
    /// Gets a function that processes Content Blocks data containing local links.
    /// </summary>
    public Func<object?, Func<object?, bool>, Func<string, string>, bool> Process => ProcessContentBlocks;

    private static bool ProcessContentBlocks(
        object? value,
        Func<object?, bool> processNested,
        Func<string, string> processStringValue)
    {
        if (value is not IEnumerable<ContentBlock> blocks)
        {
            return false;
        }

        var hasChanged = false;

        foreach (var block in blocks)
        {
            foreach (var property in block.Value)
            {
                if (processNested(property.Value))
                {
                    hasChanged = true;
                }
            }
        }

        return hasChanged;
    }
}
