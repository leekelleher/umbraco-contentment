// // SPDX-License-Identifier: MPL-2.0
// // Copyright © 2024 Lee Kelleher

// import { html, customElement, property } from '@umbraco-cms/backoffice/external/lit';
// import { UmbLitElement } from '@umbraco-cms/backoffice/lit-element';
// import { UmbPropertyValueChangeEvent } from '@umbraco-cms/backoffice/property-editor';
// import { UMB_PROPERTY_CONTEXT } from '@umbraco-cms/backoffice/property';
// import type { UmbPropertyAction } from '@umbraco-cms/backoffice/property-action';
// import type { UmbPropertyContext } from '@umbraco-cms/backoffice/property';

// @customElement('contentment-property-action-edit-json')
// export class ContentmentPropertyActionEditJsonElement extends UmbPropertyAction {
// 	// @property()
// 	// value = '';

// 	// args: any;

// 	// #propertyContext?: UmbPropertyContext;

// 	// constructor() {
// 	// 	super();

// 	// 	this.consumeContext(UMB_PROPERTY_CONTEXT, (propertyContext: UmbPropertyContext) => {
// 	// 		this.#propertyContext = propertyContext;
// 	// 	});
// 	// }

// 	// public getHref(): Promise<string | undefined> {
// 	// 	return Promise.resolve(undefined);
// 	// }

// 	// public execute(): Promise<void> {
// 	// 	return Promise.resolve();
// 	// }

// 	// #onClick() {
// 	// 	this.#editValue();

// 	// 	// TODO: [LK] Find out how to close the property-action menu.
// 	// 	this.dispatchEvent(new CustomEvent('close', { bubbles: true, composed: true }));
// 	// }

// 	// #editValue() {
// 	// 	// TODO: [LK] Wire up the modal for editing the JSON value.

// 	// 	const value = this.#propertyContext?.getValue();
// 	// 	console.log('#editValue', [value, this.#propertyContext?.getValue()]);

// 	// 	this.dispatchEvent(new UmbPropertyValueChangeEvent());
// 	// }

// 	// render() {
// 	// 	return html`
// 	// 		<uui-menu-item label="Edit raw value" @click-label="${this.#onClick}">
// 	// 			<uui-icon slot="icon" name="icon-brackets"></uui-icon>
// 	// 		</uui-menu-item>
// 	// 	`;
// 	// }
// }

// export default ContentmentPropertyActionEditJsonElement;

// declare global {
// 	interface HTMLElementTagNameMap {
// 		'contentment-property-action-edit-json': ContentmentPropertyActionEditJsonElement;
// 	}
// }
