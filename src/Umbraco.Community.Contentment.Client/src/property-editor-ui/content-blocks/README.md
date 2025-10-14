# Content Blocks - Umbraco v16 Implementation

This directory contains the Umbraco v16 implementation of the Content Blocks property editor using Lit web components.

## Overview

The Content Blocks property editor allows editors to create and manage blocks of content using configured element types. This implementation provides a simplified but functional approach to content block management.

## Architecture

### Components

#### `content-blocks.element.ts`
The main property editor UI component that:
- Displays a list of created content blocks
- Provides add/edit/delete operations
- Manages block state and configuration
- Enforces max items limit

#### `content-block-selection-modal.element.ts`
Modal for selecting an element type when creating a new block:
- Shows configured element types
- Includes search/filter functionality
- Opens workspace modal after selection

#### `content-block-workspace-modal.element.ts`
Modal for creating/editing individual blocks with full property editing:
- Uses `UmbContentTypeStructureManager` to load element type structure
- Loads all properties including from element type compositions
- Renders properties using `umb-property-layout` and `umb-property` elements
- Manages property values through `umb-property-dataset` context
- Handles property value changes and persistence
- Provides loading states and error handling

### Data Structures

#### `ContentBlockType`
Configuration for an element type that can be used as a block:
```typescript
{
  alias: string;          // Element type alias
  description?: string;   // Optional description
  icon?: string;          // Optional icon
  name: string;           // Display name
  key: string;            // Unique identifier (GUID)
  nameTemplate?: string;  // Template for block names
  overlaySize?: string;   // Modal size preference
  previewEnabled?: boolean; // Whether preview is enabled
}
```

#### `ContentBlock`
Individual block instance:
```typescript
{
  elementType: string;            // GUID of the element type
  key: string;                    // Unique block instance ID
  value: Record<string, unknown>; // Property values
}
```

## Configuration

The property editor accepts the following configuration:

- **contentBlockTypes**: Array of configured element types (managed via backend C# code)
- **maxItems**: Maximum number of blocks allowed (0 = unlimited)
- **enableFilter**: Whether to show search filter in selection modal
- **disableSorting**: Whether sorting is disabled
- **enableDevMode**: Whether developer mode is enabled

## Implementation Status

### Completed ✓
- Modal structure and token registration
- Element type selection with filtering
- Block CRUD operations (Create, Read, Update, Delete)
- State management and change detection
- Configuration parsing
- Type definitions
- **Property Editing**: Full property editing using UmbContentTypeStructureManager
  - Loads element type structure including compositions
  - Renders all properties with appropriate property editors
  - Handles property value changes through property dataset
  - Supports element type compositions (inherited properties)
  - Loading states and error handling

### Not Implemented
- Block sorting/reordering (drag-and-drop)
- Preview rendering
- Name template evaluation (currently uses simple template replacement)
- Clipboard operations (copy/paste blocks)
- Dev mode property action

## Usage

The property editor is registered with the manifest system and can be configured as a data type in Umbraco. Element types must be configured through the data type configuration.

## Future Enhancements

The core property editing functionality is now complete. Potential enhancements include:

1. **Block Sorting**: Add drag-and-drop reordering of blocks
2. **Preview Rendering**: Implement block preview using configured view paths
3. **Name Template**: Implement full AngularJS expression evaluation for block names
4. **Clipboard Support**: Add copy/paste functionality for blocks
5. **Dev Mode**: Add property action to edit raw JSON data
6. **Validation**: Add property-level validation feedback

## Design Decisions

### Implementation Philosophy

This implementation provides **complete property editing functionality** using Umbraco's standard content type and property systems:

**Key Technical Decisions:**
- Uses `UmbContentTypeStructureManager` for type-safe element type loading
- Leverages `umb-property-dataset` for consistent property state management
- Implements proper composition support (inherited properties)
- Follows Umbraco v16 patterns for modal-based editing

**Why This Approach:**
1. **Standards-Based**: Uses Umbraco's built-in property system
2. **Maintainable**: Leverages framework capabilities rather than custom implementations
3. **Composition Support**: Automatically handles element type inheritance
4. **Future-Proof**: Built on stable Umbraco APIs

This approach provides a production-ready implementation that:
- Solves the immediate need (Content Blocks in v16)
- Uses documented, supported APIs
- Provides room for future enhancements
- Maintains consistency with Umbraco's architecture

## Testing

To test this implementation:

1. Configure a Content Blocks data type in Umbraco
2. Add element types to the configuration
3. Add the property to a document type
4. Create content and test:
   - Adding blocks
   - Editing blocks with properties
   - Property value persistence
   - Deleting blocks
   - Max items limit
   - Element type selection with multiple types
   - Element type compositions (inherited properties)

## Contributing

When enhancing this implementation, consider:
- Follow Umbraco v16 patterns and practices
- Maintain backward compatibility with saved data
- Update documentation with implementation status
- Add tests for new functionality
- Consider performance implications of property loading
