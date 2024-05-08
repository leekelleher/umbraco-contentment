// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

import { css, customElement, html, property } from '@umbraco-cms/backoffice/external/lit';
import { UmbLitElement } from '@umbraco-cms/backoffice/lit-element';
import { UmbPropertyValueChangeEvent } from '@umbraco-cms/backoffice/property-editor';
import { UUITextareaElement } from '@umbraco-cms/backoffice/external/uui';
import type { UmbPropertyEditorConfigCollection } from '@umbraco-cms/backoffice/property-editor';
import type { UmbPropertyEditorUiElement } from '@umbraco-cms/backoffice/extension-registry';

const ELEMENT_NAME = 'contentment-property-editor-ui-configuration-editor';

@customElement(ELEMENT_NAME)
export class ContentmentPropertyEditorUIConfigurationEditorElement
	extends UmbLitElement
	implements UmbPropertyEditorUiElement
{
	@property({ type: Object })
	public value?: object;

	#buttonLabelKey: string = 'general_add';

	#configurationType?: string;

	public set config(config: UmbPropertyEditorConfigCollection | undefined) {
		if (!config) return;
		this.#buttonLabelKey = config.getValueByAlias('addButtonLabelKey') ?? 'general_add';
		this.#configurationType = config.getValueByAlias('configurationType');
	}

	#openConfigurationEditorModal() {
		alert(`Open configuration editor modal for ${this.#configurationType}`);
	}

	#onChange(event: Event & { target: UUITextareaElement }) {
		this.value = JSON.parse(event.target.value as string);
		this.dispatchEvent(new UmbPropertyValueChangeEvent());
	}

	render() {
		return html`
			<uui-textarea auto-height .value=${JSON.stringify(this.value, null, 4)} @change=${this.#onChange}></uui-textarea>
			<pre><code>${JSON.stringify(this.value)}</code></pre>
			${this.#renderButton()}
		`;
	}

	#renderButton() {
		return html`
			<uui-button
				label=${this.localize.term(this.#buttonLabelKey)}
				look="placeholder"
				@click=${this.#openConfigurationEditorModal}></uui-button>
		`;
	}

	static styles = [
		css`
			uui-button {
				width: 100%;
			}
		`,
	];
}

export { ContentmentPropertyEditorUIConfigurationEditorElement as element };

declare global {
	interface HTMLElementTagNameMap {
		[ELEMENT_NAME]: ContentmentPropertyEditorUIConfigurationEditorElement;
	}
}
