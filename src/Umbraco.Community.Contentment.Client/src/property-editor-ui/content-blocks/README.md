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
Modal for creating/editing individual blocks:
- Displays element type information
- Provides a foundation for property editing
- Currently shows element metadata and implementation notes

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
- Element type selection
- Block CRUD operations (Create, Read, Update, Delete)
- State management and change detection
- Configuration parsing
- Type definitions

### Simplified 
- **Property Editing**: The workspace modal currently shows element type information but does not render individual property editors. Implementing full property editing would require:
  - Fetching element type structure from Umbraco's content type repository
  - Rendering properties using `umb-property-dataset` and `umb-property` components
  - Handling property value serialization/deserialization
  - Supporting content variations and cultures
  - Property validation

### Not Implemented
- Block sorting/reordering
- Preview rendering
- Name template evaluation
- Clipboard operations
- Dev mode property action

## Usage

The property editor is registered with the manifest system and can be configured as a data type in Umbraco. Element types must be configured through the data type configuration.

## Future Enhancements

To implement full property editing in the workspace modal:

1. **Add Content Type Repository Integration**:
   ```typescript
   import { UMB_CONTENT_TYPE_REPOSITORY_CONTEXT } from '@umbraco-cms/backoffice/content-type';
   ```

2. **Fetch Element Structure**:
   ```typescript
   const repository = await this.getContext(UMB_CONTENT_TYPE_REPOSITORY_CONTEXT);
   const { data } = await repository.requestById(elementTypeKey);
   ```

3. **Render Properties**:
   ```typescript
   <umb-property-dataset .value=${this._values} @change=${this.#onChange}>
     ${repeat(properties, (prop) => html`
       <umb-property
         alias=${prop.alias}
         label=${prop.label}
         .propertyEditorUiAlias=${prop.editor.alias}
         .config=${prop.editor.config}>
       </umb-property>
     `)}
   </umb-property-dataset>
   ```

4. **Handle Value Changes**: Collect property values from the dataset and update the block's value object.

## Design Decisions

### Why a Simplified Approach?

The original v13 implementation used AngularJS and Umbraco's content resource to scaffold element types and render properties. In v16, this requires:
- Understanding Umbraco's new repository pattern
- Working with content type contexts and property datasets
- Handling property value converters and validation
- Managing workspace state correctly

Rather than attempting to replicate this complexity immediately, this implementation:
- Provides a working foundation
- Uses standard Umbraco v16 patterns (modals, Lit components)
- Clearly documents what's simplified and what would be needed for full implementation
- Allows blocks to be created and associated with element types
- Preserves the data structure for future enhancement

This approach solves the immediate problem (getting Content Blocks working in v16) while providing a clear path forward for enhancement.

## Testing

To test this implementation:

1. Configure a Content Blocks data type in Umbraco
2. Add element types to the configuration
3. Add the property to a document type
4. Create content and test:
   - Adding blocks
   - Editing blocks (currently shows element info)
   - Deleting blocks
   - Max items limit
   - Element type selection with multiple types

## Contributing

When enhancing this implementation, consider:
- Keep the simple modal approach for basic scenarios
- Add full property editing as an optional enhancement
- Maintain backward compatibility with saved data
- Follow Umbraco v16 patterns and practices
- Update documentation with implementation status
