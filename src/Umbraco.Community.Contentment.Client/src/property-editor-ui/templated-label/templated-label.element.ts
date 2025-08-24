// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

import { parseBoolean, tryHideLabel, tryMoveBeforePropertyGroup } from '../../utils/index.js';
import { customElement, html, property } from '@umbraco-cms/backoffice/external/lit';
import { UmbLitElement } from '@umbraco-cms/backoffice/lit-element';
import { UmbPropertyEditorConfigCollection } from '@umbraco-cms/backoffice/property-editor';
import type { ManifestBase } from '@umbraco-cms/backoffice/extension-api';
import type { PropertyValues } from '@umbraco-cms/backoffice/external/lit';
import type { UmbPropertyEditorUiElement } from '@umbraco-cms/backoffice/property-editor';

@customElement('contentment-property-editor-ui-templated-label')
export class ContentmentPropertyEditorUITemplatedLabelElement
	extends UmbLitElement
	implements UmbPropertyEditorUiElement
{
	#components?: Array<string>;

	#hideLabel: boolean = false;

	#hidePropertyGroup: boolean = false;

	@property({ attribute: false })
	public value?: unknown;

	set config(config: UmbPropertyEditorConfigCollection | undefined) {
		if (!config) return;
		this.#components = config.getValueByAlias('component') ?? [];
		this.#hideLabel = parseBoolean(config.getValueByAlias('hideLabel'));
		this.#hidePropertyGroup = parseBoolean(config.getValueByAlias('hidePropertyGroup'));
	}

	protected override firstUpdated(_changedProperties: PropertyValues): void {
		super.firstUpdated(_changedProperties);

		if (this.#hideLabel) {
			tryHideLabel(this);
		}

		if (this.#hidePropertyGroup) {
			tryMoveBeforePropertyGroup(this);
		}
	}

	override render() {
		if (!this.#components?.length) return html`<p>A templated label component has not been configured.</p>`;
		return html`
			<umb-extension-slot
				type="contentmentTemplatedLabelUi"
				.filter=${(manifest: ManifestBase) => this.#components?.includes(manifest.alias)}
				.props=${{ value: this.value }}>
			</umb-extension-slot>
		`;
	}
}

export { ContentmentPropertyEditorUITemplatedLabelElement as element };

declare global {
	interface HTMLElementTagNameMap {
		'contentment-property-editor-ui-templated-label': ContentmentPropertyEditorUITemplatedLabelElement;
	}
}
