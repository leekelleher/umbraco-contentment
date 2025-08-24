// SPDX-License-Identifier: MPL-2.0
// Copyright © 2023 Lee Kelleher

import { css, customElement, property, unsafeHTML } from '@umbraco-cms/backoffice/external/lit';
import { parseBoolean, tryHideLabel, tryMoveBeforePropertyGroup } from '../../utils/index.js';
import { UmbLitElement } from '@umbraco-cms/backoffice/lit-element';
import { UmbTextStyles } from '@umbraco-cms/backoffice/style';
import type { PropertyValues } from '@umbraco-cms/backoffice/external/lit';
import type {
	UmbPropertyEditorConfigCollection,
	UmbPropertyEditorUiElement,
} from '@umbraco-cms/backoffice/property-editor';

@customElement('contentment-property-editor-ui-notes')
export class ContentmentPropertyEditorUINotesElement extends UmbLitElement implements UmbPropertyEditorUiElement {
	#hideLabel: boolean = false;

	#hidePropertyGroup: boolean = false;

	#notes?: string;

	@property()
	public value?: string;

	public set config(config: UmbPropertyEditorConfigCollection | undefined) {
		if (!config) return;
		const notes = config.getValueByAlias('notes');
		this.#notes = typeof notes === 'string' ? notes : (notes as unknown as any).markup;
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
		return unsafeHTML(this.#notes);
	}

	static override styles = [
		UmbTextStyles,
		css`
			details {
				> summary {
					cursor: pointer;
				}

				&.well {
					background-color: var(--uui-color-divider);
					border: 1px solid var(--uui-color-divider-standalone);
					border-radius: var(--uui-border-radius, 3px);
					padding: var(--uui-size-space-5);
				}

				& + details {
					margin-top: var(--uui-size-space-3);
				}

				p {
					margin-bottom: 0;

					&:last-of-type {
						margin-bottom: var(--uui-size-space-3);
					}
				}
			}
		`,
	];
}

export { ContentmentPropertyEditorUINotesElement as element };

declare global {
	interface HTMLElementTagNameMap {
		'contentment-property-editor-ui-notes': ContentmentPropertyEditorUINotesElement;
	}
}
