// SPDX-License-Identifier: MIT
// Copyright © 2024 Lee Kelleher

import { CONTENTMENT_LIQUID_CONTEXT } from '../../global-context/liquid/liquid.context-token.js';
import { parseBoolean, tryHideLabel, tryMoveBeforePropertyGroup } from '../../utils/index.js';
import { customElement, nothing, property, state, unsafeHTML } from '@umbraco-cms/backoffice/external/lit';
import { UmbLitElement } from '@umbraco-cms/backoffice/lit-element';
import type { PropertyValues } from '@umbraco-cms/backoffice/external/lit';
import type { Template } from '../../external/liquidjs.js';
import type { UmbPropertyEditorUiElement } from '@umbraco-cms/backoffice/property-editor';

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
		this.#templateCompiled = await this.#liquid.parse(this.#template);
		this.#renderLiquidTemplate();
	}

	async #renderLiquidTemplate() {
		if (!this.#liquid || !this.#templateCompiled) return;
		const markup = await this.#liquid.render(this.#templateCompiled, { model: { value: this.#value } });
		this._markup = markup ? unsafeHTML(markup) : nothing;
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
