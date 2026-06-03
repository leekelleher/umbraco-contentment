// SPDX-License-Identifier: MIT
// Copyright © 2025 Lee Kelleher

// Side-effect: registers umb-content-workspace-view-edit-tab + descendants as custom elements
import '@umbraco-cms/backoffice/content';

import { css, customElement, html, state, when } from '@umbraco-cms/backoffice/external/lit';
import { UmbModalBaseElement, UMB_MODAL_MANAGER_CONTEXT, UMB_DISCARD_CHANGES_MODAL } from '@umbraco-cms/backoffice/modal';
import { UmbTextStyles } from '@umbraco-cms/backoffice/style';
import { UmbContentTypeContainerStructureHelper } from '@umbraco-cms/backoffice/content-type';
import type { UmbPropertyTypeContainerMergedModel } from '@umbraco-cms/backoffice/content-type';
import { ContentmentElementManager } from './element-manager.context.js';
import type {
	ContentmentElementWorkspaceModalData,
	ContentmentElementWorkspaceModalValue,
} from './element-workspace-modal.token.js';

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

		const showTabStrip = this._tabs.length > 1 || (this._hasRootProperties && this._tabs.length > 0);

		return html`
			<umb-body-layout .headline=${this._headline}>
				<umb-icon slot="icon" .name=${this._icon}></umb-icon>

				${when(
					showTabStrip,
					() => html`
						<uui-tab-group slot="tabs">
							${when(
								this._hasRootProperties,
								() => html`
									<uui-tab
										label=${this.localize.term('general_content')}
										.active=${this._activeContainerId === null}
										@click=${() => {
											this._activeContainerId = null;
										}}>
									</uui-tab>
								`,
							)}
							${this._tabs.map(
								(tab) => html`
									<uui-tab
										.label=${this.localize.string(tab.name) ?? ''}
										.active=${this._activeContainerId === tab.ids[0]}
										@click=${() => {
											this._activeContainerId = tab.ids[0] ?? null;
										}}>
									</uui-tab>
								`,
							)}
						</uui-tab-group>
					`,
				)}

				<umb-content-workspace-view-edit-tab .containerId=${this._activeContainerId}>
				</umb-content-workspace-view-edit-tab>

				<div slot="actions">
					<uui-button label=${this.localize.term('general_cancel')} @click=${this.#onCancel}></uui-button>
					${when(
						!this.#readonly,
						() => html`
							<uui-button
								color="positive"
								look="primary"
								label=${this.localize.term('buttons_save')}
								@click=${this.#onSubmit}>
							</uui-button>
						`,
					)}
				</div>
			</umb-body-layout>
		`;
	}

	static override styles = [
		UmbTextStyles,
		css`
			umb-content-workspace-view-edit-tab {
				padding: var(--uui-size-layout-1);
			}
		`,
	];
}

export { ContentmentElementWorkspaceModalElement as element };

declare global {
	interface HTMLElementTagNameMap {
		'contentment-element-workspace-modal': ContentmentElementWorkspaceModalElement;
	}
}
