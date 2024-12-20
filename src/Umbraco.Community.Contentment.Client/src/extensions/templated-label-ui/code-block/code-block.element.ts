// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

import type { ContentmentTemplatedLabelUiElement } from '../types.js';
import { css, customElement, html, property } from '@umbraco-cms/backoffice/external/lit';
import { UmbLitElement } from '@umbraco-cms/backoffice/lit-element';

@customElement('contentment-templated-label-ui-code-block')
export class ContentmentTemplatedLabelUiCodeBlockElement
	extends UmbLitElement
	implements ContentmentTemplatedLabelUiElement
{
	@property({ attribute: false })
	value?: unknown;

	override render() {
		return html`
			<details>
				<summary>View data</summary>
				<umb-code-block language="JSON" copy>${JSON.stringify(this.value, null, 2)}</umb-code-block>
			</details>
		`;
	}

	static override styles = [
		css`
			details > summary {
				cursor: pointer;
			}
		`,
	];
}

export { ContentmentTemplatedLabelUiCodeBlockElement as element };

declare global {
	interface HTMLElementTagNameMap {
		'contentment-templated-label-ui-code-block': ContentmentTemplatedLabelUiCodeBlockElement;
	}
}
