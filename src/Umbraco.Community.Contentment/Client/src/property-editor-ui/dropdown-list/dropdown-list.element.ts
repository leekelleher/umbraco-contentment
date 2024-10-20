import { parseBoolean } from '../../utils/parse-boolean.function.js';
import {
	css,
	customElement,
	html,
	ifDefined,
	property,
	repeat,
	state,
	unsafeHTML,
	when,
} from '@umbraco-cms/backoffice/external/lit';
import { UmbLitElement } from '@umbraco-cms/backoffice/lit-element';
import { UmbPropertyValueChangeEvent } from '@umbraco-cms/backoffice/property-editor';
import type { ContentmentDataListItem } from '../types.js';
import type { UmbPropertyEditorConfigCollection } from '@umbraco-cms/backoffice/property-editor';
import type { UmbPropertyEditorUiElement } from '@umbraco-cms/backoffice/extension-registry';
import type { UUIComboboxElement } from '@umbraco-cms/backoffice/external/uui';

const ELEMENT_NAME = 'contentment-property-editor-ui-dropdown-list';

@customElement(ELEMENT_NAME)
export class ContentmentPropertyEditorUIDropdownListElement
	extends UmbLitElement
	implements UmbPropertyEditorUiElement
{
	@property()
	public set value(value: Array<string> | string | undefined) {
		this.#value = Array.isArray(value) === true ? value[0] : value ?? '';
	}
	public get value(): string | undefined {
		return this.#value;
	}
	#value?: string = '';

	public set config(config: UmbPropertyEditorConfigCollection | undefined) {
		if (!config) return;
		this._items = config.getValueByAlias<Array<ContentmentDataListItem>>('items') ?? [];
		this._showDescriptions = parseBoolean(config.getValueByAlias('showDescriptions'));
		this._showIcons = parseBoolean(config.getValueByAlias('showIcons'));
	}

	@state()
	private _items: Array<ContentmentDataListItem> = [];

	@state()
	private _showDescriptions = false;

	@state()
	private _showIcons = false;

	#onChange(event: CustomEvent & { target: UUIComboboxElement }) {
		if (event.target.nodeName !== 'UUI-COMBOBOX') return;
		this.value = event.target.value as string;
		this.dispatchEvent(new UmbPropertyValueChangeEvent());
	}

	override render() {
		return html`
			<uui-combobox value=${ifDefined(this.value)} @change=${this.#onChange}>
				<uui-combobox-list>
					${repeat(
						this._items,
						(item) => item.value,
						(item) => this.#renderItem(item)
					)}
				</uui-combobox-list>
			</uui-combobox>
		`;
	}

	#renderItem(item: ContentmentDataListItem) {
		return html`
			<uui-combobox-list-option display-value=${item.name} value=${item.value} ?disabled=${item.disabled}>
				<div class="outer">
					${when(this._showIcons && item.icon, () => html`<umb-icon .name=${item.icon}></umb-icon>`)}
					<uui-form-layout-item>
						<span slot="label">${this.localize.string(item.name)}</span>
						${when(
							this._showDescriptions && item.description,
							() => html`<span slot="description">${unsafeHTML(item.description)}</span>`
						)}
					</uui-form-layout-item>
				</div>
			</uui-combobox-list-option>
		`;
	}

	static override styles = [
		css`
			uui-combobox {
				width: 100%;
			}

			.outer {
				display: flex;
				flex-direction: row;
				gap: 0.5rem;
				align-items: flex-start;
			}

			uui-combobox-list-option {
				padding-top: 5px;
			}

			uui-form-layout-item {
				margin-top: 3px;
				margin-bottom: 0;
			}

			umb-icon {
				font-size: 1.2rem;
			}
		`,
	];
}

export { ContentmentPropertyEditorUIDropdownListElement as element };

declare global {
	interface HTMLElementTagNameMap {
		[ELEMENT_NAME]: ContentmentPropertyEditorUIDropdownListElement;
	}
}
