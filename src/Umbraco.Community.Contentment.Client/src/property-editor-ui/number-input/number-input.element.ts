import { css, customElement, html, ifDefined, property, state } from '@umbraco-cms/backoffice/external/lit';
import { parseInt } from '../../utils/index.js';
import { UmbLitElement } from '@umbraco-cms/backoffice/lit-element';
import { UmbPropertyValueChangeEvent } from '@umbraco-cms/backoffice/property-editor';
import type {
	UmbPropertyEditorConfigCollection,
	UmbPropertyEditorUiElement,
} from '@umbraco-cms/backoffice/property-editor';

@customElement('contentment-property-editor-ui-number-input')
export class ContentmentPropertyEditorUINumberInputElement extends UmbLitElement implements UmbPropertyEditorUiElement {
	@property({ type: Number })
	value?: number;

	public set config(config: UmbPropertyEditorConfigCollection | undefined) {
		if (!config) return;
		this._placeholderText = config.getValueByAlias<string>('placeholderText');
		this._size = config.getValueByAlias<string>('size') ?? 's';
	}

	@state()
	private _placeholderText?: string;

	@state()
	private _size?: string;

	#onInput(event: InputEvent & { target: HTMLInputElement }) {
		this.value = parseInt(event.target.value);
		this.dispatchEvent(new UmbPropertyValueChangeEvent());
	}

	override render() {
		return html`
			<uui-input
				type="number"
				class=${this._size ?? 's'}
				pattern="[-0-9]*"
				placeholder=${ifDefined(this._placeholderText)}
				.value=${this.value}
				@input=${this.#onInput}>
			</uui-input>
		`;
	}

	static override styles = [
		css`
			.s {
				width: 10ch;
			}
			.m {
				width: 20ch;
			}
			.l {
				width: 40ch;
			}
			.xl {
				width: 100%;
			}
			uui-input {
				max-width: 100%;
			}
		`,
	];
}

export { ContentmentPropertyEditorUINumberInputElement as element };

declare global {
	interface HTMLElementTagNameMap {
		'contentment-property-editor-ui-number-input': ContentmentPropertyEditorUINumberInputElement;
	}
}
