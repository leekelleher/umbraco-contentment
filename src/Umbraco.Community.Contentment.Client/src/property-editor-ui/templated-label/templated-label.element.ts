// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

import { CONTENTMENT_LIQUID_CONTEXT } from '../../global-context/index.js';
import { parseBoolean, tryHideLabel, tryMoveBeforePropertyGroup } from '../../utils/index.js';
import { customElement, nothing, property, state, unsafeHTML } from '@umbraco-cms/backoffice/external/lit';
import { UmbLitElement } from '@umbraco-cms/backoffice/lit-element';
import type { PropertyValues } from '@umbraco-cms/backoffice/external/lit';
import type { Template } from '../../external/liquidjs/index.js';
import type { UmbPropertyEditorUiElement } from '@umbraco-cms/backoffice/property-editor';

@customElement('contentment-property-editor-ui-templated-label')
export class ContentmentPropertyEditorUITemplatedLabelElement
	extends UmbLitElement
	implements UmbPropertyEditorUiElement
{
	#liquidContext?: typeof CONTENTMENT_LIQUID_CONTEXT.TYPE;

	#hideLabel: boolean = false;

	#hidePropertyGroup: boolean = false;

	#templateString?: string;

	#template?: Array<Template>;

	@state()
	private _markup: unknown;

	@property({ attribute: false })
	public set value(value: unknown) {
		this.#value = value;
		this.#renderLiquid();
	}
	public get value(): unknown {
		return this.#value;
	}
	#value?: unknown;

	constructor() {
		super();
		this.consumeContext(CONTENTMENT_LIQUID_CONTEXT, (context) => {
			this.#liquidContext = context;
			this.#parseTemplate();
		});
	}

	set config(config: UmbPropertyEditorUiElement['config']) {
		if (!config) return;
		this.#hideLabel = parseBoolean(config.getValueByAlias('hideLabel'));
		this.#hidePropertyGroup = parseBoolean(config.getValueByAlias('hidePropertyGroup'));

		this.#templateString = config.getValueByAlias<string>('notes') ?? '';
		this.#parseTemplate();
	}

	#parseTemplate() {
		if (!this.#liquidContext || !this.#templateString) return;
		this.#template = this.#liquidContext.parse(this.#templateString);
		this.#renderLiquid();
	}

	async #renderLiquid() {
		if (!this.#liquidContext || !this.#template) return;
		const markup = await this.#liquidContext.render(this.#template, { model: { value: this.#value } });
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
