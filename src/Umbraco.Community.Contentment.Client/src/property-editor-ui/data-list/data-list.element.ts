// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

import { ContentmentDataListRepository } from './data-list.repository.js';
import { customElement, html, property, state } from '@umbraco-cms/backoffice/external/lit';
import { UmbChangeEvent } from '@umbraco-cms/backoffice/event';
import { UmbFormControlMixin, UMB_VALIDATION_EMPTY_LOCALIZATION_KEY } from '@umbraco-cms/backoffice/validation';
import { UmbLitElement } from '@umbraco-cms/backoffice/lit-element';
import { UMB_CONTENT_WORKSPACE_CONTEXT } from '@umbraco-cms/backoffice/content';
import { UMB_PARENT_ENTITY_CONTEXT } from '@umbraco-cms/backoffice/entity';
import { UMB_PROPERTY_CONTEXT } from '@umbraco-cms/backoffice/property';
import type { ContentmentConfigurationEditorValue, ContentmentDataListEditor } from '../types.js';
import type { UmbPropertyEditorUiElement } from '@umbraco-cms/backoffice/property-editor';

import '../../components/lee-was-here/lee-was-here.element.js';
import '../../components/property-editor-ui/property-editor-ui.element.js';

@customElement('contentment-property-editor-ui-data-list')
export class ContentmentPropertyEditorUIDataListElement
	extends UmbFormControlMixin<Array<string> | string | undefined, typeof UmbLitElement, undefined>(UmbLitElement)
	implements UmbPropertyEditorUiElement
{
	#addedFormControl = false;

	#listEditor?: ContentmentDataListEditor;

	#propertyContext?: typeof UMB_PROPERTY_CONTEXT.TYPE;

	#repository = new ContentmentDataListRepository(this);

	@state()
	private _entityIsNew = false;

	@state()
	private _entityUnique?: string | null;

	@state()
	private _parentEntityUnique?: string | null;

	@state()
	private _propertyAlias?: string;

	@state()
	private _variantId?: string;

	@state()
	private _initialized = false;

	@state()
	private _dataSource?: ContentmentConfigurationEditorValue;

	@state()
	private _listEditor?: ContentmentConfigurationEditorValue;

	@property({ type: Boolean })
	mandatory = false;

	@property({ type: String })
	mandatoryMessage = UMB_VALIDATION_EMPTY_LOCALIZATION_KEY;

	@property({ type: Boolean, reflect: true })
	readonly = false;

	public set config(config: UmbPropertyEditorUiElement['config']) {
		if (!config) return;
		this.#config = config;
		this._dataSource = config.getValueByAlias<Array<ContentmentConfigurationEditorValue>>('dataSource')?.[0];
		this._listEditor = config.getValueByAlias<Array<ContentmentConfigurationEditorValue>>('listEditor')?.[0];
	}
	public get config() {
		return this.#config;
	}
	#config: UmbPropertyEditorUiElement['config'];

	constructor() {
		super();

		this.consumeContext(UMB_CONTENT_WORKSPACE_CONTEXT, (contentWorkspaceContext) => {
			this.observe(contentWorkspaceContext?.isNew, (isNew) => (this._entityIsNew = isNew ?? false));
			this.observe(contentWorkspaceContext?.unique, (unique) => (this._entityUnique = unique));
		}).passContextAliasMatches();

		this.consumeContext(UMB_PARENT_ENTITY_CONTEXT, (parentEntityContext) => {
			this.observe(parentEntityContext?.parent, (parent) => (this._parentEntityUnique = parent?.unique));
		}).passContextAliasMatches();

		this.consumeContext(UMB_PROPERTY_CONTEXT, (propertyContext) => {
			this.#propertyContext = propertyContext;
			this.observe(propertyContext?.alias, (alias) => (this._propertyAlias = alias));
			this.observe(propertyContext?.variantId, (variantId) => (this._variantId = variantId?.toString() || 'invariant'));
		});

		this.addValidator(
			'valueMissing',
			() => this.mandatoryMessage ?? UMB_VALIDATION_EMPTY_LOCALIZATION_KEY,
			() =>
				!this.readonly && !!this.mandatory && (this.value === undefined || this.value === null || this.value === ''),
		);
	}

	override async firstUpdated() {
		await this.#init().catch(() => undefined);
		this._initialized = true;
	}

	protected override updated() {
		if (this._initialized && !this.#addedFormControl) {
			const formControl = this.shadowRoot?.querySelector('contentment-property-editor-ui');
			if (formControl) {
				this.addFormControlElement(formControl);
				this.#addedFormControl = true;
			}
		}
	}

	async #init() {
		if (!this._dataSource || !this._listEditor) return;
		this.#listEditor = await this.#repository.__getEditorInternal({
			dataSource: this._dataSource,
			listEditor: this._listEditor,
			entityIsNew: this._entityIsNew,
			entityUnique: this._entityUnique,
			parentEntityUnique: this._parentEntityUnique,
			propertyAlias: this._propertyAlias,
			variantId: this._variantId,
		});

		const combinedConfig = [...(this.#listEditor?.config ?? []), ...(this.config ?? [])];

		// TODO: [LK] Review this, as when being used in Input List, it is overwriting the property's config.
		this.#propertyContext?.setConfig(combinedConfig);
	}

	#onChange(event: UmbChangeEvent & { target: UmbPropertyEditorUiElement }) {
		const element = event.target;
		if (!element || element.value === this.value) return;
		this.value = element.value as Array<string> | string | undefined;
		this.checkValidity();
		this.dispatchEvent(new UmbChangeEvent());
	}

	override render() {
		if (!this._initialized || !this.#listEditor) return html`<uui-loader></uui-loader>`;
		if (!this.#listEditor.propertyEditorUiAlias) return html`<lee-was-here></lee-was-here>`;
		return html`
			<contentment-property-editor-ui
				.config=${this.#listEditor.config}
				.mandatoryMessage=${this.mandatoryMessage}
				.propertyEditorUiAlias=${this.#listEditor.propertyEditorUiAlias}
				.value=${this.value}
				?mandatory=${this.mandatory}
				?readonly=${this.readonly}
				@change=${this.#onChange}>
			</contentment-property-editor-ui>
		`;
	}
}

export { ContentmentPropertyEditorUIDataListElement as element };

declare global {
	interface HTMLElementTagNameMap {
		'contentment-property-editor-ui-data-list': ContentmentPropertyEditorUIDataListElement;
	}
}
