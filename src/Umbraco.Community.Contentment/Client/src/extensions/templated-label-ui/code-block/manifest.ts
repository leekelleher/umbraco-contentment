// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

import { ContentmentTemplatedLabelUiExtentionManifestType } from '../templated-label-ui.extension.js';

export const manifest: ContentmentTemplatedLabelUiExtentionManifestType = {
	type: 'contentmentTemplatedLabelUi',
	alias: 'Umb.Contentment.TemplatedLabelUi.CodeBlock',
	name: '[Contentment] Code Block Templated Label UI',
	element: () => import('./code-block.element.js'),
  meta: {
    name: 'Code Block',
    icon: 'icon-code',
  },
};
