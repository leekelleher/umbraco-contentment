// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

import { classMap, css, customElement, html, property, repeat, state } from '@umbraco-cms/backoffice/external/lit';
import { UmbChangeEvent } from '@umbraco-cms/backoffice/event';
import { UmbLitElement } from '@umbraco-cms/backoffice/lit-element';
import { UMB_PROPERTY_DATASET_CONTEXT } from '@umbraco-cms/backoffice/property';
import type { UmbPropertyEditorUiElement } from '@umbraco-cms/backoffice/property-editor';
import type { ContentmentInfoBoxElementType } from '../../components/info-box/info-box.element.js';
import { ContentmentPropertyEditorUITemplatedListElement } from '../templated-list/templated-list.element.js';

@customElement('contentment-property-editor-ui-editor-note-styles')
export class ContentmentPropertyEditorUIEditorNoteStylesElement
	extends UmbLitElement
	implements UmbPropertyEditorUiElement
{
	#items: Array<ContentmentInfoBoxElementType> = [
		'default',
		'positive',
		'warning',
		'danger',
		//'transparent',
		// 'border',
		// 'current',
		// 'divider',
		// 'selected',
	];

	@state()
	private _icon?: string;

	@state()
	private _heading?: string;

	@state()
	private _message?: string;

	@property()
	value?: string;

	constructor() {
		super();

		this.consumeContext(UMB_PROPERTY_DATASET_CONTEXT, async (context) => {
			this.observe(await context?.propertyValueByAlias<string>('icon'), (icon) => (this._icon = icon), '_observeIcon');
			this.observe(
				await context?.propertyValueByAlias<string>('heading'),
				(heading) => (this._heading = heading),
				'_observeHeading'
			);
			this.observe(
				await context?.propertyValueByAlias('message'),
				(message) => (this._message = (message as any)?.markup ?? message),
				'_observeMessage'
			);
		});
	}

	#onClick(value: string) {
		this.value = value;
		this.dispatchEvent(new UmbChangeEvent());
	}

	override render() {
		return html`
			<ul class="row">
				${repeat(
					this.#items,
					(item) => item,
					(item) => this.#renderItem(item)
				)}
			</ul>
		`;
	}

	#renderItem(item: ContentmentInfoBoxElementType) {
		const heading = this._heading || (!this._message ? item : undefined);
		return html`
			<li class=${classMap({ selected: item === this.value })}>
				<button aria-label=${item} title=${item} @click=${() => this.#onClick(item)}>
					<contentment-info-box
						.type=${item}
						.icon=${this._icon}
						.headline=${heading}
						.message=${this._message}></contentment-info-box>
				</button>
			</li>
		`;
	}

	static override styles = [
		ContentmentPropertyEditorUITemplatedListElement.styles,
		css`
			ul > li > button > contentment-info-box {
				min-width: var(--uui-size-100);
			}
		`,
	];
}

export { ContentmentPropertyEditorUIEditorNoteStylesElement as element };

declare global {
	interface HTMLElementTagNameMap {
		'contentment-property-editor-ui-editor-note-styles': ContentmentPropertyEditorUIEditorNoteStylesElement;
	}
}
