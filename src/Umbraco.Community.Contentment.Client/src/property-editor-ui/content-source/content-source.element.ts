// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

import { css, customElement, html, property, state, when } from '@umbraco-cms/backoffice/external/lit';
import { UmbLitElement } from '@umbraco-cms/backoffice/lit-element';
import { UmbChangeEvent } from '@umbraco-cms/backoffice/event';
import type {
	UmbPropertyEditorConfigCollection,
	UmbPropertyEditorUiElement,
} from '@umbraco-cms/backoffice/property-editor';

import '../../components/property-editor-ui/property-editor-ui.element.js';

@customElement('contentment-property-editor-ui-content-source')
export class ContentmentPropertyEditorUIContentSourceElement
	extends UmbLitElement
	implements UmbPropertyEditorUiElement
{
	@state()
	private _loaded = false;

	@property({ type: Object })
	public value?: unknown;

	public set config(config: UmbPropertyEditorConfigCollection | undefined) {
		if (!config) return;
	}

	#onChange(event: CustomEvent & { target: { data: unknown } }) {
		this.value = event.target.data ?? {};
		this.dispatchEvent(new UmbChangeEvent());
	}

	#onLoaded() {
		// NOTE: In order to use the `umb-input-content-picker-document-root` element,
		// we need to load the "Umb.PropertyEditorUi.ContentPicker.Source" property-editor first.
		this._loaded = true;
	}

	override render() {
		return when(
			!this._loaded,
			() => html`
				<uui-loader></uui-loader>
				<contentment-property-editor-ui
					property-editor-ui-alias="Umb.PropertyEditorUi.ContentPicker.Source"
					@loaded=${this.#onLoaded}>
				</contentment-property-editor-ui>
			`,
			() => html`
				<umb-input-content-picker-document-root .data=${this.value} @change=${this.#onChange}>
				</umb-input-content-picker-document-root>
			`
		);
	}

	static override readonly styles = [
		css`
			contentment-property-editor-ui {
				display: none;
			}
		`,
	];
}

export { ContentmentPropertyEditorUIContentSourceElement as element };

declare global {
	interface HTMLElementTagNameMap {
		'contentment-property-editor-ui-content-source': ContentmentPropertyEditorUIContentSourceElement;
	}
}
