// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

import type {
	ManifestPropertyEditorSchema,
	ManifestPropertyEditorUi,
} from '@umbraco-cms/backoffice/extension-registry';

const schema: ManifestPropertyEditorSchema = {
	type: 'propertyEditorSchema',
	name: '[Contentment] Social Links Property Editor Schema',
	alias: 'Umbraco.Community.Contentment.SocialLinks',
	meta: {
		defaultPropertyEditorUiAlias: 'Umb.Contentment.PropertyEditorUi.SocialLinks',
	},
};

const editorUi: ManifestPropertyEditorUi = {
	type: 'propertyEditorUi',
	alias: 'Umb.Contentment.PropertyEditorUi.SocialLinks',
	name: '[Contentment] Social Links Property Editor UI',
	element: () => import('../read-only/read-only.element.js'),
	meta: {
		label: '[Contentment] Social Links',
		icon: 'icon-hearts',
		group: 'lists',
		propertyEditorSchemaAlias: 'Umbraco.Community.Contentment.SocialLinks',
		settings: {
			properties: [
				{
					alias: 'networks',
					label: 'Social networks',
					description: 'Define the icon set for the available social networks.',
					propertyEditorUiAlias: 'Umb.Contentment.PropertyEditorUi.ConfigurationEditor',
					config: [
						{ alias: 'addButtonLabelKey', value: 'contentment_configureDataSource' },
						{
							alias: 'items',
							value: [
								{
									key: 'network',
									name: 'Social network',
									icon: 'icon-document',
									defaultValues: {
										icon: 'icon-document',
									},
									expressions: {
										name: '{{ name }}',
										description: '{{ url }}',
										icon: '{{ icon.split(" ")[0] }}',
										cardStyle: '{ "background-color": "{{ backgroundColor }}" }',
										iconStyle: '{ "color": "{{ iconColor }}" }',
									},
									fields: [
										{
											key: 'network',
											name: 'Network',
											description: 'An alias for the social network. This will be used as the value of the selection.',
											propertyEditorUiAlias: 'Umb.PropertyEditorUi.TextBox',
										},

										{
											key: 'name',
											name: 'Name',
											description: 'This will be used as the label of the social network in selection modal.',
											propertyEditorUiAlias: 'Umb.PropertyEditorUi.TextBox',
										},

										{
											key: 'url',
											name: 'Base URL',
											description: "This will be the starting part of the social network's profile URL.",
											propertyEditorUiAlias: 'Umb.PropertyEditorUi.TextBox',
										},

										{
											key: 'icon',
											name: 'Icon',
											description: 'Typically select the logo for the social network.',
											propertyEditorUiAlias: 'Umb.Contentment.PropertyEditorUi.IconPicker',
											config: {
												hideColors: true,
												size: 'small',
											},
										},

										{
											key: 'backgroundColor',
											name: 'Background color',
											description: 'The background color for the icon.',
											propertyEditorUiAlias: 'Umb.PropertyEditorUi.EyeDropper',
										},

										{
											key: 'iconColor',
											name: 'Icon color',
											description: 'The foreground color of the icon.',
											propertyEditorUiAlias: 'Umb.PropertyEditorUi.EyeDropper',
										},
									],
									overlaySize: 'medium',
								},
							],
						},
						{ alias: 'view', value: 'cards' },
					],
				},
				{
					alias: 'confirmRemoval',
					label: 'Confirm removals?',
					description: 'Select to enable a confirmation prompt when removing an item.',
					propertyEditorUiAlias: 'Umb.PropertyEditorUi.Toggle',
				},
				{
					alias: 'maxItems',
					label: 'Maximum items',
					description: "Enter the number for the maximum items allowed.<br>Use '0' for an unlimited amount.",
					propertyEditorUiAlias: 'Umb.Contentment.PropertyEditorUi.NumberInput',
				},
				{
					alias: 'enableDevMode',
					label: 'Developer mode?',
					description: 'Enable a property action to edit the raw data for the editor value.',
					propertyEditorUiAlias: 'Umb.PropertyEditorUi.Toggle',
				},
			],
			defaultData: [
				{
					alias: 'networks',
					value: [
						{
							key: 'network',
							value: {
								network: 'facebook',
								name: 'Facebook',
								url: 'https://facebook.com/',
								icon: 'icon-facebook',
								backgroundColor: '#3b5998',
								iconColor: '#fff',
							},
						},
						{
							key: 'network',
							value: {
								network: 'x-twitter',
								name: 'X (formerly Twitter)',
								url: 'https://twitter.com/',
								icon: 'icon-x-twitter',
								backgroundColor: '#000',
								iconColor: '#fff',
							},
						},
						{
							key: 'network',
							value: {
								network: 'instagram',
								name: 'Instagram',
								url: 'https://instagram.com/',
								icon: 'icon-instagram',
								backgroundColor: '#305777',
								iconColor: '#fff',
							},
						},
						{
							key: 'network',
							value: {
								network: 'linkedin',
								name: 'LinkedIn',
								url: 'https://linkedin.com/in/',
								icon: 'icon-linkedin',
								backgroundColor: '#007bb6',
								iconColor: '#fff',
							},
						},
						{
							key: 'network',
							value: {
								network: 'mastodon',
								name: 'Mastodon',
								url: 'https://mastodon.social/',
								icon: 'icon-mastodon',
								backgroundColor: '#5b4be1',
								iconColor: '#fff',
							},
						},
						{
							key: 'network',
							value: {
								network: 'youtube',
								name: 'YouTube',
								url: 'https://youtube.com/',
								icon: 'icon-youtube',
								backgroundColor: '#f00',
								iconColor: '#fff',
							},
						},
						{
							key: 'network',
							value: {
								network: 'github',
								name: 'GitHub',
								url: 'https://github.com/',
								icon: 'icon-github',
								backgroundColor: '#000',
								iconColor: '#fff',
							},
						},
						{
							key: 'network',
							value: {
								network: 'discord',
								name: 'Discord',
								url: 'https://discord.com/users/',
								icon: 'icon-discord',
								backgroundColor: '#404eed',
								iconColor: '#fff',
							},
						},
					],
				},
			],
		},
	},
};

export const manifests = [schema, editorUi];
