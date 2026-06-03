# Element Workspace Modal — Phase 1 Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Build a reusable `CONTENTMENT_ELEMENT_WORKSPACE_MODAL` that accepts `{ elementType, key, value }`, loads the Element Type schema including compositions, renders tabs/groups/properties with validation, and returns the updated triple on submit.

**Architecture:** A `ContentmentElementManager` (modeled on core's internal `UmbBlockElementManager`) composes public backoffice APIs (`UmbContentTypeStructureManager`, `UmbElementWorkspaceDataManager`, `UmbValidationController`, guard managers) and provides a faux `UMB_CONTENT_WORKSPACE_CONTEXT` so core's `umb-content-workspace-view-edit-tab` element chain renders the property UI. The modal element owns a `uui-tab-group` tab strip (router-safe in imperative modals) and drives `umb-content-workspace-view-edit-tab` by container ID.

**Tech Stack:** TypeScript · Lit · `@umbraco-cms/backoffice` v18 (public exports only) · `UmbModalBaseElement` · `UmbLitElement`

---

## File Map

| File | Status | Responsibility |
|---|---|---|
| `src/workspace/element/types.ts` | **Create** | `ContentmentElementValue` canonical type |
| `src/workspace/element/element-workspace-modal.token.ts` | **Create** | `CONTENTMENT_ELEMENT_WORKSPACE_MODAL` token + data/value types |
| `src/workspace/element/element-property-dataset.context.ts` | **Create** | Concrete `ContentmentElementPropertyDatasetContext` (satisfies abstract `UmbElementPropertyDatasetContext`) |
| `src/workspace/element/element-manager.context.ts` | **Create** | `ContentmentElementManager` — faux content workspace context + all data/structure/validation wiring |
| `src/workspace/element/element-workspace-modal.element.ts` | **Create** | `contentment-element-workspace-modal` — modal chrome, tab strip, submit/cancel |
| `src/workspace/element/manifests.ts` | **Create** | Modal manifest |
| `src/workspace/element/index.ts` | **Create** | Public re-exports for this feature |
| `src/workspace/manifests.ts` | **Modify** | Spread in element workspace manifests |
| `src/property-editor-ui/types.ts` | **Modify** | Alias `ContentmentContentBlockValue = ContentmentElementValue` (no duplication) |
| `src/types.ts` | **Modify** | Export element workspace types |
| `src/workspace/workspace.element.ts` | **Modify** | Temporary dev harness button |

---

## Task 1 — Canonical type + modal token

**Files:**
- Create: `src/workspace/element/types.ts`
- Create: `src/workspace/element/element-workspace-modal.token.ts`

- [ ] **Step 1.1 — Create `types.ts`**

```ts
// SPDX-License-Identifier: MIT
// Copyright © 2025 Lee Kelleher

import type { UmbPropertyValueData } from '@umbraco-cms/backoffice/property';

/** The canonical persisted shape of a single element entry. */
export type ContentmentElementValue = {
	elementType: string;
	key: string;
	value: Record<string, unknown>;
};

/**
 * Internal runtime data model used by ContentmentElementManager and
 * ContentmentElementPropertyDatasetContext. Must satisfy
 * `UmbElementDetailModel` (i.e. `{ values: Array<UmbPropertyValueData> }`).
 */
export type ContentmentElementDataModel = {
	key?: string;
	values: Array<UmbPropertyValueData>;
};
```

- [ ] **Step 1.2 — Create `element-workspace-modal.token.ts`**

```ts
// SPDX-License-Identifier: MIT
// Copyright © 2025 Lee Kelleher

import type { ContentmentElementValue } from './types.js';
import { UmbModalToken } from '@umbraco-cms/backoffice/modal';

export type ContentmentElementWorkspaceModalData = {
	element: ContentmentElementValue;
	readonly?: boolean;
};

export type ContentmentElementWorkspaceModalValue = {
	element: ContentmentElementValue;
};

export const CONTENTMENT_ELEMENT_WORKSPACE_MODAL = new UmbModalToken<
	ContentmentElementWorkspaceModalData,
	ContentmentElementWorkspaceModalValue
>('Umb.Contentment.Modal.ElementWorkspace', {
	modal: {
		type: 'sidebar',
		size: 'medium',
	},
});
```

- [ ] **Step 1.3 — Commit**

```bash
git add src/workspace/element/types.ts src/workspace/element/element-workspace-modal.token.ts
git commit -m "feat: add ContentmentElementValue type and element workspace modal token"
```

---

## Task 2 — Property dataset context (concrete subclass)

**Files:**
- Create: `src/workspace/element/element-property-dataset.context.ts`

`UmbElementPropertyDatasetContext` is abstract with three generics: `<ContentModel, ContentTypeModel, DataOwnerType>`. It requires four concrete members: `name`, `culture`, `segment`, `getName`. For invariant-only editing, `culture` and `segment` are always `null`.

`ContentmentElementDataModel` is defined in Task 1's `types.ts`. This file imports from both `types.ts` and `element-manager.context.ts` (type-only import to avoid circular dependency issues at runtime — both files reference each other by type only, which is fine in TypeScript).

- [ ] **Step 2.1 — Create `element-property-dataset.context.ts`**

```ts
// SPDX-License-Identifier: MIT
// Copyright © 2025 Lee Kelleher

import type { UmbPropertyDatasetContext } from '@umbraco-cms/backoffice/property';
import type { UmbContentTypeModel } from '@umbraco-cms/backoffice/content-type';
import type { UmbControllerHost } from '@umbraco-cms/backoffice/controller-api';
import type { UmbVariantId } from '@umbraco-cms/backoffice/variant';
import { UmbElementPropertyDatasetContext } from '@umbraco-cms/backoffice/content';
import { of } from '@umbraco-cms/backoffice/external/rxjs';
import type { ContentmentElementDataModel } from './types.js';
import type { ContentmentElementManager } from './element-manager.context.js';

export class ContentmentElementPropertyDatasetContext
	extends UmbElementPropertyDatasetContext<
		ContentmentElementDataModel,
		UmbContentTypeModel,
		ContentmentElementManager
	>
	implements UmbPropertyDatasetContext
{
	// Shown in the property dataset header (the element type name)
	readonly name = this._dataOwner.structure.ownerContentTypeObservablePart((x) => x?.name);
	// Invariant-only: culture and segment are always null
	readonly culture = of<string | null>(null);
	readonly segment = of<string | null>(null);

	constructor(host: UmbControllerHost, manager: ContentmentElementManager, variantId?: UmbVariantId) {
		super(host, manager, variantId);
	}

	getName(): string | undefined {
		return this._dataOwner.structure.getOwnerContentType()?.name;
	}
}
```

> **If tsc complains about the generic:** `UmbElementPropertyDatasetContext` may only declare 2 or 3 generics — check the base class declaration. Adjust the extends clause to match exactly. The critical requirement is that `this._dataOwner` resolves as `ContentmentElementManager` so `structure.ownerContentTypeObservablePart(...)` compiles.

- [ ] **Step 2.2 — Commit placeholder (full commit after Task 3)**

The dataset context imports the manager type — commit after both files exist to avoid dangling imports.

---

## Task 3 — Element manager (the core logic)

**Files:**
- Create: `src/workspace/element/element-manager.context.ts`

This is the heart of the feature. Modeled directly on `UmbBlockElementManager` (v18 source: `C:\VCS\Umbraco\HQ\Umbraco-CMS-v18\src\Umbraco.Web.UI.Client\src\packages\block\block\workspace\block-element-manager.ts`) with these differences:
- No `UmbViewContext` (no router/view-hint integration needed)
- No `UmbContentValidationToHintsManager`
- Added `IS_CONTENT_WORKSPACE_CONTEXT = true as const` + `propertyStructureById` to satisfy faux context guards
- `init()` method instead of constructor-observed `contentTypeId`
- Invariant-only (no variantId tracking)
- `setup(host)` provides faux content workspace context directly on the host element

- [ ] **Step 3.1 — Create `element-manager.context.ts`**

```ts
// SPDX-License-Identifier: MIT
// Copyright © 2025 Lee Kelleher

import type { UmbPropertyValueData } from '@umbraco-cms/backoffice/property';
import { UmbVariantPropertyGuardManager } from '@umbraco-cms/backoffice/property';
import type { UmbContentTypeModel } from '@umbraco-cms/backoffice/content-type';
import { UmbContentTypeStructureManager } from '@umbraco-cms/backoffice/content-type';
import type { Observable } from '@umbraco-cms/backoffice/external/rxjs';
import { appendToFrozenArray } from '@umbraco-cms/backoffice/observable-api';
import type { UmbControllerHost } from '@umbraco-cms/backoffice/controller-api';
import { type UmbClassInterface, UmbControllerBase } from '@umbraco-cms/backoffice/class-api';
import { UmbDocumentTypeDetailRepository } from '@umbraco-cms/backoffice/document-type';
import { UmbVariantId } from '@umbraco-cms/backoffice/variant';
import { UmbValidationController } from '@umbraco-cms/backoffice/validation';
import {
	UmbElementWorkspaceDataManager,
	type UmbElementPropertyDataOwner,
	UMB_CONTENT_WORKSPACE_CONTEXT,
	type UmbContentWorkspaceContext,
} from '@umbraco-cms/backoffice/content';
import { UmbReadOnlyVariantGuardManager } from '@umbraco-cms/backoffice/utils';
import { UmbDataTypeItemRepositoryManager } from '@umbraco-cms/backoffice/data-type';
import type { ContentmentElementDataModel } from './types.js';
import { ContentmentElementPropertyDatasetContext } from './element-property-dataset.context.js';

export class ContentmentElementManager
	extends UmbControllerBase
	implements UmbElementPropertyDataOwner<ContentmentElementDataModel, UmbContentTypeModel>
{
	// Satisfies IS_CONTENT_WORKSPACE_CONTEXT guard on UMB_CONTENT_WORKSPACE_CONTEXT token
	readonly IS_CONTENT_WORKSPACE_CONTEXT = true as const;

	readonly #data = new UmbElementWorkspaceDataManager<ContentmentElementDataModel>(this);

	#loadedResolve!: () => void;
	readonly #loaded = new Promise<void>((resolve) => {
		this.#loadedResolve = resolve;
	});

	readonly structure = new UmbContentTypeStructureManager<UmbContentTypeModel>(
		this,
		new UmbDocumentTypeDetailRepository(this),
	);

	readonly #dataTypeItemManager = new UmbDataTypeItemRepositoryManager(this);

	readonly validation = new UmbValidationController(this);

	readonly propertyViewGuard = new UmbVariantPropertyGuardManager(this);
	readonly propertyWriteGuard = new UmbVariantPropertyGuardManager(this);
	readonly readOnlyGuard = new UmbReadOnlyVariantGuardManager(this);

	// UmbElementPropertyDataOwner required observables
	readonly unique = this.#data.createObservablePartOfCurrent((data) => data?.key);
	readonly values = this.#data.createObservablePartOfCurrent((data) => data?.values);

	constructor(host: UmbControllerHost) {
		super(host);
		this.propertyViewGuard.fallbackToPermitted();
		this.propertyWriteGuard.fallbackToPermitted();

		// Keep data-type item manager in sync with structure changes so setPropertyValue can resolve editorAlias
		this.observe(
			this.structure.contentTypeDataTypeUniques,
			(dataTypeUniques: Array<string>) => {
				this.#dataTypeItemManager.setUniques(dataTypeUniques);
			},
			null,
		);
	}

	/**
	 * @description Seed the manager with element data and load the element type schema.
	 * Call setup(host) immediately after to wire contexts into the host element.
	 * @param {string} elementType - The element type GUID.
	 * @param {string} key - The block/element key (GUID).
	 * @param {Record<string, unknown>} value - Flat alias→value map of current property values.
	 * @param {boolean} [readonly=false] - When true, the write guard denies all edits.
	 */
	async init(elementType: string, key: string, value: Record<string, unknown>, readonly = false): Promise<void> {
		// Invariant-only — no culture/segment variation
		this.#data.setVaries(false);
		this.#data.setVariesByCulture(false);
		this.#data.setVariesBySegment(false);

		const model: ContentmentElementDataModel = {
			key,
			values: Object.entries(value).map(
				([alias, v]) =>
					({
						alias,
						value: v,
						culture: null,
						segment: null,
						// editorAlias starts as '' — overwritten by setPropertyValue when editors write back
						editorAlias: '',
					}) as UmbPropertyValueData,
			),
		};

		this.#data.setPersisted(model);
		this.#data.setCurrent(model);

		if (readonly) {
			this.propertyWriteGuard.addRule({ unique: 'readonly', permitted: false });
		}

		this.structure.loadType(elementType);
		await this.structure.whenLoaded();
		this.#loadedResolve();
	}

	// ── UmbElementPropertyDataOwner implementation ─────────────────────────────

	isLoaded(): Promise<void> {
		return this.#loaded;
	}

	getUnique(): string | undefined {
		return this.#data.getCurrent()?.key;
	}

	getEntityType(): string {
		return 'element';
	}

	getValues(): ContentmentElementDataModel['values'] | undefined {
		return this.#data.getCurrent()?.values;
	}

	async propertyValueByAlias<ReturnType = unknown>(
		propertyAlias: string,
		variantId?: UmbVariantId,
	): Promise<Observable<ReturnType | undefined> | undefined> {
		return this.#data.createObservablePartOfCurrent(
			(data) =>
				data?.values?.find((x) => x?.alias === propertyAlias && (variantId ? variantId.compare(x) : true))
					?.value as ReturnType,
		);
	}

	getPropertyValue<ReturnType = unknown>(alias: string, variantId?: UmbVariantId): ReturnType | undefined {
		const currentData = this.#data.getCurrent();
		if (currentData) {
			const entry = currentData.values?.find(
				(x) => x.alias === alias && (variantId ? variantId.compare(x) : true),
			);
			return entry?.value as ReturnType;
		}
		return undefined;
	}

	async setPropertyValue<ValueType = unknown>(alias: string, value: ValueType, variantId?: UmbVariantId): Promise<void> {
		this.initiatePropertyValueChange();
		variantId ??= UmbVariantId.CreateInvariant();

		const property = await this.structure.getPropertyStructureByAlias(alias);
		if (!property) {
			throw new Error(`Property alias "${alias}" not found.`);
		}

		const dataTypeItem = await this.#dataTypeItemManager.getItemByUnique(property.dataType.unique);
		const editorAlias = dataTypeItem?.propertyEditorSchemaAlias ?? '';

		const entry = { editorAlias, ...variantId.toObject(), alias, value } as UmbPropertyValueData;

		const currentData = this.#data.getCurrent();
		if (currentData) {
			const values = appendToFrozenArray(
				currentData.values ?? [],
				entry,
				(x) => x.alias === alias && variantId!.compare(x),
			);
			this.#data.updateCurrent({ values });
		}
		this.finishPropertyValueChange();
	}

	initiatePropertyValueChange(): void {
		this.#data.initiatePropertyValueChange();
	}

	finishPropertyValueChange = (): void => {
		this.#data.finishPropertyValueChange();
	};

	// ── Extra members for UMB_PROPERTY_STRUCTURE_WORKSPACE_CONTEXT guard ───────

	/**
	 * @description Delegates to structure.propertyStructureById.
	 * Presence of this method satisfies the `'propertyStructureById' in context` guard
	 * on UMB_PROPERTY_STRUCTURE_WORKSPACE_CONTEXT so umb-content-workspace-view-edit-tab
	 * can resolve it.
	 */
	propertyStructureById(id: string) {
		return this.structure.propertyStructureById(id);
	}

	// ── Helpers for the modal element ─────────────────────────────────────────

	getHasUnpersistedChanges(): boolean {
		return this.#data.getHasUnpersistedChanges();
	}

	/** Convert current values back to a flat alias→value record for the modal result. */
	getResult(): Record<string, unknown> {
		const values = this.getValues() ?? [];
		return Object.fromEntries(values.map((v) => [v.alias, v.value]));
	}

	/**
	 * @description Wire all contexts into the host element.
	 * Must be called from the modal element's connectedCallback AFTER init() resolves.
	 *
	 * Provides:
	 *   - UMB_PROPERTY_DATASET_CONTEXT  (via ContentmentElementPropertyDatasetContext constructor)
	 *   - UMB_VARIANT_CONTEXT           (via ContentmentElementPropertyDatasetContext constructor — internal to dataset)
	 *   - UMB_VALIDATION_CONTEXT        (via validation.provideAt)
	 *   - UMB_CONTENT_WORKSPACE_CONTEXT (via host.provideContext — satisfies both content and
	 *                                    property-structure workspace context guards since they
	 *                                    share the 'UmbWorkspaceContext' alias)
	 *
	 * @param {UmbClassInterface} host - The modal element (UmbLitElement / UmbModalBaseElement).
	 */
	setup(host: UmbClassInterface): void {
		// 1. Dataset context provides UMB_PROPERTY_DATASET_CONTEXT + UMB_VARIANT_CONTEXT (invariant)
		new ContentmentElementPropertyDatasetContext(host, this, UmbVariantId.CreateInvariant());

		// 2. Validation context allows property-level required/regex messages to surface automatically
		this.validation.provideAt(host);

		// 3. Faux workspace context — one provide satisfies both:
		//    UMB_CONTENT_WORKSPACE_CONTEXT  (guard: IS_CONTENT_WORKSPACE_CONTEXT === true)
		//    UMB_PROPERTY_STRUCTURE_WORKSPACE_CONTEXT  (guard: 'propertyStructureById' in context)
		//    Both tokens share alias 'UmbWorkspaceContext' — providing once resolves both.
		host.provideContext(UMB_CONTENT_WORKSPACE_CONTEXT, this as unknown as UmbContentWorkspaceContext);
	}

	override destroy(): void {
		this.structure.destroy();
		super.destroy();
	}
}
```

- [ ] **Step 3.2 — Commit tasks 1 partial + 2 + 3**

```bash
git add src/workspace/element/types.ts src/workspace/element/element-property-dataset.context.ts src/workspace/element/element-manager.context.ts
git commit -m "feat: add ContentmentElementManager and property dataset context"
```

---

## Task 4 — Modal element

**Files:**
- Create: `src/workspace/element/element-workspace-modal.element.ts`

Consumes `this.data` (type: `ContentmentElementWorkspaceModalData`), renders tabs/groups/properties via core's `umb-content-workspace-view-edit-tab`, manages dirty-close confirmation, and resolves with updated `ContentmentElementValue`.

**Tab routing:** Core's `umb-content-workspace-view-edit` uses `umb-router-slot` which breaks in imperative modals (tab clicks would mutate `window.location`). We **do not** use it. Instead we render `umb-content-workspace-view-edit-tab` directly with a `uui-tab-group` for tab switching. The side-effect import `'@umbraco-cms/backoffice/content'` registers `umb-content-workspace-view-edit-tab` (and its descendants `umb-content-workspace-view-edit-properties`, `umb-content-workspace-property`) as custom elements at runtime.

- [ ] **Step 4.1 — Create `element-workspace-modal.element.ts`**

```ts
// SPDX-License-Identifier: MIT
// Copyright © 2025 Lee Kelleher

// Side-effect: registers umb-content-workspace-view-edit-tab + descendants as custom elements
import '@umbraco-cms/backoffice/content';

import { css, customElement, html, nothing, state, when } from '@umbraco-cms/backoffice/external/lit';
import { UmbModalBaseElement, UMB_MODAL_MANAGER_CONTEXT, UMB_DISCARD_CHANGES_MODAL } from '@umbraco-cms/backoffice/modal';
import { UmbTextStyles } from '@umbraco-cms/backoffice/style';
import { UmbContentTypeContainerStructureHelper } from '@umbraco-cms/backoffice/content-type';
import type { UmbPropertyTypeContainerMergedModel } from '@umbraco-cms/backoffice/content-type';
import { ContentmentElementManager } from './element-manager.context.js';
import type {
	ContentmentElementWorkspaceModalData,
	ContentmentElementWorkspaceModalValue,
} from './element-workspace-modal.token.js';

@customElement('contentment-element-workspace-modal')
export class ContentmentElementWorkspaceModalElement extends UmbModalBaseElement<
	ContentmentElementWorkspaceModalData,
	ContentmentElementWorkspaceModalValue
> {
	readonly #manager = new ContentmentElementManager(this);

	readonly #tabsHelper = new UmbContentTypeContainerStructureHelper<never>(this);

	@state()
	private _tabs: Array<UmbPropertyTypeContainerMergedModel> = [];

	@state()
	private _hasRootProperties = false;

	@state()
	private _activeContainerId: string | null | undefined = null;

	@state()
	private _headline = '';

	@state()
	private _icon = 'icon-document-line';

	@state()
	private _ready = false;

	get #readonly(): boolean {
		return this.data?.readonly === true;
	}

	override async connectedCallback(): Promise<void> {
		super.connectedCallback();
		if (!this.data) return;

		const { elementType, key, value } = this.data.element;

		// init() loads the element type schema (including compositions) and seeds data
		await this.#manager.init(elementType, key, value, this.#readonly);

		// Wire UMB_PROPERTY_DATASET_CONTEXT, UMB_VALIDATION_CONTEXT, UMB_CONTENT_WORKSPACE_CONTEXT
		// onto this element so descendant umb-content-workspace-view-edit-tab can resolve them
		this.#manager.setup(this);

		// Set up tab helper to compute tab strip from the loaded structure
		this.#tabsHelper.setIsRoot(true);
		this.#tabsHelper.setContainerChildType('Tab');
		this.#tabsHelper.setStructureManager(this.#manager.structure as never);

		this.observe(this.#tabsHelper.childContainers, (tabs) => {
			this._tabs = tabs ?? [];
			// Default to null (root) if no tabs, or first tab if tabs exist
			if (this._activeContainerId === null && tabs?.length) {
				this._activeContainerId = tabs[0].id ?? tabs[0].ids[0] ?? null;
			}
		});

		this.observe(this.#tabsHelper.hasProperties, (has) => {
			this._hasRootProperties = has ?? false;
		});

		const contentType = this.#manager.structure.getOwnerContentType();
		this._headline = contentType?.name ?? '';
		this._icon = contentType?.icon ?? 'icon-document-line';

		this._ready = true;
	}

	async #onSubmit(): Promise<void> {
		try {
			await this.#manager.validation.validate();
		} catch {
			// Validation failed — inline messages are already shown by the property editors
			return;
		}

		const { elementType, key } = this.data!.element;
		this.value = {
			element: {
				elementType,
				key,
				value: this.#manager.getResult(),
			},
		};
		this._submitModal();
	}

	async #onCancel(): Promise<void> {
		if (!this.#readonly && this.#manager.getHasUnpersistedChanges()) {
			// Show the standard backoffice "discard changes?" dialog
			const modalManager = await this.getContext(UMB_MODAL_MANAGER_CONTEXT);
			const discardModal = modalManager?.open(this, UMB_DISCARD_CHANGES_MODAL);
			try {
				await discardModal?.onSubmit();
				// User chose to discard — fall through to _rejectModal
			} catch {
				// User chose to stay — abort cancel
				return;
			}
		}
		this._rejectModal();
	}

	override render() {
		if (!this._ready) {
			return html`<umb-body-layout .headline=${this._headline}><uui-loader></uui-loader></umb-body-layout>`;
		}

		const showTabStrip = this._tabs.length > 1 || (this._hasRootProperties && this._tabs.length > 0);

		return html`
			<umb-body-layout .headline=${this._headline}>
				<umb-icon slot="icon" .name=${this._icon}></umb-icon>

				${when(
					showTabStrip,
					() => html`
						<uui-tab-group slot="tabs">
							${when(
								this._hasRootProperties,
								() => html`
									<uui-tab
										label=${this.localize.term('general_content')}
										.active=${this._activeContainerId === null}
										@click=${() => { this._activeContainerId = null; }}>
									</uui-tab>
								`,
							)}
							${this._tabs.map(
								(tab) => html`
									<uui-tab
										.label=${this.localize.string(tab.name) ?? ''}
										.active=${this._activeContainerId === (tab.id ?? tab.ids[0])}
										@click=${() => { this._activeContainerId = tab.id ?? tab.ids[0] ?? null; }}>
									</uui-tab>
								`,
							)}
						</uui-tab-group>
					`,
				)}

				<umb-content-workspace-view-edit-tab
					.containerId=${this._activeContainerId}>
				</umb-content-workspace-view-edit-tab>

				<div slot="actions">
					<uui-button
						label=${this.localize.term('general_cancel')}
						@click=${this.#onCancel}>
					</uui-button>
					${when(
						!this.#readonly,
						() => html`
							<uui-button
								color="positive"
								look="primary"
								label=${this.localize.term('buttons_save')}
								@click=${this.#onSubmit}>
							</uui-button>
						`,
					)}
				</div>
			</umb-body-layout>
		`;
	}

	static override styles = [
		UmbTextStyles,
		css`
			umb-content-workspace-view-edit-tab {
				padding: var(--uui-size-layout-1);
			}
		`,
	];
}

export { ContentmentElementWorkspaceModalElement as element };

declare global {
	interface HTMLElementTagNameMap {
		'contentment-element-workspace-modal': ContentmentElementWorkspaceModalElement;
	}
}
```

> **Implementation notes:**
> - `umb-content-workspace-view-edit-tab` accepts `containerId: string | null | undefined`. Passing `null` renders root-level properties (groups with no parent tab). Passing a tab's merged ID renders that tab's groups.
> - `tab.id ?? tab.ids[0]` handles merged composition tabs where `id` may be undefined and `ids` holds all merged IDs.
> - The `uui-loader` during `_ready = false` prevents flicker before the structure loads.
> - If `UmbContentTypeContainerStructureHelper` generic typing causes TS errors, cast the structure manager: `this.#manager.structure as unknown as UmbContentTypeStructureManager<ContentTypeModel>` matching how `umb-content-workspace-view-edit-tab` does it internally.

- [ ] **Step 4.2 — Commit**

```bash
git add src/workspace/element/element-workspace-modal.element.ts
git commit -m "feat: add contentment-element-workspace-modal element"
```

---

## Task 5 — Manifests + module index

**Files:**
- Create: `src/workspace/element/manifests.ts`
- Create: `src/workspace/element/index.ts`
- Modify: `src/workspace/manifests.ts`

- [ ] **Step 5.1 — Create `manifests.ts`**

```ts
// SPDX-License-Identifier: MIT
// Copyright © 2025 Lee Kelleher

export const manifests: Array<UmbExtensionManifest> = [
	{
		type: 'modal',
		alias: 'Umb.Contentment.Modal.ElementWorkspace',
		name: '[Contentment] Element Workspace Modal',
		element: () => import('./element-workspace-modal.element.js'),
	},
];
```

- [ ] **Step 5.2 — Create `index.ts`**

```ts
// SPDX-License-Identifier: MIT
// Copyright © 2025 Lee Kelleher

export * from './element-manager.context.js';
export * from './element-property-dataset.context.js';
export * from './element-workspace-modal.token.js';
export type * from './types.js';
```

- [ ] **Step 5.3 — Update `src/workspace/manifests.ts`**

Current content of `src/workspace/manifests.ts`:
```ts
// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

const workspace: UmbExtensionManifest = {
	type: 'workspace',
	alias: 'Umb.Contentment.Workspace.Contentment',
	name: '[Contentment] Workspace',
	element: () => import('./workspace.element.js'),
	meta: { entityType: 'contentment' },
};

export const manifests = [workspace];
```

Replace with:
```ts
// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

import { manifests as elementWorkspace } from './element/manifests.js';

const workspace: UmbExtensionManifest = {
	type: 'workspace',
	alias: 'Umb.Contentment.Workspace.Contentment',
	name: '[Contentment] Workspace',
	element: () => import('./workspace.element.js'),
	meta: { entityType: 'contentment' },
};

export const manifests = [workspace, ...elementWorkspace];
```

- [ ] **Step 5.4 — Commit**

```bash
git add src/workspace/element/manifests.ts src/workspace/element/index.ts src/workspace/manifests.ts
git commit -m "feat: register element workspace modal manifest"
```

---

## Task 6 — Type alignment (no duplication)

**Files:**
- Modify: `src/property-editor-ui/types.ts`
- Modify: `src/types.ts`

`ContentmentContentBlockValue` in `property-editor-ui/types.ts` is identical to `ContentmentElementValue`. Alias it to the canonical definition without breaking any consumers (same shape, same exported name).

- [ ] **Step 6.1 — Update `src/property-editor-ui/types.ts`**

Replace the `ContentmentContentBlockValue` type (lines 69–73 of current file) with a re-export alias:

```ts
// ContentmentContentBlockValue is the persisted shape of a Content Block entry.
// Aliased to ContentmentElementValue (the reusable element workspace type) — same shape.
export type { ContentmentElementValue as ContentmentContentBlockValue } from '../workspace/element/types.js';
```

Remove the old inline definition:
```ts
// DELETE THIS:
export type ContentmentContentBlockValue = {
	elementType: string;
	key: string;
	value: Record<string, unknown>;
};
```

- [ ] **Step 6.2 — Update `src/types.ts`**

Add an export for the element workspace types so they appear in the published package:

```ts
// SPDX-License-Identifier: MPL-2.0
// Copyright © 2025 Lee Kelleher

export type * from './api/index.js';
export type * from './extensions/types.js';
export type * from './property-editor-ui/types.js';
export type * from './workspace/element/types.js';   // <-- add this line
```

- [ ] **Step 6.3 — Commit**

```bash
git add src/property-editor-ui/types.ts src/types.ts
git commit -m "refactor: alias ContentmentContentBlockValue to canonical ContentmentElementValue"
```

---

## Task 7 — Dev harness

**Files:**
- Modify: `src/workspace/workspace.element.ts`

Add a temporary button to the Contentment Settings workspace that opens the element workspace modal with sample data and logs the result. Lets you test without touching Content Blocks.

**Before adding the harness**, pick an element type GUID from the running demo site. Navigate to Settings → Element Types in the Umbraco backoffice, create a simple element type with a Text Box property, and copy its GUID.

- [ ] **Step 7.1 — Add dev harness imports to `workspace.element.ts`**

Add after the existing imports:
```ts
import { UMB_MODAL_MANAGER_CONTEXT } from '@umbraco-cms/backoffice/modal';
import { UmbId } from '@umbraco-cms/backoffice/id';
import { CONTENTMENT_ELEMENT_WORKSPACE_MODAL } from './element/element-workspace-modal.token.js';
```

- [ ] **Step 7.2 — Add harness method and button to `workspace.element.ts`**

Add the method to `ContentmentWorkspaceElement`:
```ts
// TODO: Remove dev harness before merge
async #openTestModal(): Promise<void> {
    const modalManager = await this.getContext(UMB_MODAL_MANAGER_CONTEXT);
    if (!modalManager) return;

    // Replace with a real element type GUID from your demo site
    const ELEMENT_TYPE_GUID = '<REPLACE_WITH_ELEMENT_TYPE_GUID>';

    const modal = modalManager.open(this, CONTENTMENT_ELEMENT_WORKSPACE_MODAL, {
        data: {
            element: {
                elementType: ELEMENT_TYPE_GUID,
                key: UmbId.new(),
                value: {},
            },
        },
    });

    try {
        const result = await modal.onSubmit();
        console.log('[Contentment dev harness] Modal submitted:', result);
    } catch {
        console.log('[Contentment dev harness] Modal cancelled');
    }
}
```

In the `#renderFeatureOptions()` method, add a dev harness box (after the tree dashboard section):

```ts
// TODO: Remove dev harness before merge
<uui-box headline="Dev Harness (remove before merge)">
    <uui-button
        look="primary"
        label="Open Element Workspace Modal (test)"
        @click=${this.#openTestModal}>
    </uui-button>
</uui-box>
```

- [ ] **Step 7.3 — Commit**

```bash
git add src/workspace/workspace.element.ts
git commit -m "chore: add temporary element workspace modal dev harness"
```

---

## Task 8 — Build verification

- [ ] **Step 8.1 — TypeScript build**

```bash
cd src/Umbraco.Community.Contentment.Client
npm run build
```

Expected: Build completes with no TypeScript errors. The two most likely error sites:
1. **Generic mismatch in `ContentmentElementPropertyDatasetContext`** — if `UmbElementPropertyDatasetContext` has different generic arity, adjust the extends clause to match what the base class declares (check the base class source).
2. **`host.provideContext(UMB_CONTENT_WORKSPACE_CONTEXT, this as unknown as ...)` type error** — if `UmbContentWorkspaceContext` is not exported from `@umbraco-cms/backoffice/content`, use `typeof UMB_CONTENT_WORKSPACE_CONTEXT.TYPE` instead.

- [ ] **Step 8.2 — Start dev host**

In two terminals:
```bash
# Terminal 1 (C# host)
dotnet run --project src/Umbraco.Cms.18.x

# Terminal 2 (client)
cd src/Umbraco.Community.Contentment.Client
npm run dev
```

- [ ] **Step 8.3 — Verification checklist**

Navigate to the Contentment workspace (Settings section).

**Scenario A — Basic rendering:**
- Click the dev harness button with an element type GUID that has multiple tabs and a composition.
- Expected: modal opens, headline = element type name, tab strip shows tabs from both owner and composition, correct groups in each tab.

**Scenario B — Mandatory property validation:**
- Open an element type that has at least one mandatory property. Leave it blank, click Save.
- Expected: modal stays open, the mandatory property shows a red validation message inline.

**Scenario C — Successful submit:**
- Fill in all required fields, click Save.
- Expected: modal closes, browser console logs `[Contentment dev harness] Modal submitted:` with `{ element: { elementType, key, value: { alias: '...' } } }`.

**Scenario D — Dirty-close confirmation:**
- Edit a field value, then click Cancel (or press Escape).
- Expected: the standard "Discard changes?" dialog appears. Choosing "Discard" closes the modal; choosing "Stay" returns to editing.

**Scenario E — Clean close (no changes):**
- Open the modal, edit nothing, click Cancel.
- Expected: modal closes immediately with no confirmation dialog.

**Scenario F — Read-only mode:**
- Temporarily modify the dev harness to pass `readonly: true`.
- Expected: properties rendered as read-only, no Submit button visible, Cancel closes without a dirty-close prompt.

**Scenario G — Tab navigation:**
- Switch tabs in the modal.
- Expected: browser URL does **not** change; the correct properties appear for each tab.

- [ ] **Step 8.4 — Commit verification note**

```bash
git commit --allow-empty -m "chore: verify element workspace modal passes all manual test scenarios"
```

---

## Risks and edge cases

| Risk | Mitigation |
|---|---|
| `UmbElementPropertyDatasetContext` generic signature differs from block's version | Read base class declaration in installed `@umbraco-cms/backoffice/content`; adjust generic params accordingly |
| `editorAlias: ''` on seeded values causes a property editor to malfunction | After `init()`, `setPropertyValue` overwrites the alias on first edit using the real schema alias from `UmbDataTypeItemRepositoryManager`; pre-existing block-list precedent uses `''` with no issues |
| `UmbContentTypeContainerStructureHelper` generic type errors | Cast: `this.#manager.structure as unknown as UmbContentTypeStructureManager<UmbContentTypeModel>` |
| Faux context missing a member that a future core element accesses at runtime | Keep the consumer member trace in JSDoc comments in `element-manager.context.ts`; pin `@umbraco-cms/backoffice` peer dep |
| `ContentmentContentBlockValue` consumers (published `.d.ts`) break after alias change | Alias export preserves the name — existing imports resolve identically |
| `client.md` documents peer dep `^16.0.0` but `package.json` is `^18.0.0-rc1` | Pre-existing doc drift; do not silently fix, but note it to the maintainer |
