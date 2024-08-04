// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

import { CONTENTMENT_CONFIGURATION_EDITOR_WORKSPACE_MODAL } from './configuration-editor-workspace-modal.element.js';
import { ContentmentConfigurationEditorModel, ContentmentConfigurationEditorValue } from '../types.js';
import { css, customElement, html, ifDefined, repeat, state, when } from '@umbraco-cms/backoffice/external/lit';
import { debounce } from '@umbraco-cms/backoffice/utils';
import { umbFocus } from '@umbraco-cms/backoffice/lit-element';
import { UmbModalBaseElement, UmbModalToken, UMB_MODAL_MANAGER_CONTEXT } from '@umbraco-cms/backoffice/modal';
import { UUIInputEvent } from '@umbraco-cms/backoffice/external/uui';

interface ContentmentConfigurationEditorSelectionModalData {
	items: Array<ContentmentConfigurationEditorModel>;
}

export const CONTENTMENT_CONFIGURATION_EDITOR_SELECTION_MODAL = new UmbModalToken<
	ContentmentConfigurationEditorSelectionModalData,
	ContentmentConfigurationEditorValue
>('Umb.Contentment.Modal.ConfigurationEditor.Selection', {
	modal: {
		type: 'sidebar',
		size: 'medium',
	},
});

const ELEMENT_NAME = 'contentment-property-editor-ui-configuration-editor-selection-modal';

@customElement(ELEMENT_NAME)
export class ContentmentPropertyEditorUIConfigurationEditorSelectionModalElement extends UmbModalBaseElement<
	ContentmentConfigurationEditorSelectionModalData,
	ContentmentConfigurationEditorValue
> {
	#emptyGroup = (Math.random() + 1).toString(36).substring(7);

	@state()
	private _grouped?: Array<{ alias: string; items: Array<ContentmentConfigurationEditorModel> }>;

	#modalManager?: typeof UMB_MODAL_MANAGER_CONTEXT.TYPE;

	constructor() {
		super();

		this.consumeContext(UMB_MODAL_MANAGER_CONTEXT, (modalManager) => {
			this.#modalManager = modalManager;
		});
	}

	connectedCallback() {
		super.connectedCallback();
		this.#groupItems(this.data?.items);
	}

	#debouncedFilter = debounce((query: string) => {
		if (!this.data) return;
		const items = query ? this.data.items.filter((item) => item.name.toLowerCase().includes(query)) : this.data.items;
		this.#groupItems(items);
	}, 500);

	#groupItems(items: Array<ContentmentConfigurationEditorModel> | null | undefined) {
		if (!items) return;

		// @ts-expect-error
		const grouped = Object.groupBy(
			items,
			(item: ContentmentConfigurationEditorModel) => item.group ?? this.#emptyGroup
		);
		const entries = Object.entries(grouped) as Array<[string, Array<ContentmentConfigurationEditorModel>]>;
		this._grouped = entries.map(([alias, items]) => ({ alias, items }));
	}

	async #onChoose(model: ContentmentConfigurationEditorModel) {
		if (!model.fields?.length) {
			this.value = { key: model.key, value: {} };
			this._submitModal();
			return;
		}

		if (!this.#modalManager) return;

		const item = {
			key: model.key,
			value: model.defaultValues ?? {},
		};

		const modal = this.#modalManager.open(this, CONTENTMENT_CONFIGURATION_EDITOR_WORKSPACE_MODAL, {
			data: { item, model },
		});

		const data = await modal.onSubmit().catch(() => undefined);

		if (!data) return;

		this.value = data;
		this._submitModal();
	}

	#onInput(event: UUIInputEvent) {
		const query = (event.target.value as string) || '';
		this.#debouncedFilter(query.toLowerCase());
	}

	render() {
		return html`
			<umb-body-layout headline=${this.localize.term('general_choose')}>
				${this.#renderFilter()} ${this.#renderGroups()}
				<div slot="actions">
					<uui-button label=${this.localize.term('general_cancel')} @click=${this._rejectModal}></uui-button>
				</div>
			</umb-body-layout>
		`;
	}

	#renderFilter() {
		const label = this.localize.term('placeholders_filter');
		return html`
			<uui-input type="search" id="filter" label=${label} placeholder=${label} @input=${this.#onInput} ${umbFocus()}>
				<uui-icon name="search" slot="prepend" id="filter-icon"></uui-icon>
			</uui-input>
		`;
	}

	#renderGroups() {
		if (!this._grouped?.length) return html`<uui-box><p>No items found</p></uui-box>`;
		return html`
			<uui-box>
				${repeat(
					this._grouped,
					(group) => group.alias,
					(group) => html`
						${when(group.alias !== this.#emptyGroup, () => html`<h4>${group.alias}</h4>`)}
						<uui-ref-list>
							${repeat(
								group.items,
								(item) => item.key,
								(item) => html`
									<umb-ref-item
										name=${item.name}
										detail=${ifDefined(item.description)}
										icon=${ifDefined(item.icon)}
										@click=${() => this.#onChoose(item)}>
									</umb-ref-item>
								`
							)}
						</uui-ref-list>
					`
				)}
			</uui-box>
		`;
	}

	static styles = [
		css`
			h4 {
				margin-top: 0.5rem;
				margin-bottom: 0.5rem;
			}

			#filter {
				width: 100%;
				margin-bottom: var(--uui-size-space-4);
			}

			#filter-icon {
				display: flex;
				color: var(--uui-color-border);
				height: 100%;
				padding-left: var(--uui-size-space-2);
			}
		`,
	];
}

export { ContentmentPropertyEditorUIConfigurationEditorSelectionModalElement as element };

declare global {
	interface HTMLElementTagNameMap {
		[ELEMENT_NAME]: ContentmentPropertyEditorUIConfigurationEditorSelectionModalElement;
	}
}
