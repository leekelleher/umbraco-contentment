// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

import { ContentmentSocialLinkValue, ContentmentSocialNetworkModel } from '../types.js';
import { css, customElement, html, repeat } from '@umbraco-cms/backoffice/external/lit';
import { UmbModalBaseElement, UmbModalToken } from '@umbraco-cms/backoffice/modal';

interface ContentmentSocialLinksSelectionModalData {
	items: Array<ContentmentSocialNetworkModel>;
}

export const CONTENTMENT_SOCIAL_LINKS_SELECTION_MODAL = new UmbModalToken<
	ContentmentSocialLinksSelectionModalData,
	ContentmentSocialLinkValue
>('Umb.Contentment.Modal.SocialLinks.Selection', {
	modal: {
		type: 'dialog',
	},
});

const ELEMENT_NAME = 'contentment-property-editor-ui-social-links-selection-modal';

@customElement(ELEMENT_NAME)
export class ContentmentPropertyEditorUISocialLinksSelectionModalElement extends UmbModalBaseElement<
	ContentmentSocialLinksSelectionModalData,
	ContentmentSocialLinkValue
> {
	#onChoose(item: ContentmentSocialNetworkModel) {
		this.value = { name: item.name, network: item.network, url: item.url };
		this._submitModal();
	}

	override render() {
		return html`
			<uui-dialog-layout .headline=${this.localize.term('contentment_selectSocialNetwork')}>
				${this.#renderItems()}
				<uui-button
					slot="actions"
					label=${this.localize.term('general_cancel')}
					@click=${this._rejectModal}></uui-button>
			</uui-dialog-layout>
		`;
	}

	#renderItems() {
		if (!this.data?.items?.length) {
			return html`
				<umb-localize key="contentment_emptySocialNetworks">There are no social networks to select.</umb-localize>
			`;
		}

		return html`
			<div id="wrapper">
				${repeat(
					this.data.items,
					(item) => item.network,
					(item) => this.#renderItem(item)
				)}
			</div>
		`;
	}

	#renderItem(item: ContentmentSocialNetworkModel) {
		return html`
			<uui-button look="outline" label="Change social network" @click=${() => this.#onChoose(item)}>
				<div class="inner">
					<div class="icon" style="background-color: ${item.backgroundColor};">
						<uui-icon name=${item.icon} style="--uui-icon-color: ${item.iconColor};"></uui-icon>
					</div>
					<span>${item.name}</span>
				</div>
			</uui-button>
		`;
	}

	static override styles = [
		css`
			#wrapper {
				display: grid;
				grid-template-columns: repeat(auto-fill, minmax(var(--uui-size-40), 1fr));
				grid-gap: var(--uui-size-layout-3);
				margin: var(--uui-size-layout-2);
				place-items: start;
				justify-content: space-between;

				max-width: 45vw;
			}

			.inner {
				display: flex;
				flex-direction: column;
				align-items: stretch;
				justify-content: space-between;
				gap: var(--uui-size-2);
				margin-top: var(--uui-size-2);
			}

			.icon {
				border-radius: var(--uui-size-2);
				font-size: var(--uui-size-layout-3);
				height: var(--uui-size-40);
				width: var(--uui-size-40);

				display: flex;
				justify-content: center;
				align-items: center;
			}
		`,
	];
}

export { ContentmentPropertyEditorUISocialLinksSelectionModalElement as element };

declare global {
	interface HTMLElementTagNameMap {
		[ELEMENT_NAME]: ContentmentPropertyEditorUISocialLinksSelectionModalElement;
	}
}
