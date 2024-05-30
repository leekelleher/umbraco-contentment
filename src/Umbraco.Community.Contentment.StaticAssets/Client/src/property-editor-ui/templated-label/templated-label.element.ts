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
			<div class="flex">
				<umb-code-block language="config" copy>${JSON.stringify(this.config, null, 2)}</umb-code-block>
				<umb-code-block language="value" copy>${JSON.stringify(this.value, null, 2)}</umb-code-block>
			</div>
		`;
	}

	static styles = [
		css`
			.flex {
				display: flex;
				gap: var(--uui-size-layout-1);
			}

			.flex > umb-code-block {
				flex: 1;
				max-width: 50%;
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
