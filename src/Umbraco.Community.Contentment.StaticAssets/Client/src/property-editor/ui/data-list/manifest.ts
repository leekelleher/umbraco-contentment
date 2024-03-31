/* Copyright © 2024 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

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
