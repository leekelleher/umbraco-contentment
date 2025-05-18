// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

import { css, customElement, html, state, unsafeHTML, when } from '@umbraco-cms/backoffice/external/lit';
import { DOMPurify } from '@umbraco-cms/backoffice/external/dompurify';
import { Marked } from '@umbraco-cms/backoffice/external/marked';
import { UmbLitElement } from '@umbraco-cms/backoffice/lit-element';
import { UmbTextStyles } from '@umbraco-cms/backoffice/style';

const UmbMarked = new Marked({ gfm: true, breaks: true });

@customElement('contentment-workspace')
export class ContentmentWorkspaceElement extends UmbLitElement {
	#emojis: { [key: string]: string } = {
		':no_entry_sign:': '🚫',
		':thinking:': '🤔',
		':octocat:': '🚥',
		':green_circle:': '🟢',
		':large_blue_circle:': '🔵',
		':warning:': '⚠️',
	};

	#gfm = `
### Property editors

| :octocat:           | Editor              | Status        | Comment |
| ------------------- | ------------------- | ------------- | ------- |
| :green_circle:      | **Bytes**           | **Done**      | |
| :green_circle:      | **Code Editor**     | **Done**      | |
| :large_blue_circle: | Content Blocks      | _In progress_ | 37% done. |
| :green_circle:      | **Data List**       | **Done**      | _(see below for development status on individual components)_ |
| :green_circle:      | **Data Picker**     | **Done**      | |
| :green_circle:      | **Editor Notes**    | **Done**      | |
| :green_circle:      | **Icon Picker**     | **Done**      | |
| :green_circle:      | **List Items**      | **Done**      | |
| :green_circle:      | **Notes**           | **Done**      | |
| :green_circle:      | **Number Input**    | **Done**      | |
| :no_entry_sign:     | ~Render Macro~      | _Deprecated_  | :no_entry_sign: Macros have been deprecated in Umbraco.<br>:warning: Replaced the editor with a deprecation notice. |
| :green_circle:      | **Social Links**    | **Done**      | |
| :green_circle:      | **Templated Label** | **Done**      | |
| :green_circle:      | **Textbox List**    | **Done**      | |
| :green_circle:      | **Text Input**      | **Done**      | :warning: Unfortunately \`uui-input\` doesn't support \`datalist\` yet. |

### Internal components

Status of components used internally within Contentment.

| :octocat:       | Editor          | Status      | Comment |
| --------------- | --------------- | ----------- | ------- |
| :green_circle:  | **Cascading Dropdown List** | **Done** | |
| :green_circle:  | **Configuration Editor** | **Done** | |
| :green_circle:  | **Content Picker** | **Done** | |
| :no_entry_sign: | ~Data Table~ | _Removed_ | :no_entry_sign: No longer used internally. |
| :green_circle:  | **Dictionary Picker** | **Done** | |
| :no_entry_sign: | ~Macro Picker~ | _Removed_ | :no_entry_sign: Macros have been removed in Umbraco 14. |
| :no_entry_sign: | ~Read Only~ | _Removed_ | :no_entry_sign: Not required, as was only used in the Content Block configuration. |
| :no_entry_sign: | ~Rich Text Editor~ | _Removed_ | :no_entry_sign: Not required, reuses Umbraco's RTE component. |

### Data List editors

Status of list-editors used by the Data List editor.

| :octocat:           | Editor                | Status    | Comment |
| ------------------- | --------------------- | --------- | ------- |
| :green_circle:      | **Buttons**           | **Done**  | |
| :green_circle:      | **Checkbox List**     | **Done**  | |
| :green_circle:      | **Dropdown List**     | **Done**  | |
| :green_circle:      | **Item Picker**       | **Done**  | :warning: Done, but sorting and "List type" config hasn't been implemented yet. |
| :green_circle:      | **Radio Button List** | **Done**  | |
| :large_blue_circle: | Tags | _In progress_  | 42% done. | |
| :green_circle:      | **Templated List**    | **Done**  | |

### Data List sources

Status of data-sources used by the Data List editor.
The majority of this work is reliant on the internal **Configuration Editor** UI.

| :octocat:       | Editor          | Status      | Comment |
| --------------- | --------------- | ----------- | ------- |
| :green_circle:  | **.NET Countries** | **Done** | |
| :green_circle:  | **.NET Currencies** | **Done** | |
| :green_circle:  | **.NET Enum** | **Done** | |
| :green_circle:  | **.NET Languages** | **Done** | |
| :green_circle:  | **.NET Time Zone** | **Done** | |
| :green_circle:  | **Examine** | **Done** | |
| :green_circle:  | **JSON** | **Done** | |
| :green_circle:  | **Number Range** | **Done** | |
| :green_circle:  | **Physical File System** | **Done** | |
| :green_circle:  | **SQL** | **Done** | |
| :green_circle:  | **Text Delimited** | **Done** | |
| :green_circle:  | **uCssClassName** | **Done** | |
| :green_circle:  | **Umbraco Backoffice Sections** | **Done** | |
| :green_circle:  | **Umbraco Content** | **Done** | |
| :green_circle:  | **Umbraco Content Properties** | **Done** | |
| :green_circle:  | **Umbraco Content Property Value** | **Done**  | |
| :green_circle:  | **Umbraco Content Types** | **Done** | |
| :no_entry_sign: | ~Umbraco Content XPath~ | _Deprecated_ | :no_entry_sign: XPath has been removed in Umbraco 14. |
| :green_circle:  | **Umbraco Dictionary** | **Done** | |
| :green_circle:  | **Umbraco Entity** | **Done** | |
| :green_circle:  | **Umbraco Files** | **Done** | |
| :green_circle:  | **Umbraco Image Crop** | **Done** | |
| :green_circle:  | **Umbraco Languages** | **Done** |  |
| :green_circle:  | **Umbraco Member Group** | **Done** | |
| :green_circle:  | **Umbraco Members** | **Done** | |
| :green_circle:  | **Umbraco Tags** | **Done** | |
| :green_circle:  | **Umbraco Templates** | **Done** | |
| :green_circle:  | **Umbraco User Group** | **Done** | |
| :green_circle:  | **Umbraco Users** | **Done** | |
| :green_circle:  | **User Defined** | **Done** | |
| :green_circle:  | **XML** | **Done** | |
| :green_circle:  | **XML Sitemap Change Frequency** | **Done** | |
| :green_circle:  | **XML Sitemap Priority** | **Done** | |
`;

	@state()
	private _markup?: string;

	override firstUpdated() {
		const regex = new RegExp(Object.keys(this.#emojis).join('|'), 'gi');
		const markdown = this.#gfm.replace(regex, (matched) => this.#emojis[matched]);
		const markup = UmbMarked.parse(markdown) as string;
		this._markup = DOMPurify.sanitize(markup);
	}

	override render() {
		return html`
			<umb-body-layout headline="Migration status of Contentment for Umbraco 15">
				<div slot="action-menu"><uui-tag color="positive" look="placeholder">Beta testing</uui-tag></div>

				<contentment-info-box
					type="divider"
					icon="icon-contentment"
					heading="Status update for Contentment v6.0.0-beta001">
					<p>Contentment v6.0 is in beta. Your testing and feedback of the package is essential.</p>
					<p>
						If you find any bugs, or feel something is amiss, then please raise an issue on
						<a href="https://github.com/leekelleher/umbraco-contentment/issues" target="_blank" rel="noopener"
							>the Contentment source-code repository on GitHub</a
						>.
					</p>
					<p>Please do keep in mind that I am a solo developer on this project, working on it in my own free time.</p>
				</contentment-info-box>

				${when(
					this._markup,
					() => html`<uui-box><div class="gfm">${unsafeHTML(this._markup)}</div></uui-box>`,
					() => html`<uui-loader></uui-loader>`
				)}
			</umb-body-layout>
		`;
	}

	static override styles = [
		UmbTextStyles,
		css`
			div[slot='action-menu'] {
				margin-right: var(--uui-size-layout-3);
			}

			contentment-info-box {
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

export { ContentmentWorkspaceElement as element };

declare global {
	interface HTMLElementTagNameMap {
		'contentment-workspace': ContentmentWorkspaceElement;
	}
}
