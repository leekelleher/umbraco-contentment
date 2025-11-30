// SPDX-License-Identifier: MIT
// Copyright © 2025 Lee Kelleher

import { customElement, nothing, property, state, unsafeHTML, until } from '@umbraco-cms/backoffice/external/lit';
import { Liquid } from '../../external/liquidjs/index.js';
import { UmbLitElement } from '@umbraco-cms/backoffice/lit-element';
import { UmbTextStyles } from '@umbraco-cms/backoffice/style';
import { UMB_BLOCK_ENTRY_CONTEXT } from '@umbraco-cms/backoffice/block';
import type { ContentmentBlockEditorCustomViewLiquidManifestKind } from './liquid.kind.js';
import type { Template } from '../../external/liquidjs/index.js';
import type { UmbBlockDataType, UmbBlockLayoutBaseModel } from '@umbraco-cms/backoffice/block';
import type {
	UmbBlockEditorCustomViewConfiguration,
	UmbBlockEditorCustomViewElement,
} from '@umbraco-cms/backoffice/block-custom-view';
import type { UmbBlockTypeBaseModel } from '@umbraco-cms/backoffice/block-type';

@customElement('contentment-block-editor-liquid-view')
export class ContentmentBlockEditorLiquidViewElement extends UmbLitElement implements UmbBlockEditorCustomViewElement {
	#engine = new Liquid({ cache: true });

	#template?: Array<Template>;

	@state()
	private _contentElementTypeAlias?: string;

	@property({ attribute: false })
	public set manifest(value: ContentmentBlockEditorCustomViewLiquidManifestKind | undefined) {
		this.#manifest = value;

		if (value?.template) {
			if (typeof value.template === 'function') {
				value.template().then((result) => {
					const templateString = typeof result === 'string' ? result : result.default;
					this.#template = this.#engine.parse(templateString);
					this.requestUpdate();
				});
			} else {
				this.#template = this.#engine.parse(value.template);
			}
		}
	}
	public get manifest(): ContentmentBlockEditorCustomViewLiquidManifestKind | undefined {
		return this.#manifest;
	}
	#manifest?: ContentmentBlockEditorCustomViewLiquidManifestKind | undefined;

	@property({ attribute: false })
	config?: UmbBlockEditorCustomViewConfiguration;

	@property({ attribute: false })
	blockType?: UmbBlockTypeBaseModel;

	@property({ attribute: false })
	contentKey?: string;

	@property({ attribute: false })
	label?: string;

	@property({ attribute: false })
	icon?: string;

	@property({ attribute: false })
	index?: number;

	@property({ attribute: false })
	layout?: UmbBlockLayoutBaseModel;

	@property({ attribute: false })
	content?: UmbBlockDataType;

	@property({ attribute: false })
	settings?: UmbBlockDataType;

	@property({ attribute: false })
	contentInvalid?: boolean;

	@property({ attribute: false })
	settingsInvalid?: boolean;

	@property({ attribute: false })
	unsupported?: boolean;

	@property({ attribute: false })
	unpublished?: boolean;

	constructor() {
		super();

		this.consumeContext(UMB_BLOCK_ENTRY_CONTEXT, (blockEntry) => {
			this.observe(blockEntry?.contentElementTypeAlias, (contentElementTypeAlias) => {
				this._contentElementTypeAlias = contentElementTypeAlias;
			});
		});
	}

	override render() {
		return until(this.#renderTemplate());
	}

	async #renderTemplate() {
		if (!this.#engine || !this.#template) return nothing;

		const scope = {
			manifest: this.manifest,
			config: this.config,
			blockType: this.blockType,
			label: this.label,
			icon: this.icon,
			index: this.index,
			layout: this.layout,
			content: this.content,
			settings: this.settings,
			contentInvalid: this.contentInvalid,
			settingsInvalid: this.settingsInvalid,
			unsupported: this.unsupported,
			unpublished: this.unpublished,
			contentElementTypeAlias: this._contentElementTypeAlias,
		};

		const markup = await this.#engine.render(this.#template, scope);
		return markup ? unsafeHTML(markup) : nothing;
	}

	static override readonly styles = [UmbTextStyles];
}

export { ContentmentBlockEditorLiquidViewElement as element };

declare global {
	interface HTMLElementTagNameMap {
		'contentment-block-editor-liquid-view': ContentmentBlockEditorLiquidViewElement;
	}
}
