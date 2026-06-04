// SPDX-License-Identifier: MIT
// Copyright © 2025 Lee Kelleher

import { customElement, html, state, when } from '@umbraco-cms/backoffice/external/lit';
import { ContentmentElementManager } from './element-manager.context.js';
import { UmbContentTypeContainerStructureHelper } from '@umbraco-cms/backoffice/content-type';
import {
	UmbModalBaseElement,
	UMB_MODAL_MANAGER_CONTEXT,
	UMB_DISCARD_CHANGES_MODAL,
} from '@umbraco-cms/backoffice/modal';
import type {
	ContentmentElementWorkspaceModalData,
	ContentmentElementWorkspaceModalValue,
} from './element-workspace-modal.token.js';
import type { UmbPropertyTypeContainerMergedModel } from '@umbraco-cms/backoffice/content-type';

import './element-workspace-tab.element.js';

@customElement('contentment-element-workspace-modal')
export class ContentmentElementWorkspaceModalElement extends UmbModalBaseElement<
	ContentmentElementWorkspaceModalData,
	ContentmentElementWorkspaceModalValue
> {
	readonly #manager = new ContentmentElementManager(this);
	readonly #tabsHelper = new UmbContentTypeContainerStructureHelper<never>(this);

	@state()
	private _tabs: Array<UmbPropertyTypeContainerMergedModel> = [];

	@state()
	private _hasRootProperties = false;

	@state()
	private _activeContainerId: string | null | undefined = null;

	@state()
	private _headline = '';

	@state()
	private _icon = 'icon-document-line';

	@state()
	private _ready = false;

	get #readonly(): boolean {
		return this.data?.readonly === true;
	}

	override connectedCallback(): void {
		super.connectedCallback();
		this.#init();
	}

	async #init(): Promise<void> {
		if (!this.data) return;

		const { elementType, key, value } = this.data.element;

		await this.#manager.init(elementType, key, value, this.#readonly);
		this.#manager.setup(this);

		this.#tabsHelper.setIsRoot(true);
		this.#tabsHelper.setContainerChildType('Tab');
		this.#tabsHelper.setStructureManager(this.#manager.structure as never);

		this.observe(this.#tabsHelper.childContainers, (tabs) => {
			this._tabs = tabs ?? [];
			if (this._activeContainerId === null && tabs?.length) {
				this._activeContainerId = tabs[0].ids[0] ?? null;
			}
		});

		this.observe(this.#tabsHelper.hasProperties, (has) => {
			this._hasRootProperties = has ?? false;
		});

		const contentType = this.#manager.structure.getOwnerContentType();
		this._headline = contentType?.name ?? '';
		this._icon = contentType?.icon ?? 'icon-document-line';
		this._ready = true;
	}

	async #onSubmit(): Promise<void> {
		try {
			await this.#manager.validation.validate();
		} catch {
			return;
		}
		const { elementType, key } = this.data!.element;
		this.value = { element: { elementType, key, value: this.#manager.getResult() } };
		this._submitModal();
	}

	async #onCancel(): Promise<void> {
		if (!this.#readonly && this.#manager.getHasUnpersistedChanges()) {
			const modalManager = await this.getContext(UMB_MODAL_MANAGER_CONTEXT);
			const discardModal = modalManager?.open(this, UMB_DISCARD_CHANGES_MODAL);
			try {
				await discardModal?.onSubmit();
			} catch {
				return;
			}
		}
		this._rejectModal();
	}

	override render() {
		if (!this._ready) {
			return html`<umb-body-layout .headline=${this._headline}><uui-loader></uui-loader></umb-body-layout>`;
		}

		const showTabStrip = this._tabs.length > 0 || this._hasRootProperties;

		return html`
			<umb-body-layout main-no-padding .headline=${this._headline}>
				<umb-icon slot="icon" .name=${this._icon}></umb-icon>

				<umb-body-layout header-fit-height>
					${when(
						showTabStrip,
						() => html`
							<uui-tab-group slot="header">
								${when(
									this._hasRootProperties,
									() => html`
										<uui-tab
											label=${this.localize.term('general_content')}
											.active=${this._activeContainerId === null}
											@click=${() => {
												this._activeContainerId = null;
											}}></uui-tab>
									`,
								)}
								${this._tabs.map(
									(tab) => html`
										<uui-tab
											.label=${this.localize.string(tab.name) ?? ''}
											.active=${this._activeContainerId === tab.ids[0]}
											@click=${() => {
												this._activeContainerId = tab.ids[0] ?? null;
											}}></uui-tab>
									`,
								)}
							</uui-tab-group>
						`,
					)}

					<contentment-element-workspace-tab .containerId=${this._activeContainerId}>
					</contentment-element-workspace-tab>
				</umb-body-layout>

				<div slot="actions">
					<uui-button label=${this.localize.term('general_cancel')} @click=${this.#onCancel}></uui-button>
					${when(
						!this.#readonly,
						() => html`
							<uui-button
								color="positive"
								look="primary"
								label=${this.localize.term('general_update')}
								@click=${this.#onSubmit}></uui-button>
						`,
					)}
				</div>
			</umb-body-layout>
		`;
	}
}

export { ContentmentElementWorkspaceModalElement as element };

declare global {
	interface HTMLElementTagNameMap {
		'contentment-element-workspace-modal': ContentmentElementWorkspaceModalElement;
	}
}
