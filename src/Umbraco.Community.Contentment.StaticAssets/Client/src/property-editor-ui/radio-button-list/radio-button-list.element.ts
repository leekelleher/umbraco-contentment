import { html, customElement, property, state, repeat, when, css, ifDefined, } from '@umbraco-cms/backoffice/external/lit';
import { UmbLitElement } from '@umbraco-cms/backoffice/lit-element';
import { UmbPropertyValueChangeEvent } from '@umbraco-cms/backoffice/property-editor';
import type { ContentmentDataListItem } from '../types.js';
import type { UmbPropertyEditorConfigCollection } from '@umbraco-cms/backoffice/property-editor';
import type { UmbPropertyEditorUiElement } from '@umbraco-cms/backoffice/extension-registry';
import type { UUIRadioEvent } from '@umbraco-cms/backoffice/external/uui';

@customElement('contentment-property-editor-ui-radio-button-list')
export default class ContentmentPropertyEditorUIRadioButtonListElement
	extends UmbLitElement
	implements UmbPropertyEditorUiElement
{
	@property()
	value?: string = '';

	public set config(config: UmbPropertyEditorConfigCollection) {
		if (!config) return;
		this._allowClear = config.getValueByAlias('allowClear') ?? false;
		this._items = config.getValueByAlias<Array<ContentmentDataListItem>>('items') ?? [];
		this._showDescriptions = config.getValueByAlias('showDescriptions') ?? false;
		this._showIcons = config.getValueByAlias('showIcons') ?? false;
	}

	@state()
	private _allowClear = false;

	@state()
	private _items: Array<ContentmentDataListItem> = [];

	@state()
	private _showDescriptions = false;

	@state()
	private _showIcons = false;

	#onChange(event: UUIRadioEvent) {
		if (event.target.nodeName !== 'UUI-RADIO') return;
		this.value = event.target.value;
		this.dispatchEvent(new UmbPropertyValueChangeEvent());
	}

	render() {
		return html`
			<uui-radio-group .value=${this.value} @change=${this.#onChange}>
				${repeat(
					this._items,
					(item) => item,
					(item) => this.#renderRadioButton(item)
				)}
			</uui-radio-group>
		`;
	}

	#renderRadioButton(item: ContentmentDataListItem) {
		return html`
			<uui-radio value=${item.value}>
				<div class="outer">
					${when(
						this._showIcons && item.icon,
						() =>
							html`<umb-icon
								name=${ifDefined(item.icon?.split(' ')[0])}
								color=${ifDefined(item.icon?.split(' ')[1])}></umb-icon>`
					)}
					<div class="inner">
						<span>${item.name}</span>
						${when(this._showDescriptions && item.description, () => html`<small>${item.description}</small>`)}
					</div>
				</div>
			</uui-radio>
		`;
	}

	static styles = [
		css`
			.outer {
				display: flex;
				flex-direction: row;
				gap: 0.5rem;
			}
			.inner {
				display: flex;
				flex-direction: column;
			}

			umb-icon {
				font-size: 1.2rem;
			}
		`,
	];
}

declare global {
	interface HTMLElementTagNameMap {
		'contentment-property-editor-ui-radio-button-list': ContentmentPropertyEditorUIRadioButtonListElement;
	}
}
