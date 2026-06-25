// SPDX-License-Identifier: MPL-2.0
// Copyright © 2026 Lee Kelleher

import { css, customElement, html } from '@umbraco-cms/backoffice/external/lit';
import { UmbLitElement } from '@umbraco-cms/backoffice/lit-element';
import { UUIInputElement } from '@umbraco-cms/backoffice/external/uui';

@customElement('contentment-uui-input-wrapper')
export class ContentmentUuiInputWrapperElement extends UmbLitElement {
	override render() {
		return html`
			<slot name="prepend"></slot>
			<slot name="input"></slot>
			<slot name="append"></slot>
		`;
	}

	static override styles = [
		UUIInputElement.styles,
		css`
			:host {
				--uui-input-padding: 1px 9px;
				width: 100%;
			}
		`,
	];
}

declare global {
	interface HTMLElementTagNameMap {
		'contentment-uui-input-wrapper': ContentmentUuiInputWrapperElement;
	}
}
