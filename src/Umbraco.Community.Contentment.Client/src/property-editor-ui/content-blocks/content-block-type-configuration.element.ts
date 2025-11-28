// SPDX-License-Identifier: MPL-2.0
// Copyright © 2025 Lee Kelleher

import { ContentBlocksService } from '../../api/sdk.gen.js';
import { customElement } from '@umbraco-cms/backoffice/external/lit';
import { tryExecute } from '@umbraco-cms/backoffice/resources';
import { umbHttpClient } from '@umbraco-cms/backoffice/http-client';
import { ContentmentPropertyEditorUIConfigurationEditorElement } from '../configuration-editor/configuration-editor.element.js';
import type { ContentmentConfigurationEditorModel } from '../types.js';

@customElement('contentment-property-editor-ui-content-block-type-configuration')
export class ContentmentPropertyEditorUIContentBlockTypeConfigurationElement extends ContentmentPropertyEditorUIConfigurationEditorElement {
	protected override async getModels() {
		if (this.models) return;

		const { data } = await tryExecute(this, ContentBlocksService.getElementTypes({ client: umbHttpClient }));
		if (!data) return;

		this.models = data.map(
			(elementType): ContentmentConfigurationEditorModel => ({
				key: elementType.value!,
				name: elementType.name!,
				description: elementType.description,
				icon: elementType.icon,
				defaultValues: { nameTemplate: `${elementType.name}`, overlaySize: 'medium' },
				fields: [
					{
						key: 'nameTemplate',
						name: 'Name template',
						description: 'Enter (something) to evaluate against each block for its name.',
						propertyEditorUiAlias: 'Umb.PropertyEditorUi.TextBox',
					},
					{
						key: 'overlaySize',
						name: 'Editor overlay size',
						description: "Select the size of the overlay editing panel. The default is 'medium'.",
						propertyEditorUiAlias: 'Umb.PropertyEditorUi.OverlaySize',
					},
				],
				overlaySize: 'medium',
			})
		);

		this.populateModelLookup();
	}
}

export { ContentmentPropertyEditorUIContentBlockTypeConfigurationElement as element };

declare global {
	interface HTMLElementTagNameMap {
		'contentment-property-editor-ui-content-block-type-configuration': ContentmentPropertyEditorUIContentBlockTypeConfigurationElement;
	}
}
