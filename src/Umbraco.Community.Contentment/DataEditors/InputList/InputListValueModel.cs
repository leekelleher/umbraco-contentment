namespace Umbraco.Community.Contentment.DataEditors;

internal sealed class InputListValueModel
{
    public Guid Alias { get; init; } = Guid.Empty;

    public object? Value { get; set; }
}
