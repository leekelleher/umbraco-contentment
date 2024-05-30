// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

import { customElement, property } from '@umbraco-cms/backoffice/external/lit';
import { UmbLitElement } from '@umbraco-cms/backoffice/lit-element';
import { UmbPropertyValueChangeEvent } from '@umbraco-cms/backoffice/property-editor';
import { UMB_PROPERTY_CONTEXT } from '@umbraco-cms/backoffice/property';
import type { UmbPropertyAction } from '@umbraco-cms/backoffice/property-action';
import type { UmbPropertyContext } from '@umbraco-cms/backoffice/property';

@customElement('contentment-property-action-edit-json')
export class ContentmentPropertyActionEditJsonElement extends UmbLitElement implements UmbPropertyAction {
	@property()
	value = '';

	args: any;

	#propertyContext?: UmbPropertyContext;

	constructor() {
		super();

		this.consumeContext(UMB_PROPERTY_CONTEXT, (propertyContext: UmbPropertyContext) => {
      console.log('UMB_PROPERTY_CONTEXT', propertyContext);
			this.#propertyContext = propertyContext;
		});
	}

	getHref(): Promise<string | undefined> {
		return Promise.resolve(undefined);
	}

	execute(): Promise<void> {
		console.log('execute', this.#propertyContext);

		const value = this.#propertyContext?.getValue();
		console.log('execute.value', value);

		this.#propertyContext?.setValue('value3');
		this.dispatchEvent(new UmbPropertyValueChangeEvent());

		return Promise.resolve();
	}
}

export default ContentmentPropertyActionEditJsonElement;

declare global {
	interface HTMLElementTagNameMap {
		'contentment-property-action-edit-json': ContentmentPropertyActionEditJsonElement;
	}
}
