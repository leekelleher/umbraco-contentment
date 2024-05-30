// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

import { css, customElement, html, state, unsafeHTML, when } from '@umbraco-cms/backoffice/external/lit';
import { DOMPurify } from '@umbraco-cms/backoffice/external/dompurify';
import { Marked } from '@umbraco-cms/backoffice/external/marked';
import { UmbLitElement } from '@umbraco-cms/backoffice/lit-element';
import { UmbTextStyles } from '@umbraco-cms/backoffice/style';
import { UmbPropertyEditorConfigCollection } from '@umbraco-cms/backoffice/property-editor';

import '../../property-editor-ui/editor-notes/editor-notes.element.js';

const UmbMarked = new Marked({ gfm: true, breaks: true });

const elementName = 'umb-bellissima-status-dashboard-element';
@customElement(elementName)
export class UmbBellissimaStatusDashboardElement extends UmbLitElement {
	#started = 12;
	#total = 63;
	#percentage = Math.floor((this.#started / this.#total) * 100);

	#editorNoteConfig = new UmbPropertyEditorConfigCollection([
		{ alias: 'alertType', value: 'divider' },
		{ alias: 'heading', value: 'Status update for Contentment v6.0.0-alpha001' },
		{ alias: 'icon', value: 'icon-contentment' },
		{
			alias: 'message',
			value: {
				markup: `
<p>During the alpha phase of Contentment v6.0, this dashboard will appear to provide a status update of progress on Contentment for Umbraco Bellissima.</p>
<p>Once the development is out of the alpha phase, this dashboard <strong>will be removed</strong> from stable releases.</p>
<p>Development on <strong>${this.#started} of ${this.#total}</strong> UI components has been commenced; package migration is <strong>${this.#percentage}% complete</strong>.</p>
<uui-progress-bar progress="${this.#percentage}" style="background-color:var(--uui-palette-grey)"></uui-progress-bar>`,
			},
		},
	]);

	#emojis: { [key: string]: string } = {
		':no_entry_sign:': '🚫',
		':grey_question:': '❔',
		':thinking:': '🤔',
		':octocat:': '🐱',
		':green_circle:': '🟢',
		':large_blue_circle:': '🔵',
		':warning:': '⚠️',
	};

	#gfm = `
### Property editors

| :octocat:       | Editor          | Status      | Comment |
| --------------- | --------------- | ----------- | ------- |
| :green_circle: | **Bytes**           | **Done** | Implemented as standalone component, doesn't reuse Umbraco's Label editor. |
| :green_circle:  | **Code Editor**     | **Started** | Property editor built; configuration needs more work. |
| :no_entry_sign:  | Content Blocks  | _Pending_   | Considering dropping; potentially migrate to Block List? |
| :green_circle: | **Data List**       | **Done** | Property-editor work is done, **BUT!** The data-type configuration depends on the internal **Configuration Editor**, which is still being worked on, _(see below)_. |
| :grey_question: | Data Picker     | _Pending_   | I haven't thought about it yet. |
| :large_blue_circle:  | **Editor Notes**    | **Started** | Property editor built; configuration needs more work. |
| :green_circle: | **Icon Picker**     | **Done** | Implemented to reuse Umbraco's internal Icon Picker editor. |
| :grey_question: | List Items      | _Pending_   | I haven't thought about it yet. |
| :large_blue_circle:  | **Notes**           | **Started** | Property editor built; configuration needs more work. |
| :grey_question: | Number Input    | _Pending_   | I haven't thought about it yet. |
| :green_circle:       | **Render Macro**    | **Done** | :no_entry_sign: Macros have been deprecated in Umbraco.<br>:warning: Replaced the editor with a deprecation notice. |
| :grey_question: | Social Links    | _Pending_   | I haven't thought about it yet. |
| :thinking: | Templated Label | _Researching_   | Exploring alternative options. |
| :grey_question: | Textbox List    | _Pending_   | I haven't thought about it yet. |
| :grey_question: | Text Input      | _Pending_   | I haven't thought about it yet. |

### Internal components

Status of components used internally within Contentment.

| :octocat:       | Editor          | Status      | Comment |
| --------------- | --------------- | ----------- | ------- |
| :grey_question: | Cascading Dropdown List | _Pending_ | I haven't thought about it yet. |
| :large_blue_circle: | **Configuration Editor** | **Started** | This is the centrepiece of Contentment, there is **A LOT** of work to do here! |
| :grey_question: | Content Picker | _Pending_ | I haven't thought about it yet. |
| :grey_question: | Data Table | _Pending_ | I haven't thought about it yet. |
| :grey_question: | Dictionary Picker | _Pending_ | I haven't thought about it yet. |
| :no_entry_sign: | Macro Picker | _Pending_ | :no_entry_sign: Macros have been deprecated in Umbraco. |
| :grey_question: | Read Only | _Pending_ | I haven't thought about it yet. |
| :grey_question: | Rich Text Editor | _Pending_ | I haven't thought about it yet. |

### Data List editors

Status of list-editors used by the Data List editor.

| :octocat:       | Editor          | Status      | Comment |
| --------------- | --------------- | ----------- | ------- |
| :green_circle: | Buttons | **Done** |  |
| :green_circle: | Checkbox List | **Done** |  |
| :green_circle: | Dropdown List | **Done** |  |
| :grey_question: | Item Picker | _Pending_ | I haven't thought about it yet. |
| :green_circle: | Radio Button List | **Done** |  |
| :grey_question: | Tags | _Pending_ | I haven't thought about it yet. |
| :thinking: | Templated List | _Researching_ | Exploring alternative options. |

### Data List sources

Status of data-sources used by the Data List editor.
The majority of this work is reliant on the internal **Configuration Editor** UI.

| :octocat:       | Editor          | Status      | Comment |
| --------------- | --------------- | ----------- | ------- |
| :grey_question: | .NET Countries | _Pending_ | I haven't thought about it yet. |
| :grey_question: | .NET Currencies | _Pending_ | I haven't thought about it yet. |
| :grey_question: | .NET Enum | _Pending_ | I haven't thought about it yet. |
| :grey_question: | Examine | _Pending_ | I haven't thought about it yet. |
| :grey_question: | JSON | _Pending_ | I haven't thought about it yet. |
| :grey_question: | .NET Languages | _Pending_ | I haven't thought about it yet. |
| :grey_question: | Number Range | _Pending_ | I haven't thought about it yet. |
| :grey_question: | Physical File System | _Pending_ | I haven't thought about it yet. |
| :grey_question: | SQL | _Pending_ | I haven't thought about it yet. |
| :grey_question: | Text Delimited | _Pending_ | I haven't thought about it yet. |
| :grey_question: | Time Zone | _Pending_ | I haven't thought about it yet. |
| :grey_question: | uCssClassName | _Pending_ | I haven't thought about it yet. |
| :no_entry_sign: | Umbraco Backoffice Sections | **Deprecated** | Backoffice sections are registered client-side, unable to query on the server. |
| :grey_question: | Umbraco Content | _Pending_ | I haven't thought about it yet. |
| :grey_question: | Umbraco Content Properties | _Pending_ | I haven't thought about it yet. |
| :grey_question: | Umbraco Content Property Value | _Pending_ | I haven't thought about it yet. |
| :grey_question: | Umbraco Content Types | _Pending_ | I haven't thought about it yet. |
| :no_entry_sign: | Umbraco Content XPath | **Deprecated** | Umbraco 14 doesn't support XPath in the content cache. |
| :grey_question: | Umbraco Dictionary | _Pending_ | I haven't thought about it yet. |
| :grey_question: | Umbraco Entity | _Pending_ | I haven't thought about it yet. |
| :grey_question: | Umbraco Files | _Pending_ | I haven't thought about it yet. |
| :grey_question: | Umbraco Image Crop | _Pending_ | I haven't thought about it yet. |
| :grey_question: | Umbraco Languages | _Pending_ | I haven't thought about it yet. |
| :grey_question: | Umbraco Member Group | _Pending_ | I haven't thought about it yet. |
| :grey_question: | Umbraco Members | _Pending_ | I haven't thought about it yet. |
| :grey_question: | Umbraco Tags | _Pending_ | I haven't thought about it yet. |
| :grey_question: | Umbraco Templates | _Pending_ | I haven't thought about it yet. |
| :grey_question: | Umbraco User Group | _Pending_ | I haven't thought about it yet. |
| :grey_question: | Umbraco Users | _Pending_ | I haven't thought about it yet. |
| :grey_question: | User Defined | _Pending_ | I haven't thought about it yet. |
| :grey_question: | XML | _Pending_ | I haven't thought about it yet. |
| :grey_question: | XML Sitemap Change Frequency | _Pending_ | I haven't thought about it yet. |
| :grey_question: | XML Sitemap Priority | _Pending_ | I haven't thought about it yet. |
`;

	@state()
	private _markup?: string;

	constructor() {
		super();
	}

	firstUpdated() {
		const regex = new RegExp(Object.keys(this.#emojis).join('|'), 'gi');
		const markdown = this.#gfm.replace(regex, (matched) => this.#emojis[matched]);
		const markup = UmbMarked.parse(markdown) as string;
		this._markup = DOMPurify.sanitize(markup);
	}

	render() {
		return html`
			<umb-body-layout headline="Migration status of Contentment for Umbraco Bellissima">
				<div slot="action-menu"><uui-tag color="positive" look="placeholder">Under active development</uui-tag></div>
				${when(
					this._markup,
					() => html`
						<contentment-property-editor-ui-editor-notes
							.config=${this.#editorNoteConfig}></contentment-property-editor-ui-editor-notes>
						<uui-box>
							<div class="gfm">${unsafeHTML(this._markup)}</div>
						</uui-box>
					`,
					() => html`<uui-loader></uui-loader>`
				)}
			</umb-body-layout>
		`;
	}

	static styles = [
		UmbTextStyles,
		css`
			div[slot='action-menu'] {
				margin-right: var(--uui-size-layout-3);
			}

			contentment-property-editor-ui-editor-notes {
				display: block;
				margin-bottom: var(--uui-size-layout-1);
			}

			.gfm h3 {
				margin: 1rem 0;
				font-size: 1rem;
			}

			.gfm table {
				border-collapse: collapse;
				table-layout: fixed;
				width: 100%;
			}

			.gfm table thead {
				background-color: var(--uui-color-border);
			}

			.gfm tbody tr:nth-child(even) {
				background-color: var(--uui-color-divider);
			}

			.gfm table thead th:nth-child(1) {
				width: 2%;
			}

			.gfm table thead th:nth-child(2) {
				width: 24%;
			}

			.gfm table thead th:nth-child(3) {
				width: 24%;
			}

			.gfm table thead th:nth-child(4) {
				width: 50%;
			}

			.gfm table th {
				text-align: left;
				padding: 10px;
			}
			.gfm table td {
				padding: 10px;
			}
		`,
	];
}

export { UmbBellissimaStatusDashboardElement as element };

declare global {
	interface HTMLElementTagNameMap {
		[elementName]: UmbBellissimaStatusDashboardElement;
	}
}
