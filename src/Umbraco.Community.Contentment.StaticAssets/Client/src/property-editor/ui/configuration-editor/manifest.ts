// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

import type { ManifestPropertyEditorUi } from '@umbraco-cms/backoffice/extension-registry';

export const manifest: ManifestPropertyEditorUi = {
  type: 'propertyEditorUi',
  alias: 'Umb.Contentment.PropertyEditorUi.ConfigurationEditor',
  name: '[Contentment] Configuration Editor',
  element: () => import('./configuration-editor.element.js'),
  meta: {
    label: '[Contentment] Configuration Editor',
    icon: 'icon-settings-alt',
    group: 'data',
  }
};
