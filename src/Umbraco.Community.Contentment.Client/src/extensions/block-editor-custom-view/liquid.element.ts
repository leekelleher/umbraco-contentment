// SPDX-License-Identifier: MIT
// Copyright © 2025 Lee Kelleher

import { CONTENTMENT_LIQUID_CONTEXT } from '../../global-context/liquid/liquid.context.js';
import { customElement, html, nothing, property, state, unsafeHTML } from '@umbraco-cms/backoffice/external/lit';
import { UmbLitElement } from '@umbraco-cms/backoffice/lit-element';
import { UmbTextStyles } from '@umbraco-cms/backoffice/style';
import { UMB_BLOCK_ENTRY_CONTEXT } from '@umbraco-cms/backoffice/block';
import type { ContentmentBlockEditorCustomViewLiquidManifestKind } from './liquid.kind.js';
import type { PropertyValues } from '@umbraco-cms/backoffice/external/lit';
import type { Template } from '../../external/liquidjs.js';
import type { UmbBlockDataType, UmbBlockLayoutBaseModel } from '@umbraco-cms/backoffice/block';
import type {
	UmbBlockEditorCustomViewConfiguration,
	UmbBlockEditorCustomViewElement,
} from '@umbraco-cms/backoffice/block-custom-view';
import type { UmbBlockTypeBaseModel } from '@umbraco-cms/backoffice/block-type';

/**
 * Properties from the scope object that should trigger a re-render when changed.
 * Note: 'manifest' is excluded as it has its own setter that handles template loading.
 * Note: 'contentElementTypeAlias' is excluded as it's observed separately via UMB_BLOCK_ENTRY_CONTEXT.
 */
const WATCHED_SCOPE_PROPERTIES = [
	'config',
	'blockType',
	'label',
	'icon',
	'index',
	'layout',
	'content',
	'settings',
	'contentInvalid',
	'settingsInvalid',
	'unsupported',
	'unpublished',
] as const;

@customElement('contentment-block-editor-liquid-view')
export class ContentmentBlockEditorLiquidViewElement extends UmbLitElement implements UmbBlockEditorCustomViewElement {
	#liquid?: typeof CONTENTMENT_LIQUID_CONTEXT.TYPE;

	#template?: string;

	#templateCompiled?: Array<Template>;

	@state()
	private _contentElementTypeAlias?: string;

	@state()
	private _markup: unknown;

	@property({ attribute: false })
	public set manifest(value: ContentmentBlockEditorCustomViewLiquidManifestKind | undefined) {
		this.#manifest = value;
		this.#loadTemplate(value);
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

		this.consumeContext(CONTENTMENT_LIQUID_CONTEXT, (context) => {
			this.#liquid = context;
			this.#parseLiquidTemplate();
		});

		this.consumeContext(UMB_BLOCK_ENTRY_CONTEXT, (blockEntry) => {
			this.observe(blockEntry?.contentElementTypeAlias, (contentElementTypeAlias) => {
				this._contentElementTypeAlias = contentElementTypeAlias;
				this.#renderLiquidTemplate();
			});
		});
	}

	async #loadTemplate(manifest: ContentmentBlockEditorCustomViewLiquidManifestKind | undefined) {
		if (!manifest) return;

		let template: string | undefined;

		// Priority 1: Loader function (dynamic import)
		if (typeof manifest.template === 'function') {
			try {
				const result = await manifest.template();
				template = typeof result === 'string' ? result : result.default;
			} catch (error) {
				console.error('[Contentment] Failed to load template via import:', error);
			}
		}
		// Priority 2: Path string (fetch at runtime)
		else if (typeof manifest.template === 'string') {
			try {
				const response = await fetch(manifest.template);
				if (!response.ok) {
					throw new Error(`HTTP ${response.status}: ${manifest.template}`);
				}
				template = await response.text();
			} catch (error) {
				console.error('[Contentment] Failed to fetch template:', error);
			}
		}

		// Priority 3: Fallback to inline content
		if (!template && manifest.templateContent) {
			template = manifest.templateContent;
		}

		this.#template = template;
		this.#parseLiquidTemplate();
	}

	async #parseLiquidTemplate() {
		if (!this.#liquid || !this.#template) return;
		try {
			this.#templateCompiled = await this.#liquid.parse(this.#template);
		} catch (error) {
			console.error('[Contentment] Failed to parse Liquid template:', error);
			this._markup = this.#renderError('parse', error);
			return;
		}
		this.#renderLiquidTemplate();
	}

	async #renderLiquidTemplate() {
		if (!this.#liquid || !this.#templateCompiled) return;

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

		try {
			const markup = await this.#liquid.render(this.#templateCompiled, scope);
			this._markup = markup ? unsafeHTML(markup) : nothing;
		} catch (error) {
			console.error('[Contentment] Failed to render Liquid template:', error);
			this._markup = this.#renderError('render', error);
		}
	}

	#renderError(stage: 'parse' | 'render', error: unknown) {
		const message = error instanceof Error ? error.message : String(error);
		const escaped = message.replace(/[&<>"']/g, (c) => `&#${c.charCodeAt(0)};`);
		return html`
			<contentment-info-box
				type="warning"
				icon="icon-alert"
				headline="Liquid template ${stage} error"
				message=${escaped}></contentment-info-box>
		`;
	}

	protected override updated(changedProperties: PropertyValues): void {
		super.updated(changedProperties);

		// Re-render Liquid when any scope property changes
		if (WATCHED_SCOPE_PROPERTIES.some((prop) => changedProperties.has(prop))) {
			this.#renderLiquidTemplate();
		}
	}

	override render() {
		return this._markup ?? nothing;
	}

	static override readonly styles = [UmbTextStyles];
}

export { ContentmentBlockEditorLiquidViewElement as element };

declare global {
	interface HTMLElementTagNameMap {
		'contentment-block-editor-liquid-view': ContentmentBlockEditorLiquidViewElement;
	}
}
