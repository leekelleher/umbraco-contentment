// SPDX-License-Identifier: MPL-2.0
// Copyright © 2025 Lee Kelleher

import { css, customElement, html, property, when } from '@umbraco-cms/backoffice/external/lit';
import { UmbChangeEvent } from '@umbraco-cms/backoffice/event';
import { UmbLitElement } from '@umbraco-cms/backoffice/lit-element';
import { UMB_ICON_PICKER_MODAL } from '@umbraco-cms/backoffice/icon';
import { UMB_MODAL_MANAGER_CONTEXT } from '@umbraco-cms/backoffice/modal';

export type IconSize = 'large' | 'small';

@customElement('contentment-icon-picker')
export default class ContentmentIconPickerElement extends UmbLitElement {
	@property({ attribute: 'default-icon' })
	public defaultIcon?: string = '';

	@property()
	public size?: IconSize = 'large';

	@property()
	public value?: string | null;

	async #openIconPicker() {
		const modalManager = await this.getContext(UMB_MODAL_MANAGER_CONTEXT);
		const modalValue = this.#parseIcon(this.value);
		const modal = modalManager?.open(this, UMB_ICON_PICKER_MODAL, { value: modalValue });

		const picked = await modal?.onSubmit().catch(() => undefined);
		if (!picked) return;

		this.value = [picked.icon, picked.icon && picked.color ? `color-${picked.color}` : null].filter(Boolean).join(' ');

		this.dispatchEvent(new UmbChangeEvent());
	}

	#parseIcon(iconString: string | null | undefined): typeof UMB_ICON_PICKER_MODAL.VALUE {
		const [icon, color] = iconString?.split(' ') ?? [];
		return { icon, color: color?.replace('color-', '') };
	}

	override render() {
		return html`
			<uui-button
				look=${this.value ? 'outline' : 'placeholder'}
				label=${this.localize.term('defaultdialogs_selectIcon')}
				?compact=${this.size === 'small'}
				@click=${this.#openIconPicker}>
				${when(
					this.value || this.defaultIcon,
					(icon) => html`<umb-icon name=${icon}></umb-icon>`,
					() => html`<uui-icon name="add" style="--uui-icon-color: var(--uui-color-disabled-contrast);"></uui-icon>`
				)}
			</uui-button>
		`;
	}

	static override readonly styles = [
		css`
			uui-button:not([compact]) {
				font-size: var(--uui-size-layout-2);
				height: var(--uui-size-layout-4);
				width: var(--uui-size-layout-4);
			}
		`,
	];
}

declare global {
	interface HTMLElementTagNameMap {
		'contentment-icon-picker': ContentmentIconPickerElement;
	}
}
