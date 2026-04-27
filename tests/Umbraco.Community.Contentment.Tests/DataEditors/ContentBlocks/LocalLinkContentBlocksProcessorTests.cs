using System.Text.Json;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Models.Blocks;
using Umbraco.Cms.Core.Models.Editors;
using Umbraco.Community.Contentment.DataEditors;
using Xunit;

namespace Umbraco.Community.Contentment.Tests.DataEditors.ContentBlocks;

public class LocalLinkContentBlocksProcessorTests
{
    private readonly LocalLinkContentBlocksProcessor _processor;

    public LocalLinkContentBlocksProcessorTests()
    {
#pragma warning disable CS0618 // Type or member is obsolete
        _processor = new LocalLinkContentBlocksProcessor();
#pragma warning restore CS0618
    }

    [Fact]
    public void PropertyEditorValueType_Is_List_Of_ContentBlock()
    {
        Assert.Equal(typeof(List<ContentBlock>), _processor.PropertyEditorValueType);
    }

    [Fact]
    public void PropertyEditorAliases_Contains_ContentBlocks_Alias()
    {
        var aliases = _processor.PropertyEditorAliases.ToList();

        Assert.Single(aliases);
        Assert.Equal("Umbraco.Community.Contentment.ContentBlocks", aliases[0]);
    }

    [Fact]
    public void Process_Returns_False_When_Value_Is_Null()
    {
        var result = _processor.Process(null, _ => true, s => s);

        Assert.False(result);
    }

    [Fact]
    public void Process_Returns_False_When_Value_Is_Wrong_Type()
    {
        var result = _processor.Process("not a content block list", _ => true, s => s);

        Assert.False(result);
    }

    [Fact]
    public void Process_Returns_False_When_Blocks_Are_Empty()
    {
        var blocks = new List<ContentBlock>();

        var result = _processor.Process(blocks, _ => true, s => s);

        Assert.False(result);
    }

    [Fact]
    public void Process_Returns_False_When_No_Values_Changed()
    {
        var blocks = new List<ContentBlock>
        {
            CreateBlock(("richText", "some html content")),
        };

        var result = _processor.Process(blocks, _ => false, s => s);

        Assert.False(result);
    }

    [Fact]
    public void Process_Returns_True_When_Nested_Value_Changed()
    {
        var blocks = new List<ContentBlock>
        {
            CreateBlock(("richText", "html with local link")),
        };

        var result = _processor.Process(blocks, _ => true, s => s);

        Assert.True(result);
    }

    [Fact]
    public void Process_Calls_ProcessNested_For_Each_Property_Value()
    {
        var processedValues = new List<object?>();

        var blocks = new List<ContentBlock>
        {
            CreateBlock(
                ("richText", "first rte value"),
                ("title", "a title")),
            CreateBlock(
                ("body", "second rte value")),
        };

        _processor.Process(
            blocks,
            value =>
            {
                processedValues.Add(value);
                return false;
            },
            s => s);

        Assert.Equal(3, processedValues.Count);
        Assert.Equal("first rte value", processedValues[0]);
        Assert.Equal("a title", processedValues[1]);
        Assert.Equal("second rte value", processedValues[2]);
    }

    [Fact]
    public void Process_Returns_True_When_Any_Value_In_Any_Block_Changed()
    {
        var blocks = new List<ContentBlock>
        {
            CreateBlock(("noChange", "unchanged")),
            CreateBlock(("willChange", "has local link")),
        };

        var result = _processor.Process(
            blocks,
            value => value is string s && s.Contains("local link"),
            s => s);

        Assert.True(result);
    }

    [Fact]
    public void Process_Handles_Null_Property_Values_In_Block()
    {
        var block = new ContentBlock
        {
            ElementType = Guid.NewGuid(),
            Key = Guid.NewGuid(),
            Value = new Dictionary<string, object?>(StringComparer.InvariantCultureIgnoreCase)
            {
                { "richText", null },
                { "title", "a value" },
            },
        };

        var processedValues = new List<object?>();

        _processor.Process(
            new List<ContentBlock> { block },
            value =>
            {
                processedValues.Add(value);
                return false;
            },
            s => s);

        // Null values should still be passed to processNested (it decides what to do)
        Assert.Equal(2, processedValues.Count);
        Assert.Null(processedValues[0]);
        Assert.Equal("a value", processedValues[1]);
    }

    [Fact]
    public void Process_Handles_RichTextEditorValue_Objects()
    {
        // When ToEditor is called, RTE properties get converted to RichTextEditorValue objects.
        // This test verifies processNested receives the actual typed object.
        var rteValue = new RichTextEditorValue
        {
            Markup = "<p>Hello <a href=\"/{localLink:umb://document/abc123}\">link</a></p>",
        };

        var block = new ContentBlock
        {
            ElementType = Guid.NewGuid(),
            Key = Guid.NewGuid(),
            Value = new Dictionary<string, object?>(StringComparer.InvariantCultureIgnoreCase)
            {
                { "richText", rteValue },
            },
        };

        object? receivedValue = null;

        _processor.Process(
            new List<ContentBlock> { block },
            value =>
            {
                receivedValue = value;
                return true;
            },
            s => s);

        Assert.IsType<RichTextEditorValue>(receivedValue);
        Assert.Same(rteValue, receivedValue);
    }

    [Fact]
    public void Process_Handles_String_Values_From_Unrecognized_Editors()
    {
        // When ToEditor can't find the property editor, ContentBlocksDataValueEditor
        // falls back to fakeProperty.GetValue()?.ToString() — a plain string.
        // processNested won't match any typed processor for a string, but it shouldn't crash.
        var blocks = new List<ContentBlock>
        {
            CreateBlock(("unknownEditor", "<p>some html with /{localLink:umb://document/123}</p>")),
        };

        var result = _processor.Process(
            blocks,
            value =>
            {
                // Simulates LocalLinkProcessor.ProcessToEditorValue returning false
                // because no typed processor matches typeof(string)
                return false;
            },
            s => s);

        Assert.False(result);
    }

    [Fact]
    public void Process_Handles_JsonElement_Values_From_Raw_Deserialization()
    {
        // When ContentBlock is deserialized but inner values haven't been through ToEditor
        // (e.g. element type not found), values may remain as JsonElement.
        var json = """{"markup": "<p>test</p>"}""";
        var jsonElement = JsonSerializer.Deserialize<JsonElement>(json);

        var block = new ContentBlock
        {
            ElementType = Guid.NewGuid(),
            Key = Guid.NewGuid(),
            Value = new Dictionary<string, object?>(StringComparer.InvariantCultureIgnoreCase)
            {
                { "richText", jsonElement },
            },
        };

        object? receivedValue = null;

        _processor.Process(
            new List<ContentBlock> { block },
            value =>
            {
                receivedValue = value;
                // JsonElement wouldn't match any processor type, so return false
                return false;
            },
            s => s);

        Assert.IsType<JsonElement>(receivedValue);
    }

    [Fact]
    public void Process_Works_With_IEnumerable_ContentBlock()
    {
        // Verify it works with any IEnumerable<ContentBlock>, not just List<ContentBlock>.
        // This covers the case where a different serializer returns an array.
        ContentBlock[] blocks =
        [
            CreateBlock(("richText", "some value")),
        ];

        var result = _processor.Process(blocks, _ => true, s => s);

        Assert.True(result);
    }

    [Fact]
    public void Process_Handles_Multiple_Blocks_With_Multiple_Properties()
    {
        var changedCount = 0;
        var blocks = new List<ContentBlock>
        {
            CreateBlock(
                ("heading", "My Heading"),
                ("body", "CHANGE_ME"),
                ("summary", "A summary")),
            CreateBlock(
                ("title", "CHANGE_ME"),
                ("content", "Some content")),
            CreateBlock(
                ("text", "CHANGE_ME")),
        };

        var result = _processor.Process(
            blocks,
            value =>
            {
                if (value is string s && s == "CHANGE_ME")
                {
                    changedCount++;
                    return true;
                }

                return false;
            },
            s => s);

        Assert.True(result);
        Assert.Equal(3, changedCount);
    }

    [Fact]
    public void Process_Handles_Block_With_Empty_Value_Dictionary()
    {
        var block = new ContentBlock
        {
            ElementType = Guid.NewGuid(),
            Key = Guid.NewGuid(),
            Value = new Dictionary<string, object?>(StringComparer.InvariantCultureIgnoreCase),
        };

        var result = _processor.Process(
            new List<ContentBlock> { block },
            _ => true,
            s => s);

        Assert.False(result);
    }

    [Fact]
    public void Process_Handles_BlockListValue_Nested_Inside_ContentBlock()
    {
        // A ContentBlock property could be a Block List, which would be converted
        // to BlockListValue by ToEditor before reaching the processor.
        var blockListValue = new BlockListValue { ContentData = [], SettingsData = [] };

        var block = new ContentBlock
        {
            ElementType = Guid.NewGuid(),
            Key = Guid.NewGuid(),
            Value = new Dictionary<string, object?>(StringComparer.InvariantCultureIgnoreCase)
            {
                { "blocks", blockListValue },
            },
        };

        object? receivedValue = null;

        _processor.Process(
            new List<ContentBlock> { block },
            value =>
            {
                receivedValue = value;
                return true;
            },
            s => s);

        Assert.IsType<BlockListValue>(receivedValue);
        Assert.Same(blockListValue, receivedValue);
    }

    private static ContentBlock CreateBlock(params (string alias, object? value)[] properties)
    {
        var block = new ContentBlock
        {
            ElementType = Guid.NewGuid(),
            Key = Guid.NewGuid(),
        };

        foreach (var (alias, value) in properties)
        {
            block.Value[alias] = value;
        }

        return block;
    }
}
