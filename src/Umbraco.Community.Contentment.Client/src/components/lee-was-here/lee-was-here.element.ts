// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

import { html, customElement, LitElement, css } from '@umbraco-cms/backoffice/external/lit';

const ELEMENT_NAME = 'lee-was-here';

@customElement(ELEMENT_NAME)
export default class ContentmentLeeWasHereElement extends LitElement {
	override render() {
		return html`
			<img
				src="/App_Plugins/Contentment/lee-was-here.svg"
				alt="Lee was here"
				width="115"
				height="55" />
		`;
	}

	static override readonly styles = [
		css`
			:host {
				height: 55px;
				width: 115px;
			}
		`,
	];
}

declare global {
	interface HTMLElementTagNameMap {
		[ELEMENT_NAME]: ContentmentLeeWasHereElement;
	}
}
