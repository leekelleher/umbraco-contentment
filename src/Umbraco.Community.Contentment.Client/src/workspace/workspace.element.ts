// SPDX-License-Identifier: MIT
// Copyright © 2025 Lee Kelleher

import { css, customElement, html, nothing, repeat, state, when } from '@umbraco-cms/backoffice/external/lit';
import { tryExecute } from '@umbraco-cms/backoffice/resources';
import { MetaService } from '../api/index.js';
import { UmbLitElement } from '@umbraco-cms/backoffice/lit-element';
import { UmbTextStyles } from '@umbraco-cms/backoffice/style';

import '../components/lee-was-here/lee-was-here.element.js';

@customElement('contentment-workspace')
export class ContentmentWorkspaceElement extends UmbLitElement {
	#links = [
		{
			icon: 'icon-book',
			name: '#contentment_dashboard_links_documentation_label',
			description: '#contentment_dashboard_links_documentation_description',
			url: 'https://github.com/leekelleher/umbraco-contentment/tree/contrib/docs',
		},
		{
			icon: 'icon-youtube',
			name: '#contentment_dashboard_links_video_label',
			description: '#contentment_dashboard_links_video_description',
			url: 'https://www.youtube.com/playlist?list=PL8grlRt7-8oVULPYJpqido5QItRsJBt3M',
		},
		{
			icon: 'icon-chat',
			name: '#contentment_dashboard_links_support_label',
			description: '#contentment_dashboard_links_support_description',
			url: 'https://forum.umbraco.com/tag/contentment',
		},
		{
			icon: 'icon-forking',
			name: '#contentment_dashboard_links_source_label',
			description: '#contentment_dashboard_links_source_description',
			url: 'https://github.com/leekelleher/umbraco-contentment',
		},
		{
			icon: 'icon-bug',
			name: '#contentment_dashboard_links_issues_label',
			description: '#contentment_dashboard_links_issues_description',
			url: 'https://github.com/leekelleher/umbraco-contentment/issues/new/choose',
		},
		{
			icon: 'icon-vcard',
			name: '#contentment_dashboard_links_license_label',
			description: '#contentment_dashboard_links_license_description',
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
		const { data } = await tryExecute(this, MetaService.getConfiguration());
		if (data) {
			this._headline = data.name || this.localize.term('contentment_title');
			this._version = data.version || '0.0.0';
			this._telemetryDisabled = data.features?.disableTelemetry ?? false;
		}
	}

	override render() {
		return html`
			<umb-body-layout>
				<div slot="header">
					<div id="headline">
						<h3>${this._headline}</h3>
						${when(this._version, (version) => html`<p><em>v${version}</em></p>`)}
					</div>
					<lee-was-here></lee-was-here>
				</div>
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
			<uui-box headline=${this.localize.term('contentment_dashboard_links_headline')}>
				<div>
					<uui-ref-list>
						${repeat(
							this.#links,
							(item) => item.url,
							(item) => html`
								<uui-ref-node
									.name=${this.localize.string(item.name)}
									.detail=${this.localize.string(item.description)}
									.href=${item.url}
									target="_blank">
									<umb-icon slot="icon" name=${item.icon}></umb-icon>
								</uui-ref-node>
							`,
						)}
					</uui-ref-list>
				</div>
			</uui-box>
		`;
	}

	#renderFeatureOptions() {
		return html`
			<uui-box headline=${this.localize.term('contentment_dashboard_features_headline')}>
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
									href="https://github.com/leekelleher/umbraco-contentment/blob/contrib/docs/telemetry.md#disable-telemetry-feature"
									target="_blank"
									rel="noopener"
									><strong>find a code snippet</strong> on the telemetry documentation page</a
								>.
							</p>
						`,
					)}

					<hr />

					<h5>Tree dashboard</h5>
					<p>
						If you would like to remove this page and tree item from the Settings section, you can
						<a
							href="https://github.com/leekelleher/umbraco-contentment/blob/contrib/docs/tree-dashboard.md#disable-tree-dashboard"
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
			<uui-box headline=${this.localize.term('contentment_dashboard_sponsorship_headline')}>
				<div slot="header-actions">
					<a href="https://github.com/sponsors/leekelleher?o=esb" target="_blank">
						<umb-icon name="icon-github"></umb-icon>
					</a>
				</div>

				<div class="intro">
					<div>
						<p>
							While Contentment is a <strong>free</strong> and <strong>open-source</strong> package, I have invested a
							huge amount of time and effort into its product design, development and maintenance. If you rely using
							these powerful property-editors and want to see me continue my efforts with more quality editors,
							<a href="https://github.com/sponsors/leekelleher?o=esb" target="_blank" rel="noopener"
								><strong>please consider supporting my open-source development with a GitHub sponsorship</strong>.</a
							>
						</p>
						<p>
							I'd like to say heartfelt <strong>thank you</strong> to all my past and present sponsors, I appreciate
							your support.
						</p>
					</div>
					<aside>
						<a href="https://github.com/sponsors/leekelleher?o=esb" target="_blank" rel="noopener"
							><img
								alt="GitHub Octocat holding a heart symbol"
								src="/App_Plugins/Contentment/github-sponsor-octocat.png"
								height="100"
								width="100"
						/></a>
					</aside>
				</div>
			</uui-box>
		`;
	}

	static override styles = [
		UmbTextStyles,
		css`
			umb-body-layout {
				div[slot='header'] {
					flex: 1;

					display: flex;
					align-items: center;
					justify-content: space-between;

					h3,
					p {
						margin: 0;
					}
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

				div[slot='header-actions'] {
					umb-icon {
						font-size: var(--uui-size-6);
					}
				}

				.intro {
					display: flex;
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
