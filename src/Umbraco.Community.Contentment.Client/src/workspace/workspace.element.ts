// SPDX-License-Identifier: MIT
// Copyright © 2025 Lee Kelleher

import { css, customElement, html, nothing, repeat, state, when } from '@umbraco-cms/backoffice/external/lit';
import { tryExecute } from '@umbraco-cms/backoffice/resources';
import { umbHttpClient } from '@umbraco-cms/backoffice/http-client';
import { MetaService } from '../api/sdk.gen.js';
import { UmbLitElement } from '@umbraco-cms/backoffice/lit-element';
import { UmbTextStyles } from '@umbraco-cms/backoffice/style';

@customElement('contentment-workspace')
export class ContentmentWorkspaceElement extends UmbLitElement {
	#links = [
		{
			icon: 'icon-book',
			name: 'Documentation',
			description: 'How to use each of the property editors.',
			url: 'https://github.com/leekelleher/umbraco-contentment/tree/develop/docs',
		},
		{
			icon: 'icon-youtube',
			name: 'Video demonstrations',
			description: 'Demos, guides and tutorials on YouTube.',
			url: 'https://www.youtube.com/playlist?list=PL8grlRt7-8oVULPYJpqido5QItRsJBt3M',
		},
		{
			icon: 'icon-chat',
			name: 'Support forum',
			description: 'Ask for help, the community is your friend.',
			url: 'https://forum.umbraco.com/tag/contentment',
		},
		{
			icon: 'icon-forking',
			name: 'Source code',
			description: 'See the code, all free and open-source.',
			url: 'https://github.com/leekelleher/umbraco-contentment',
		},
		{
			icon: 'icon-bug',
			name: 'Issue tracker',
			description: 'Found a bug? Suggest a feature? Let me know.',
			url: 'https://github.com/leekelleher/umbraco-contentment/issues/new/choose',
		},
		{
			icon: 'icon-vcard',
			name: 'License',
			description: 'Licensed under the MIT License.',
			url: 'https://opensource.org/licenses/MIT',
		},
	];

	@state()
	private _telemetryDisabled = false;

	@state()
	private _headline?: string;

	@state()
	private _version?: string;

	protected override async firstUpdated() {
		const { data } = await tryExecute(this, MetaService.getConfiguration({ client: umbHttpClient }));
		if (data) {
			this._headline = data.name || 'Contentment';
			this._version = data.version || '0.0.0';
			this._telemetryDisabled = data.features?.disableTelemetry ?? false;
		}
	}

	override render() {
		return html`
			<umb-body-layout>
				<div slot="header">
					<h3>${this._headline}</h3>
					<p>v${this._version}</p>
				</div>
				<div slot="navigation"><lee-was-here></lee-was-here></div>
				<div id="layout">
					<div>${this.#renderSponsorship()} ${this.#renderFeatureOptions()}</div>
					<div>${this.#renderLinks()}</div>
				</div>
			</umb-body-layout>
		`;
	}

	#renderLinks() {
		if (!this.#links?.length) return nothing;
		return html`
			<uui-box headline="Useful links">
				<div>
					<uui-ref-list>
						${repeat(
							this.#links,
							(item) => item.url,
							(item) => html`
								<uui-ref-node .name=${item.name} .detail=${item.description} .href=${item.url} target="_blank">
									<umb-icon slot="icon" name=${item.icon}></umb-icon>
								</uui-ref-node>
							`
						)}
					</uui-ref-list>
				</div>
			</uui-box>
		`;
	}

	#renderFeatureOptions() {
		return html`
			<uui-box headline="Feature options">
				<div class="uui-text">
					${when(
						this._telemetryDisabled,
						() => html`
							<h5>Telemetry</h5>
							<p><em>The telemetry feature has been disabled.</em> 🤐</p>
						`,
						() => html`
							<h5>Telemetry</h5>
							<p>
								By default, the package sends telemetry data about which property-editors are being used (from
								Contentment only). For more details about the data being captured and transparency on the analysis,
								please visit
								<a href="https://leekelleher.com/umbraco/contentment/telemetry/" target="_blank" rel="noopener"
									><strong>leekelleher.com/umbraco/contentment/telemetry</strong></a
								>.
							</p>
							<p>
								If you would prefer to opt-out and disable the telemetry feature, you can
								<a
									href="https://github.com/leekelleher/umbraco-contentment/blob/develop/docs/telemetry.md#disable-telemetry-feature"
									target="_blank"
									rel="noopener"
									><strong>find a code snippet</strong> on the telemetry documentation page</a
								>.
							</p>
						`
					)}

					<hr />

					<h5>Tree dashboard</h5>
					<p>
						If you would like to remove this page and tree item from the Settings section, you can
						<a
							href="https://github.com/leekelleher/umbraco-contentment/blob/develop/docs/tree-dashboard.md#disable-tree-dashboard"
							target="_blank"
							rel="noopener"
							><strong>find a code snippet</strong> on the tree dashboard documentation page</a
						>.
					</p>
				</div>
			</uui-box>
		`;
	}

	#renderSponsorship() {
		return html`
			<uui-box headline="Sponsorship">
				<div slot="header-actions">
					<a href="https://github.com/sponsors/leekelleher?o=esb" target="_blank">
						<umb-icon name="icon-github" style="font-size: var(--uui-size-6);"></umb-icon>
					</a>
				</div>

				<div style="display:flex;">
					<div>
						<p>
							Whilst Contentment is a <strong>free</strong> and <strong>open-source</strong> package, I have invested a
							huge amount of time and effort into its product design, development and maintenance. If you enjoy using
							these powerful property-editors and want to see me continue my efforts with more quality editors,
							<a href="https://github.com/sponsors/leekelleher?o=esb" target="_blank" rel="noopener"
								><strong>please consider supporting my development with a GitHub sponsorship</strong>.</a
							>
						</p>
						<p>
							I'd like to say heartfelt <strong>thank you</strong> to all my existing sponsors, I appreciate your
							support.
						</p>
					</div>
					<div style="flex: 0 0 auto;">
						<a href="https://github.com/sponsors/leekelleher?o=esb" target="_blank" rel="noopener"
							><img
								class="umb-avatar--xl"
								src="data:image/png;base64,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"
						/></a>
					</div>
				</div>
			</uui-box>
		`;
	}

	static override styles = [
		UmbTextStyles,
		css`
			umb-body-layout {
				div[slot='header'] {
					h3,
					p {
						margin: 0 var(--uui-size-layout-1);
					}
				}

				div[slot='navigation'] {
					padding-right: var(--uui-size-layout-3);
					padding-top: var(--uui-size-space-4);
				}
			}

			#layout {
				display: grid;
				gap: var(--uui-size-layout-1);
				grid-template-columns: 1fr 350px;

				> div {
					display: flex;
					flex-direction: column;
					gap: var(--uui-size-layout-1);
				}
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
