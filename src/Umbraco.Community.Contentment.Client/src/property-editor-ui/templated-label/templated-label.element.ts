// SPDX-License-Identifier: MIT
// Copyright © 2024 Lee Kelleher

import { CONTENTMENT_LIQUID_CONTEXT } from '../../global-context/liquid/liquid.context.js';
import { parseBoolean, tryHideLabel, tryMoveBeforePropertyGroup } from '../../utils/index.js';
import { customElement, html, nothing, property, state, unsafeHTML } from '@umbraco-cms/backoffice/external/lit';
import { UmbLitElement } from '@umbraco-cms/backoffice/lit-element';
import type { PropertyValues } from '@umbraco-cms/backoffice/external/lit';
import type { Template } from '../../external/liquidjs.js';
import type { UmbPropertyEditorUiElement } from '@umbraco-cms/backoffice/property-editor';

import '../../components/info-box/info-box.element.js';

@customElement('contentment-property-editor-ui-templated-label')
export class ContentmentPropertyEditorUITemplatedLabelElement
	extends UmbLitElement
	implements UmbPropertyEditorUiElement
{
	#liquid?: typeof CONTENTMENT_LIQUID_CONTEXT.TYPE;

	#hideLabel: boolean = false;

	#hidePropertyGroup: boolean = false;

	#template?: string;

	#templateCompiled?: Array<Template>;

	@state()
	private _markup: unknown;

	@property({ attribute: false })
	public set value(value: unknown) {
		this.#value = value;
		this.#renderLiquidTemplate();
	}
	public get value(): unknown {
		return this.#value;
	}
	#value?: unknown;

	constructor() {
		super();
		this.consumeContext(CONTENTMENT_LIQUID_CONTEXT, (context) => {
			this.#liquid = context;
			this.#parseLiquidTemplate();
		});
	}

	set config(config: UmbPropertyEditorUiElement['config']) {
		if (!config) return;
		this.#hideLabel = parseBoolean(config.getValueByAlias('hideLabel'));
		this.#hidePropertyGroup = parseBoolean(config.getValueByAlias('hidePropertyGroup'));
		this.#template = config.getValueByAlias<string>('notes') ?? '';

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
		try {
			const markup = await this.#liquid.render(this.#templateCompiled, { model: { value: this.#value } });
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

	protected override firstUpdated(_changedProperties: PropertyValues): void {
		super.firstUpdated(_changedProperties);

		if (this.#hideLabel) {
			tryHideLabel(this);
		}

		if (this.#hidePropertyGroup) {
			tryMoveBeforePropertyGroup(this);
		}
	}

	override render() {
		return this._markup ?? nothing;
	}
}

export { ContentmentPropertyEditorUITemplatedLabelElement as element };

declare global {
	interface HTMLElementTagNameMap {
		'contentment-property-editor-ui-templated-label': ContentmentPropertyEditorUITemplatedLabelElement;
	}
}
