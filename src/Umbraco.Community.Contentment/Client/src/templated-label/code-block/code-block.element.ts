import { css, customElement, html, property } from '@umbraco-cms/backoffice/external/lit';
import { UmbLitElement } from '@umbraco-cms/backoffice/lit-element';

const ELEMENT_NAME = 'contentment-templated-label-code-block';

@customElement(ELEMENT_NAME)
export class ContentmentTemplatedLabelCodeBlockElement extends UmbLitElement {
	@property({ attribute: false })
	value?: unknown;

	render() {
		return html`
			<details>
				<summary>View data</summary>
				<umb-code-block language="JSON" copy>${JSON.stringify(this.value, null, 2)}</umb-code-block>
			</details>
		`;
	}

	static styles = [
		css`
			details > summary {
				cursor: pointer;
			}
		`,
	];
}

export { ContentmentTemplatedLabelCodeBlockElement as element };

declare global {
	interface HTMLElementTagNameMap {
		[ELEMENT_NAME]: ContentmentTemplatedLabelCodeBlockElement;
	}
}
