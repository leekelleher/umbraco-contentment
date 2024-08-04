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

const ELEMENT_NAME = 'umb-bellissima-status-dashboard-element';

@customElement(ELEMENT_NAME)
export class UmbBellissimaStatusDashboardElement extends UmbLitElement {
	// alpha001 was 40 started, 63 total = 63% complete
	// alpha002 was 51 started, 41 completed, 63 total = 73% complete
	#started = 61;
	#completed = 55;
	#total = 63;
	#percentage = Math.floor(((this.#started + this.#completed) / (this.#total * 2)) * 100);

	#notesConfig = new UmbPropertyEditorConfigCollection([
		{ alias: 'alertType', value: 'current' },
		{ alias: 'heading', value: 'Status update for Contentment v6.0.0-alpha003' },
		{ alias: 'icon', value: 'icon-contentment' },
		{
			alias: 'message',
			value: {
				markup: `
<p>During the alpha phase of Contentment v6.0, this dashboard will appear to provide a status update of progress on Contentment for Umbraco Bellissima.</p>
<p>Once the development is out of the alpha phase, this dashboard <strong>will be removed</strong>, until then consider this release as <strong>unstable</strong>.</p>
<p>Development has started on <strong>${this.#started}</strong> of the <strong>${
					this.#total
				}</strong> UI components, (with <strong>${this.#completed}</strong> complete). Package migration is <strong>${
					this.#percentage
				}% complete</strong>.</p>
<uui-progress-bar progress="${this.#percentage}" style="background-color:var(--uui-color-divider)"></uui-progress-bar>
<p>If you find any bugs, or feel something is amiss, then please raise an issue on <a href="https://github.com/leekelleher/umbraco-contentment/issues" target="_blank" rel="noopener">the Contentment source-code repository on GitHub</a>.</p>
<p>Please do keep in mind that I am a solo developer on this project, working on it in my own free time.</p>`,
			},
		},
	]);

	#emojis: { [key: string]: string } = {
		':no_entry_sign:': '🚫',
		':grey_question:': '❔',
		':thinking:': '🤔',
		':octocat:': '🚥',
		':green_circle:': '🟢',
		':large_blue_circle:': '🔵',
		':red_circle:': '🔴',
		':shrug:': '🤷',
		':warning:': '⚠️',
	};

	#gfm = `
### Property editors

| :octocat:       | Editor          | Status      | Comment |
| --------------- | --------------- | ----------- | ------- |
| :green_circle: | **Bytes** | **Done** | |
| :green_circle: | **Code Editor** | **Done** | |
| :red_circle: | Content Blocks | _Undecided_ | :no_entry_sign: Considering deprecating; potentially migrate to Block List? :thinking: |
| :green_circle: | **Data List** | **Done** | Property-editor work is done.<br>**BUT!** A few of the data-sources and list-editors are still under active development, _(see below for status)_. |
| :large_blue_circle: | Data Picker | _Started_ | 1% done; A read-only placeholder editor is available. |
| :green_circle: | **Editor Notes** | **Done** | |
| :green_circle: | **Icon Picker** | **Done** | |
| :green_circle: | **List Items** | **Done** | |
| :green_circle: | **Notes** | **Done** | |
| :green_circle: | **Number Input** | **Done** | |
| :green_circle: | ~Render Macro~ | **Done** | :no_entry_sign: Macros have been deprecated in Umbraco.<br>:warning: Replaced the editor with a deprecation notice. |
| :green_circle: | **Social Links** | **Done** | |
| :thinking: | Templated Label | _Researching_ | 1% done; A read-only placeholder editor is available;<br>I'm currently researching the templating possibilities. |
| :green_circle: | **Textbox List** | **Done** | |
| :green_circle: | **Text Input** | **Done** | :warning: Unfortunately \`uui-input\` doesn't support \`datalist\` yet. |

### Internal components

Status of components used internally within Contentment.

| :octocat:       | Editor          | Status      | Comment |
| --------------- | --------------- | ----------- | ------- |
| :green_circle:  | **Cascading Dropdown List** | **Done** | |
| :green_circle:  | **Configuration Editor** | **Done** | |
| :green_circle:  | **Content Picker** | **Done** | |
| :large_blue_circle: | Data Table | _Started_ | 1% done; A read-only placeholder editor is available. |
| :green_circle:  | **Dictionary Picker** | **Done** | |
| :red_circle:    | ~Macro Picker~ | _Deprecated_ | :no_entry_sign: Macros have been removed in Umbraco 14. |
| :red_circle:    | ~Read Only~ | _Deprecated_ | :no_entry_sign: Not required, as was only used in the Content Block configuration. |
| :red_circle:    | ~Rich Text Editor~ | _Deprecated_ | :no_entry_sign: Not required, reuses Umbraco's RTE component. |

### Data List editors

Status of list-editors used by the Data List editor.

| :octocat:       | Editor          | Status      | Comment |
| --------------- | --------------- | ----------- | ------- |
| :green_circle: | **Buttons** | **Done** | |
| :green_circle: | **Checkbox List** | **Done** |  |
| :green_circle: | **Dropdown List** | **Done** | :warning: Missing "HTML attributes", Data Table UI.  |
| :large_blue_circle: | Item Picker | _Started_ | 37% done. |
| :green_circle: | **Radio Button List** | **Done** |  |
| :large_blue_circle: | Tags | _Started_ | 42% done. |
| :thinking: | Templated List | _Researching_ | :thinking: Exploring alternative options.<br>:warning: Missing "HTML attributes", Data Table UI. |

### Data List sources

Status of data-sources used by the Data List editor.
The majority of this work is reliant on the internal **Configuration Editor** UI.

| :octocat:       | Editor          | Status      | Comment |
| --------------- | --------------- | ----------- | ------- |
| :green_circle: | **.NET Countries** | **Done** | |
| :green_circle: | **.NET Currencies** | **Done** | |
| :green_circle: | **.NET Enum** | **Done** | |
| :green_circle: | **.NET Languages** | **Done** | |
| :green_circle: | **.NET Time Zone** | **Done** | |
| :green_circle: | **Examine** | **Done** | |
| :green_circle: | **JSON** | **Done** | |
| :green_circle: | **Number Range** | **Done** | |
| :green_circle: | **Physical File System** | **Done** | |
| :green_circle: | **SQL** | **Done** | |
| :green_circle: | **Text Delimited** | **Done** | |
| :green_circle: | **uCssClassName** | **Done** | |
| :thinking: | Umbraco Backoffice Sections | _Researching_ | :no_entry_sign: Backoffice sections are now registered on the client-side, unable to query from the server. I'd like to explore a way to have client-side data-sources. |
| :green_circle: | **Umbraco Content** | **Done** | |
| :green_circle: | **Umbraco Content Properties** | **Done** | |
| :green_circle: | **Umbraco Content Property Value** | **Done**  | |
| :green_circle: | **Umbraco Content Types** | **Done** | |
| :no_entry_sign: | ~Umbraco Content XPath~ | _Deprecated_ | :no_entry_sign: XPath has been removed in Umbraco 14. |
| :green_circle: | **Umbraco Dictionary** | **Done** | |
| :green_circle: | **Umbraco Entity** | **Done** | |
| :green_circle: | **Umbraco Files** | **Done** | |
| :green_circle: | **Umbraco Image Crop** | **Done** | |
| :green_circle: | **Umbraco Languages** | **Done** |  |
| :green_circle: | **Umbraco Member Group** | **Done** | |
| :green_circle: | **Umbraco Members** | **Done** | |
| :green_circle: | **Umbraco Tags** | **Done** | |
| :green_circle: | **Umbraco Templates** | **Done** | |
| :green_circle: | **Umbraco User Group** | **Done** | |
| :green_circle: | **Umbraco Users** | **Done** | |
| :green_circle: | **User Defined** | **Done** | |
| :green_circle: | **XML** | **Done** | |
| :green_circle: | **XML Sitemap Change Frequency** | **Done** | |
| :green_circle: | **XML Sitemap Priority** | **Done** | |
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
							.config=${this.#notesConfig}></contentment-property-editor-ui-editor-notes>
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
		[ELEMENT_NAME]: UmbBellissimaStatusDashboardElement;
	}
}
