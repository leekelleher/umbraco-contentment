// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

import { html, customElement, LitElement } from '@umbraco-cms/backoffice/external/lit';

@customElement('contentment-lee-was-here')
export default class ContentmentLeeWasHereElement extends LitElement {
	render() {
		return html`
			<img
				src="/App_Plugins/Contentment/backoffice/contentment/lee-was-here.svg"
				alt="Lee was here"
				width="115"
				height="55" />
		`;
	}
}

declare global {
	interface HTMLElementTagNameMap {
		'contentment-lee-was-here': ContentmentLeeWasHereElement;
	}
}
