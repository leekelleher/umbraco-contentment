// SPDX-License-Identifier: MIT
// Copyright © 2025 Lee Kelleher

import { css, customElement, html, property, repeat, state } from '@umbraco-cms/backoffice/external/lit';
import {
	UmbContentTypeContainerStructureHelper,
	UmbContentTypePropertyStructureHelper,
	UMB_PROPERTY_STRUCTURE_WORKSPACE_CONTEXT,
} from '@umbraco-cms/backoffice/content-type';
import { UmbLitElement } from '@umbraco-cms/backoffice/lit-element';
import type {
	UmbContentTypeModel,
	UmbContentTypeStructureManager,
	UmbPropertyTypeContainerMergedModel,
} from '@umbraco-cms/backoffice/content-type';

// Side-effect: registers umb-content-workspace-property (from global-components)
import '@umbraco-cms/backoffice/content';

@customElement('contentment-element-workspace-tab')
export class ContentmentElementWorkspaceTabElement extends UmbLitElement {
	@property({ type: String })
	public get containerId(): string | null | undefined {
		return this._containerId;
	}
	public set containerId(value: string | null | undefined) {
		this._containerId = value;
		this.#groupHelper.setContainerId(value ?? null);
		this.#getOrCreatePropertyHelper(value ?? null);
	}

	@state()
	private _containerId?: string | null;

	@state()
	private _groups: Array<UmbPropertyTypeContainerMergedModel> = [];

	@state()
	private _hasRootProperties = false;

	#structure?: UmbContentTypeStructureManager<UmbContentTypeModel>;

	readonly #groupHelper = new UmbContentTypeContainerStructureHelper<UmbContentTypeModel>(this);

	readonly #propertyHelpers = new Map<string | null, UmbContentTypePropertyStructureHelper<UmbContentTypeModel>>();

	readonly #propertyAliasesMap = new Map<string | null, Array<string>>();

	constructor() {
		super();

		this.consumeContext(UMB_PROPERTY_STRUCTURE_WORKSPACE_CONTEXT, (ctx) => {
			this.#structure = ctx?.structure as unknown as UmbContentTypeStructureManager<UmbContentTypeModel>;
			this.#groupHelper.setStructureManager(this.#structure);
			this.#propertyHelpers.forEach((helper) => {
				if (this.#structure) helper.setStructureManager(this.#structure);
			});
		});

		this.#groupHelper.setContainerChildType('Group');

		this.observe(this.#groupHelper.childContainers, (groups) => {
			this._groups = groups ?? [];
			groups?.forEach((group) => this.#getOrCreatePropertyHelper(group.ids[0]));
		});

		this.observe(this.#groupHelper.hasProperties, (has) => {
			this._hasRootProperties = has ?? false;
		});
	}

	#getOrCreatePropertyHelper(containerId: string | null): UmbContentTypePropertyStructureHelper<UmbContentTypeModel> {
		if (!this.#propertyHelpers.has(containerId)) {
			const helper = new UmbContentTypePropertyStructureHelper<UmbContentTypeModel>(this);
			if (this.#structure) helper.setStructureManager(this.#structure);
			helper.setContainerId(containerId);
			this.#propertyHelpers.set(containerId, helper);
			this.observe(
				helper.propertyAliases,
				(aliases) => {
					this.#propertyAliasesMap.set(containerId, aliases ?? []);
					this.requestUpdate();
				},
				null,
			);
		}
		return this.#propertyHelpers.get(containerId)!;
	}

	override render() {
		const currentContainerId = this._containerId ?? null;
		const rootAliases = this.#propertyAliasesMap.get(currentContainerId) ?? [];

		return html`
			${this._hasRootProperties
				? html`
						<uui-box>
							${repeat(
								rootAliases,
								(alias) => alias,
								(alias) =>
									html`<umb-content-workspace-property
										class="property"
										.alias=${alias}></umb-content-workspace-property>`,
							)}
						</uui-box>
					`
				: ''}
			${repeat(
				this._groups,
				(group) => group.key,
				(group) => {
					const groupId = group.ids[0];
					const aliases = this.#propertyAliasesMap.get(groupId) ?? [];
					return html`
						<uui-box .headline=${this.localize.string(group.name) ?? ''}>
							${repeat(
								aliases,
								(alias) => alias,
								(alias) =>
									html`<umb-content-workspace-property
										class="property"
										.alias=${alias}></umb-content-workspace-property>`,
							)}
						</uui-box>
					`;
				},
			)}
		`;
	}

	static override styles = [
		css`
			uui-box {
				--uui-box-default-padding: 0 var(--uui-size-space-5);
			}
			uui-box:not(:first-child) {
				margin-top: var(--uui-size-layout-1);
			}
		`,
	];
}

export { ContentmentElementWorkspaceTabElement as element };

declare global {
	interface HTMLElementTagNameMap {
		'contentment-element-workspace-tab': ContentmentElementWorkspaceTabElement;
	}
}
