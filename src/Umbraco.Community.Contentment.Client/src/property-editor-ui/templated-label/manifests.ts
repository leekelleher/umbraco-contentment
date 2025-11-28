// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

const schema: UmbExtensionManifest = {
	type: 'propertyEditorSchema',
	name: '[Contentment] Templated Label Property Editor Schema',
	alias: 'Umbraco.Community.Contentment.TemplatedLabel',
	meta: {
		defaultPropertyEditorUiAlias: 'Umb.Contentment.PropertyEditorUi.TemplatedLabel',
		settings: {
			properties: [
				{
					alias: 'umbracoDataValueType',
					label: 'Value type',
					description: "Select the value's type. This defines how the underlying value is stored in the database.",
					propertyEditorUiAlias: 'Umb.Contentment.PropertyEditorUi.DropdownList',
					config: [
						{
							alias: 'items',
							value: [
								{ name: 'Big Integer', value: 'BIGINT' },
								{ name: 'Date', value: 'DATE' },
								{ name: 'Date/Time', value: 'DATETIME' },
								{ name: 'Decimal', value: 'DECIMAL' },
								{ name: 'Integer', value: 'INT' },
								{ name: 'JSON', value: 'JSON' },
								{ name: 'String', value: 'STRING' },
								{ name: 'Text', value: 'TEXT' },
								{ name: 'Time', value: 'TIME' },
								{ name: 'XML', value: 'XML' },
							],
						},
					],
				},
			],
			defaultData: [{ alias: 'umbracoDataValueType', value: 'STRING' }],
		},
	},
};

const editorUi: UmbExtensionManifest = {
	type: 'propertyEditorUi',
	alias: 'Umb.Contentment.PropertyEditorUi.TemplatedLabel',
	name: '[Contentment] Templated Label Property Editor UI',
	element: () => import('./templated-label.element.js'),
	meta: {
		label: 'Templated Label',
		icon: 'icon-fa-codepen',
		group: 'contentment',
		propertyEditorSchemaAlias: 'Umbraco.Community.Contentment.TemplatedLabel',
		settings: {
			properties: [
				{
					alias: '_notes',
					label: '',
					propertyEditorUiAlias: 'Umb.Contentment.PropertyEditorUi.Notes',
					config: [
						{
							alias: 'notes',
							value: `<details class="well">
<summary><strong>Do you need help with your custom template?</strong></summary>
<p>Your custom template will be used to display the label on the property from the underlying value.</p>
<p>If you are familiar with Liquid template syntax, you can display the value using an expression: e.g. <code>{{ model.value }}</code>.</p>
<p>If you need assistance with Liquid expression syntax, please refer to this resource: <a href="https://liquidjs.com/" target="_blank"><strong>liquidjs.com</strong></a>.</p>
<hr>
<p>If you would like a starting point for your custom template, here is an example.</p>
<umb-code-block language="Liquid template" copy>&lt;details&gt;
    &lt;summary&gt;View data&lt;/summary&gt;
    &lt;umb-code-block language="JSON" copy&gt;{{ model.value | json }}&lt;/umb-code-block&gt;
&lt;/details&gt;</umb-code-block>
</details>`,
						},
						{ alias: 'hideLabel', value: true },
					],
				},

				{
					alias: 'notes',
					label: 'Template',
					description: 'Enter the Liquid template to be displayed for the label.',
					propertyEditorUiAlias: 'Umb.Contentment.PropertyEditorUi.CodeEditor',
					config: [{ alias: 'mode', value: 'html' }],
				},
				{
					alias: 'hideLabel',
					label: 'Hide label?',
					description: `<uui-tag look="placeholder">experimental</uui-tag>
Select to hide the label and have the editor take up the full width of the panel.`,
					propertyEditorUiAlias: 'Umb.PropertyEditorUi.Toggle',
				},
				{
					alias: 'hidePropertyGroup',
					label: 'Move above property group container?',
					description: `<uui-tag look="placeholder">experimental</uui-tag>
Select to move the note above/outside the property group.`,
					propertyEditorUiAlias: 'Umb.PropertyEditorUi.Toggle',
				},
				{
					alias: 'enableDevMode',
					label: 'Developer mode?',
					description: 'Enable a property action to edit the raw data for the editor value.',
					propertyEditorUiAlias: 'Umb.PropertyEditorUi.Toggle',
				},
			],
			defaultData: [{ alias: 'component', value: ['Umb.Contentment.TemplatedLabelUi.CodeBlock'] }],
		},
	},
};

export const manifests = [schema, editorUi];
