// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

import { parseBoolean, tryHideLabel, tryMoveBeforePropertyGroup } from '../../utils/index.js';
import { customElement, nothing, property, unsafeHTML, until } from '@umbraco-cms/backoffice/external/lit';
import { Liquid } from '../../external/liquidjs/index.js';
import { UmbLitElement } from '@umbraco-cms/backoffice/lit-element';
import type { PropertyValues } from '@umbraco-cms/backoffice/external/lit';
import type { Template } from '../../external/liquidjs/index.js';
import type { UmbPropertyEditorUiElement } from '@umbraco-cms/backoffice/property-editor';

@customElement('contentment-property-editor-ui-templated-label')
export class ContentmentPropertyEditorUITemplatedLabelElement
	extends UmbLitElement
	implements UmbPropertyEditorUiElement
{
	#engine = new Liquid({ cache: true });

	#hideLabel: boolean = false;

	#hidePropertyGroup: boolean = false;

	#template?: Array<Template>;

	@property({ attribute: false })
	public value?: unknown;

	set config(config: UmbPropertyEditorUiElement['config']) {
		if (!config) return;
		this.#hideLabel = parseBoolean(config.getValueByAlias('hideLabel'));
		this.#hidePropertyGroup = parseBoolean(config.getValueByAlias('hidePropertyGroup'));

		const notes = config.getValueByAlias<string>('notes') ?? '';
		this.#template = this.#engine.parse(notes);
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
		return until(this.#renderTemplate());
	}

	async #renderTemplate() {
		if (!this.#engine || !this.#template) return null;
		const markup = await this.#engine.render(this.#template, { model: { value: this.value } });
		return markup ? unsafeHTML(markup) : nothing;
	}
}

export { ContentmentPropertyEditorUITemplatedLabelElement as element };

declare global {
	interface HTMLElementTagNameMap {
		'contentment-property-editor-ui-templated-label': ContentmentPropertyEditorUITemplatedLabelElement;
	}
}
