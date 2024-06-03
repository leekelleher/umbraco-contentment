import { css, customElement, html, property } from '@umbraco-cms/backoffice/external/lit';
import { UmbLitElement } from '@umbraco-cms/backoffice/lit-element';
import { UmbPropertyEditorUiElement } from '@umbraco-cms/backoffice/extension-registry';
import { UmbPropertyEditorConfigCollection } from '@umbraco-cms/backoffice/property-editor';

const ELEMENT_NAME = 'contentment-property-editor-ui-templated-label';

@customElement(ELEMENT_NAME)
export class ContentmentPropertyEditorUITemplatedLabelElement
	extends UmbLitElement
	implements UmbPropertyEditorUiElement
{
	@property({ attribute: false })
	public value?: unknown;

	@property({ attribute: false })
	public config?: UmbPropertyEditorConfigCollection;

	constructor() {
		super();
	}

	render() {
		return html`
			<div>
				<p><em>This is a read-only view of the property value and data-type configuration.</em></p>
				<details>
					<summary>Value</summary>
					<umb-code-block copy>${JSON.stringify(this.value, null, 2)}</umb-code-block>
				</details>
				<details>
					<summary>Configuration</summary>
					<umb-code-block copy>${JSON.stringify(this.config, null, 2)}</umb-code-block>
				</details>
			</div>
		`;
	}

	static styles = [
		css`
			details > summary {
				cursor: pointer;
				font-weight: bold;
			}

			umb-code-block {
				margin-bottom: 1rem;
			}
		`,
	];
}

export { ContentmentPropertyEditorUITemplatedLabelElement as element };

declare global {
	interface HTMLElementTagNameMap {
		[ELEMENT_NAME]: ContentmentPropertyEditorUITemplatedLabelElement;
	}
}
