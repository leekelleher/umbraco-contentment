// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

import type { ManifestPropertyEditorSchema } from '@umbraco-cms/backoffice/extension-registry';

export const manifest: ManifestPropertyEditorSchema = {
  type: 'propertyEditorSchema',
  name: '[Contentment] Data List',
  alias: 'Umbraco.Community.Contentment.DataList',
  meta: {
    defaultPropertyEditorUiAlias: 'Umb.Contentment.PropertyEditorUi.DataList',
    settings: {
      properties: [
        {
          alias: 'dataSource',
          label: 'Data source',
          description: 'Select and configure a data source.',
          propertyEditorUiAlias: 'Umb.Contentment.PropertyEditorUi.ConfigurationEditor',
          config: [
            { alias: 'addButtonLabelKey', value: 'contentment_configureDataSource' },
          ],
        },
        {
          alias: 'listEditor',
          label: 'List editor',
          description: 'Select and configure a list editor.',
          propertyEditorUiAlias: 'Umb.Contentment.PropertyEditorUi.ConfigurationEditor',
          config: [
            { alias: 'addButtonLabelKey', value: 'contentment_configureListEditor' },
          ],
        },
      ],
    }
  },
};
