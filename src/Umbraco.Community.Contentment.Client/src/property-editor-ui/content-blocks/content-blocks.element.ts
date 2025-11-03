// SPDX-License-Identifier: MPL-2.0
// Copyright © 2025 Lee Kelleher

import { ContentmentContentBlocksManagerContext } from './content-blocks-manager.context.js';
import { ContentmentContentBlocksEntriesContext } from './content-blocks-entries.context.js';
import { css, customElement, html, nothing, property, repeat, state } from '@umbraco-cms/backoffice/external/lit';
import { debounceTime } from '@umbraco-cms/backoffice/external/rxjs';
import { observeMultiple } from '@umbraco-cms/backoffice/observable-api';
import { umbDestroyOnDisconnect, UmbLitElement } from '@umbraco-cms/backoffice/lit-element';
import { umbExtensionsRegistry } from '@umbraco-cms/backoffice/extension-registry';
import { UmbExtensionElementInitializer } from '@umbraco-cms/backoffice/extension-api';
import { UmbPropertyEditorConfigCollection } from '@umbraco-cms/backoffice/property-editor';
import { UmbSorterController } from '@umbraco-cms/backoffice/sorter';
import { UmbFormControlMixin, UmbValidationContext } from '@umbraco-cms/backoffice/validation';
import {
	UMB_BLOCK_LIST_PROPERTY_EDITOR_SCHEMA_ALIAS,
	UMB_BLOCK_LIST_PROPERTY_EDITOR_UI_ALIAS,
} from '@umbraco-cms/backoffice/block-list';
import { UMB_PROPERTY_CONTEXT, UMB_PROPERTY_DATASET_CONTEXT } from '@umbraco-cms/backoffice/property';
import type { ContentmentConfigurationEditorValue, ContentmentContentBlockValue } from '../types.js';
import type { UmbBlockLayoutBaseModel } from '@umbraco-cms/backoffice/block';
import type { UmbBlockListLayoutModel, UmbBlockListValueModel } from '@umbraco-cms/backoffice/block-list';
import type { UmbBlockTypeBaseModel } from '@umbraco-cms/backoffice/block-type';
import type { UmbModalRouteBuilder } from '@umbraco-cms/backoffice/router';
import type { UmbPropertyEditorConfig, UmbPropertyEditorUiElement } from '@umbraco-cms/backoffice/property-editor';
import type { UUIModalSidebarSize } from '@umbraco-cms/backoffice/external/uui';

import './content-blocks-entry.element.js';

@customElement('contentment-property-editor-ui-content-blocks')
export class ContentmentPropertyEditorUIContentBlocksElement
	extends UmbFormControlMixin<Array<ContentmentContentBlockValue> | undefined, typeof UmbLitElement, undefined>(
		UmbLitElement
	)
	implements UmbPropertyEditorUiElement
{
	readonly #sorter = new UmbSorterController<UmbBlockListLayoutModel, UmbLitElement & { contentKey?: string }>(this, {
		getUniqueOfElement: (element) => element.contentKey!,
		getUniqueOfModel: (modelEntry) => modelEntry.contentKey,
		itemSelector: 'umb-block-list-entry',
		onChange: ({ model }) => {
			this.#entriesContext.setLayouts(model);
		},
	});

	readonly #managerContext = new ContentmentContentBlocksManagerContext(this);
	readonly #entriesContext = new ContentmentContentBlocksEntriesContext(this);
	readonly #validationContext = new UmbValidationContext(this);

	@state()
	private _blocks?: Array<UmbBlockTypeBaseModel>;

	@state()
	private _catalogueRouteBuilder?: UmbModalRouteBuilder;

	@state()
	private _createButtonLabelKey = '#content_createEmpty';

	@state()
	private _layouts: Array<UmbBlockLayoutBaseModel> = [];

	@state()
	private _initialized = false;

	#fauxValue?: UmbBlockListValueModel;

	#lastValue?: Array<ContentmentContentBlockValue> = undefined;

	@property({ attribute: false })
	public override set value(value: Array<ContentmentContentBlockValue> | undefined) {
		this.#lastValue = value;

		if (!value) {
			super.value = undefined;
			return;
		}

		super.value = value;

		this.#fauxValue = this.#convertToBlockListValue(value);

		this.#managerContext.setLayouts(this.#fauxValue.layout[UMB_BLOCK_LIST_PROPERTY_EDITOR_SCHEMA_ALIAS] ?? []);
		this.#managerContext.setContents(this.#fauxValue.contentData);
		this.#managerContext.setSettings(this.#fauxValue.settingsData);
		this.#managerContext.setExposes(this.#fauxValue.expose);
	}
	public override get value(): Array<ContentmentContentBlockValue> | undefined {
		return super.value;
	}

	public set config(config: UmbPropertyEditorConfigCollection | undefined) {
		if (!config) return;

		this._createButtonLabelKey = config.getValueByAlias('addButtonLabelKey') ?? '#content_createEmpty';

		const contentBlockTypes =
			config.getValueByAlias<Array<ContentmentConfigurationEditorValue>>('contentBlockTypes') ?? [];

		const blockTypes = contentBlockTypes.map((contentBlockType) => ({
			contentElementTypeKey: contentBlockType.key,
			label: contentBlockType.value.nameTemplate as string,
			iconColor: '',
			backgroundColor: '',
			editorSize: contentBlockType.value.overlaySize as UUIModalSidebarSize,
			forceHideContentEditorInOverlay: false,
		}));

		this.#managerContext.setBlockTypes(blockTypes);

		const editorConfig = new UmbPropertyEditorConfigCollection([
			{ alias: 'useLiveEditing', value: false },
			{ alias: 'validationLimit', value: { min: 0, max: Infinity } },
		] as UmbPropertyEditorConfig);

		this.#managerContext.setEditorConfiguration(editorConfig);
	}

	public set readonly(value) {
		this.#readonly = value;

		if (this.#readonly) {
			this.#sorter.disable();
			this.#managerContext.readOnlyState.fallbackToPermitted();
		} else {
			this.#sorter.enable();
			this.#managerContext.readOnlyState.fallbackToNotPermitted();
		}
	}
	public get readonly() {
		return this.#readonly;
	}
	#readonly = false;

	constructor() {
		super();

		// Dynamically loads in the Block List property-editor, so we can reuse some of its internal UI components.
		new UmbExtensionElementInitializer(
			this,
			umbExtensionsRegistry,
			UMB_BLOCK_LIST_PROPERTY_EDITOR_UI_ALIAS,
			() => (this._initialized = true)
		);

		this.consumeContext(UMB_PROPERTY_CONTEXT, (context) => this.#gotPropertyContext(context));

		this.consumeContext(UMB_PROPERTY_DATASET_CONTEXT, async (context) =>
			this.#managerContext.setVariantId(context?.getVariantId())
		);

		this.observe(
			this.#entriesContext.layoutEntries,
			(layouts) => {
				this._layouts = layouts;
				this.#sorter.setModel(layouts);
				this.#managerContext.setLayouts(layouts);
			},
			null
		);

		this.observe(this.#managerContext.blockTypes, (blockTypes) => (this._blocks = blockTypes), null);

		this.observe(
			this.#entriesContext.catalogueRouteBuilder,
			(routeBuilder) => (this._catalogueRouteBuilder = routeBuilder),
			null
		);
	}

	#gotPropertyContext(context: typeof UMB_PROPERTY_CONTEXT.TYPE | undefined) {
		this.observe(
			context?.dataPath,
			(dataPath) => {
				if (dataPath) {
					// Set the data path for the local validation context:
					this.#validationContext.setDataPath(dataPath);
					this.#validationContext.autoReport();
				}
			},
			'observeDataPath'
		);

		this.observe(context?.alias, (alias) => this.#managerContext.setPropertyAlias(alias), 'observePropertyAlias');

		this.observe(
			observeMultiple([
				this.#managerContext.layouts,
				this.#managerContext.contents,
				this.#managerContext.settings,
				this.#managerContext.exposes,
			]).pipe(debounceTime(20)),
			([layouts, contents, settings, exposes]) => {
				if (layouts.length === 0) {
					this.#fauxValue = undefined;
				} else {
					this.#fauxValue = {
						...this.#fauxValue,
						layout: { [UMB_BLOCK_LIST_PROPERTY_EDITOR_SCHEMA_ALIAS]: layouts },
						contentData: contents,
						settingsData: settings,
						expose: exposes,
					};
				}

				// If we don't have a value set from the outside or an internal value, we don't want to set the value.
				// This is added to prevent the block list from setting an empty value on startup.
				if (this.#lastValue === undefined && super.value === undefined) {
					return;
				}

				const realValue = this.#convertToContentBlockValue(this.#fauxValue);

				context?.setValue(realValue);
			},
			'motherObserver'
		);
	}

	#convertToBlockListValue(items: Array<ContentmentContentBlockValue> | undefined): UmbBlockListValueModel {
		return {
			layout: { [UMB_BLOCK_LIST_PROPERTY_EDITOR_SCHEMA_ALIAS]: items?.map((x) => ({ contentKey: x.key })) },
			contentData:
				items?.map((x) => ({
					key: x.key,
					contentTypeKey: x.elementType,
					values: Object.entries(x.value ?? {}).map(([alias, value]) => ({
						alias,
						value,
						culture: null,
						segment: null,
						editorAlias: '',
						entityType: 'block',
					})),
				})) ?? [],
			settingsData: [],
			expose: items?.map((x) => ({ contentKey: x.key, culture: null, segment: null })) ?? [],
		};
	}

	#convertToContentBlockValue(model: UmbBlockListValueModel | undefined): Array<ContentmentContentBlockValue> {
		const items: Array<ContentmentContentBlockValue> = [];

		model?.layout[UMB_BLOCK_LIST_PROPERTY_EDITOR_SCHEMA_ALIAS]?.forEach((layout) => {
			const data = model.contentData.find((d) => d.key === layout.contentKey);
			if (!data) return;

			items.push({
				elementType: data.contentTypeKey,
				key: layout.contentKey,
				value: Object.fromEntries(data.values.map((v) => [v.alias, v.value])),
			});
		});

		return items;
	}

	override render() {
		if (!this._initialized) return html`<uui-loader></uui-loader>`;
		return html`
			${repeat(
				this._layouts,
				(layout, index) => `${index}_${layout.contentKey}`,
				(layout, index) => html`
					<contentment-content-blocks-entry index=${index} .contentKey=${layout.contentKey} ${umbDestroyOnDisconnect()}>
					</contentment-content-blocks-entry>
				`
			)}
			${this.#renderCreateButtonGroup()}
		`;
	}

	#renderCreateButtonGroup() {
		if (this.readonly && this._layouts.length > 0) {
			return nothing;
		} else {
			return html`<uui-button-group>${this.#renderCreateButton()}</uui-button-group>`;
		}
	}

	#renderCreateButton() {
		let createPath: string | undefined;
		if (this._blocks?.length === 1) {
			const elementKey = this._blocks[0].contentElementTypeKey;
			createPath =
				this._catalogueRouteBuilder?.({ view: 'create', index: -1 }) + 'modal/umb-modal-workspace/create/' + elementKey;
		} else {
			createPath = this._catalogueRouteBuilder?.({ view: 'create', index: -1 });
		}
		return html`
			<uui-button
				look="placeholder"
				label=${this.localize.string(this._createButtonLabelKey)}
				href=${createPath ?? ''}
				?disabled=${this.readonly}></uui-button>
		`;
	}

	static override readonly styles = [
		css`
			:host {
				display: grid;
				gap: 1px;
			}
			> div {
				display: flex;
				flex-direction: column;
				align-items: stretch;
			}

			uui-button-group {
				padding-top: 1px;
				display: grid;
				grid-template-columns: 1fr auto;
			}
		`,
	];
}

export { ContentmentPropertyEditorUIContentBlocksElement as element };

declare global {
	interface HTMLElementTagNameMap {
		'contentment-property-editor-ui-content-blocks': ContentmentPropertyEditorUIContentBlocksElement;
	}
}
