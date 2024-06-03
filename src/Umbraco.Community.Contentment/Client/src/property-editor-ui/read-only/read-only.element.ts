import { customElement, html, nothing, property } from '@umbraco-cms/backoffice/external/lit';
import { UmbLitElement } from '@umbraco-cms/backoffice/lit-element';
import { UmbPropertyEditorUiElement } from '@umbraco-cms/backoffice/extension-registry';
import { UmbPropertyEditorConfigCollection } from '@umbraco-cms/backoffice/property-editor';

import '../../property-editor-ui/editor-notes/editor-notes.element.js';

const ELEMENT_NAME = 'contentment-property-editor-ui-read-only';

@customElement(ELEMENT_NAME)
export class ContentmentPropertyEditorUIReadOnlyElement
	extends UmbLitElement
	implements UmbPropertyEditorUiElement
{
	@property({ attribute: false })
	public value?: unknown;

	public set config(config: UmbPropertyEditorConfigCollection | undefined) {
		if (!config) return;

		const jsonConfig = JSON.stringify(config, null, 2);
		const jsonValue = JSON.stringify(this.value, null, 2);
		const markup = `
<p><em>The property value and data-type configuration are in read-only mode.</em></p>
${jsonValue ? `<details><summary>Value</summary><umb-code-block copy>${jsonValue}</umb-code-block></details>` : ''}
${jsonConfig ? `<details><summary>Configuration</summary><umb-code-block copy>${jsonConfig}</umb-code-block></details>`	: ''}
`;

		this.#notesConfig = new UmbPropertyEditorConfigCollection([
			{ alias: 'alertType', value: 'warning' },
			{ alias: 'icon', value: 'icon-alert' },
			{ alias: 'heading', value: 'This editor has not been developed yet.' },
			{ alias: 'message', value: { markup } },
		]);
	}

	#notesConfig?: UmbPropertyEditorConfigCollection;

	constructor() {
		super();
	}

	render() {
		if (!this.#notesConfig) return nothing;
		return html`
			<contentment-property-editor-ui-editor-notes .config=${this.#notesConfig}>
			</contentment-property-editor-ui-editor-notes>
		`;
	}
}

export { ContentmentPropertyEditorUIReadOnlyElement as element };

declare global {
	interface HTMLElementTagNameMap {
		[ELEMENT_NAME]: ContentmentPropertyEditorUIReadOnlyElement;
	}
}
