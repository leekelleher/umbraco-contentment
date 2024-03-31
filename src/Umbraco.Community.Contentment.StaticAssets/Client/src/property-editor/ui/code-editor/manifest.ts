/* Copyright © 2023 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

import type { ManifestPropertyEditorUi } from '@umbraco-cms/backoffice/extension-registry';

export const manifest: ManifestPropertyEditorUi = {
  type: 'propertyEditorUi',
  alias: 'Umb.Contentment.PropertyEditorUi.CodeEditor',
  name: '[Contentment] Code Editor',
  element: () => import('./code-editor.element.js'),
  meta: {
    label: '[Contentment] Code Editor',
    icon: 'icon-code',
    group: 'code',
    propertyEditorSchemaAlias: 'Umbraco.Plain.String',
    settings: {
      properties: [
        {
          alias: 'mode',
          label: 'Language mode',
          description: 'WIP: Use one of the following: `razor`, `javascript`.',
          propertyEditorUiAlias: 'Umb.PropertyEditorUi.Dropdown',
          config: [
            { alias: 'items', value: ['javascript', 'razor'] },
          ],
        },
      ],
      defaultData: [{
        alias: 'mode', value: 'razor'
      }]
    }
  },
};
