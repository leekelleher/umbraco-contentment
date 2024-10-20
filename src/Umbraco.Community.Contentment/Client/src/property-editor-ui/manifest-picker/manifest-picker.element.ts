import { parseInt } from '../../utils/index.js';
import { customElement, html, nothing, property, repeat } from '@umbraco-cms/backoffice/external/lit';
import { UmbLitElement } from '@umbraco-cms/backoffice/lit-element';
import { UmbPropertyEditorUiElement } from '@umbraco-cms/backoffice/extension-registry';
import {
	UmbPropertyEditorConfigCollection,
	UmbPropertyValueChangeEvent,
} from '@umbraco-cms/backoffice/property-editor';

const ELEMENT_NAME = 'contentment-property-editor-ui-manifest-picker';

@customElement(ELEMENT_NAME)
export class ContentmentPropertyEditorUIManifestPickerElement
	extends UmbLitElement
	implements UmbPropertyEditorUiElement
{
	#extensionType?: string;

	#maxItems = Infinity;

	@property({ type: Array })
	public set value(value: Array<string> | string | undefined) {
		this.#value = Array.isArray(value) ? value : value ? [value] : [];
	}
	public get value(): Array<string> | undefined {
		return this.#value;
	}
	#value?: Array<string> = [];

	set config(config: UmbPropertyEditorConfigCollection | undefined) {
		if (!config) return;

		this.#extensionType = config.getValueByAlias('extensionType');
		this.#maxItems = parseInt(config.getValueByAlias('maxItems')) || Infinity;
	}

	#onChoose(event: CustomEvent & { target: { value: { value: string } } }) {
		const { value } = event.target.value;
		this.#setValue([value], this.value?.length ?? 0);
	}

	async #removeItem(item: string, index: number) {
		if (!item || !this.value || index == -1) return;

		const tmp = [...this.value];
		tmp.splice(index, 1);
		this.value = tmp;

		this.dispatchEvent(new UmbPropertyValueChangeEvent());
	}

	#setValue(value: Array<string> | undefined, index: number) {
		if (!value || index === -1) return;

		if (!this.value) {
			this.value = [];
		}

		const tmp = [...this.value];
		tmp.splice(index, 0, ...value);
		this.value = tmp;

		this.dispatchEvent(new UmbPropertyValueChangeEvent());
	}

	render() {
		if (!this.#extensionType) return html`<pre><code>Missing configuration for the extension type.</code></pre>`;
		return html`${this.#renderItems()} ${this.#renderAddButton()}`;
	}

	#renderAddButton() {
		if (!this.#extensionType || (this.value && this.value.length >= this.#maxItems)) return nothing;
		return html`
			<umb-input-manifest extension-type=${this.#extensionType} @change=${this.#onChoose}></umb-input-manifest>
		`;
	}

	#renderItems() {
		if (!this.#extensionType || !this.value) return;
		return html`
			<uui-ref-list>
				${repeat(
					this.value,
					(alias) => alias,
					(alias, index) => this.#renderItem(alias, index)
				)}
			</uui-ref-list>
		`;
	}

	#renderItem(alias: string, index: number) {
		return html`
			<uui-ref-node name="${alias}" ?standalone=${this.#maxItems === 1}>
				<umb-icon slot="icon" name="icon-contentment"></umb-icon>
				<uui-action-bar slot="actions">
					<uui-button
						label=${this.localize.term('general_remove')}
						@click=${() => this.#removeItem(alias, index)}></uui-button>
				</uui-action-bar>
			</uui-ref-node>
		`;
	}
}

export { ContentmentPropertyEditorUIManifestPickerElement as element };

declare global {
	interface HTMLElementTagNameMap {
		[ELEMENT_NAME]: ContentmentPropertyEditorUIManifestPickerElement;
	}
}
