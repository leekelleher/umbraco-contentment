// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

import { css, customElement, html } from '@umbraco-cms/backoffice/external/lit';
import { UmbLitElement } from '@umbraco-cms/backoffice/lit-element';
import { UmbTextStyles } from '@umbraco-cms/backoffice/style';

@customElement('contentment-workspace')
export class ContentmentWorkspaceElement extends UmbLitElement {
	override render() {
		return html`
			<umb-body-layout headline="Contentment for Umbraco">
				<div slot="action-menu"><lee-was-here></lee-was-here></div>
				<contentment-info-box type="current" icon="icon-contentment" heading="Thank you for using Contentment v6.0">
					<p>
						If you find any bugs, or feel something is amiss, then please raise an issue on
						<a href="https://github.com/leekelleher/umbraco-contentment/issues" target="_blank"
							>the Contentment source-code repository on GitHub</a
						>.
					</p>
				</contentment-info-box>
			</umb-body-layout>
		`;
	}

	static override styles = [
		UmbTextStyles,
		css`
			div[slot='action-menu'] {
				margin-right: var(--uui-size-layout-3);
			}

			contentment-info-box {
				display: block;
				margin-bottom: var(--uui-size-layout-1);
			}
		`,
	];
}

export { ContentmentWorkspaceElement as element };

declare global {
	interface HTMLElementTagNameMap {
		'contentment-workspace': ContentmentWorkspaceElement;
	}
}
