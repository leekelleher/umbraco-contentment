import { parseInt } from '../../utils/parse-int.function.js';
import type { ContentmentDataListItem } from '../types.js';
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
import type { UmbPropertyEditorConfigCollection } from '@umbraco-cms/backoffice/property-editor';
import type { UmbPropertyEditorUiElement } from '@umbraco-cms/backoffice/extension-registry';
import type { UUIComboboxElement, UUIComboboxEvent } from '@umbraco-cms/backoffice/external/uui';
import { OpenAPI } from '@umbraco-cms/backoffice/external/backend-api';
import { request as __request } from '../../api/core/request.js';
import { tryExecuteAndNotify } from '@umbraco-cms/backoffice/resources';

const ELEMENT_NAME = 'contentment-property-editor-ui-cascading-dropdown-list';

@customElement(ELEMENT_NAME)
export class ContentmentPropertyEditorUICascadingDropdownListElement
	extends UmbLitElement
	implements UmbPropertyEditorUiElement
{
	@state()
	private _apis: Array<string> = [];

	@state()
	private _loading = true;

	@state()
	private _options: Array<Array<ContentmentDataListItem>> = [];

	@state()
	private _promises: Array<Promise<Array<ContentmentDataListItem>>> = [];

	@property({ type: Array })
	public value?: Array<string>;

	public set config(config: UmbPropertyEditorConfigCollection | undefined) {
		if (!config) return;

		this._apis = config.getValueByAlias<Array<string>>('apis') ?? [];

		if (!this.value?.length) {
			this.value = [''];
		}

		if (this.value?.length && this._apis?.length) {
			for (var i = 0; i < this.value.length; i++) {
				var url = this._apis[i];

				for (var j = 0; j < i; j++) {
					url = url.replace(`{${j}}`, this.value[j]);
				}

				this._promises.push(this.#getItems(url));
			}
		}
	}

	// TODO: [LK] Move this to its own repository.
	async #getItems(url: string) {
		const { data } = await tryExecuteAndNotify(
			this,
			__request(OpenAPI, {
				method: 'GET',
				url: url,
				errors: {
					401: 'The resource is protected and requires an authentication token',
					403: 'The authenticated user do not have access to this resource',
					404: 'Not Found',
				},
			})
		);

		return data as Array<ContentmentDataListItem>;
	}

	async #onChange(event: UUIComboboxEvent & { target: UUIComboboxElement }) {
		if (event.target.nodeName !== 'UUI-COMBOBOX') return;

		const index = parseInt(event.target.dataset.index) || 0;
		const next = index + 1;
		const value = event.target.value as string;

		const currentValue = this.value ? this.value.slice(0, next) : [];
		currentValue[index] = value;
		this.value = currentValue;

		this.dispatchEvent(new UmbPropertyValueChangeEvent());

		if (this._apis.length > next) {
			this._loading = true;

			var url = this._apis[next].replace(/{(\d+)}/g, function (match, digit) {
				return typeof currentValue[digit] != 'undefined' ? currentValue[digit] : match;
			});

			if (typeof value !== 'number' && value) {
				this._options[next] = await this.#getItems(url);
			} else {
				this._options = this._options.splice(0, next);
			}

			this._loading = false;

			requestAnimationFrame(() => {
				(this.shadowRoot?.querySelector(`uui-combobox:nth-child(${next + 1})`) as HTMLElement)?.focus();
			});
		}
	}

	protected async firstUpdated() {
		this._options = await Promise.all(this._promises);
		this._loading = false;
	}

	render() {
		if (this._loading) return html`<uui-loader></uui-loader>`;
		return html`${repeat(
			this._options,
			(options, index) => html`
				<uui-combobox data-index=${index} required value=${ifDefined(this.value?.[index])} @change=${this.#onChange}>
					<uui-combobox-list>
						${repeat(
							options,
							(option) => option.value,
							(option) => this.#renderItem(option)
						)}
					</uui-combobox-list>
				</uui-combobox>
			`
		)}`;
	}

	#renderItem(item: ContentmentDataListItem) {
		return html`
			<uui-combobox-list-option display-value=${item.name} value=${item.value} ?disabled=${item.disabled}>
				<div class="outer">
					${when(item.icon, () => html`<umb-icon .name=${item.icon}></umb-icon>`)}
					<uui-form-layout-item>
						<span slot="label">${this.localize.string(item.name)}</span>
						${when(item.description, () => html`<span slot="description">${unsafeHTML(item.description)}</span>`)}
					</uui-form-layout-item>
				</div>
			</uui-combobox-list-option>
		`;
	}

	static styles = [
		css`
			:host {
				display: flex;
				flex-direction: column;
				gap: var(--uui-size-2);
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

export { ContentmentPropertyEditorUICascadingDropdownListElement as element };

declare global {
	interface HTMLElementTagNameMap {
		[ELEMENT_NAME]: ContentmentPropertyEditorUICascadingDropdownListElement;
	}
}
