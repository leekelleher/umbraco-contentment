import { css, customElement, html, property, when } from '@umbraco-cms/backoffice/external/lit';
import { UmbLitElement } from '@umbraco-cms/backoffice/lit-element';
import type { UmbPropertyEditorConfigCollection, UmbPropertyEditorUiElement } from '@umbraco-cms/backoffice/property-editor';

import '../../components/info-box/info-box.element.js';

const ELEMENT_NAME = 'contentment-property-editor-ui-read-only';

@customElement(ELEMENT_NAME)
export class ContentmentPropertyEditorUIReadOnlyElement extends UmbLitElement implements UmbPropertyEditorUiElement {
	#config?: string;
	#value?: string;

	@property({ attribute: false })
	public value?: unknown;

	public set config(config: UmbPropertyEditorConfigCollection | undefined) {
		if (!config) return;
		this.#config = JSON.stringify(config, null, 2);
		this.#value = JSON.stringify(this.value, null, 2);
	}

	override render() {
		return html`
			<contentment-info-box type="warning" icon="icon-alert" heading="This editor has not been developed yet.">
				<p><em>The property value and data-type configuration are in read-only mode.</em></p>

				${when(
					this.#value,
					() =>
						html`
							<details>
								<summary>Value</summary>
								<umb-code-block copy>${this.#value}</umb-code-block>
							</details>
						`
				)}
				${when(
					this.#config,
					() =>
						html`
							<details>
								<summary>Configuration</summary>
								<umb-code-block copy>${this.#config}</umb-code-block>
							</details>
						`
				)}
			</contentment-info-box>
		`;
	}

	static override styles = [
		css`
			details > summary {
				cursor: pointer;
				font-weight: bold;
			}
		`,
	];
}

export { ContentmentPropertyEditorUIReadOnlyElement as element };

declare global {
	interface HTMLElementTagNameMap {
		[ELEMENT_NAME]: ContentmentPropertyEditorUIReadOnlyElement;
	}
}
