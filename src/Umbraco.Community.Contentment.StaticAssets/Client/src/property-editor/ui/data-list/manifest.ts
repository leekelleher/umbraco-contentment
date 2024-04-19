// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

import type { ManifestPropertyEditorUi } from '@umbraco-cms/backoffice/extension-registry';

export const manifest: ManifestPropertyEditorUi = {
  type: 'propertyEditorUi',
  alias: 'Umb.Contentment.PropertyEditorUi.DataList',
  name: '[Contentment] Data List',
  element: () => import('./data-list.element.js'),
  meta: {
    label: '[Contentment] Data List',
    icon: 'icon-list',
    group: 'lists',
    propertyEditorSchemaAlias: 'Umbraco.Community.Contentment.DataList',
  },
};
