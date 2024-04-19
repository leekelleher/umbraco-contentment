// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

import type { ManifestPropertyEditorSchema } from '@umbraco-cms/backoffice/extension-registry';

export const manifest: ManifestPropertyEditorSchema = {
  type: 'propertyEditorSchema',
  name: '[Contentment] Notes',
  alias: 'Umbraco.Community.Contentment.Notes',
  meta: {
    defaultPropertyEditorUiAlias: 'Umb.Contentment.PropertyEditorUi.Notes',
  },
};
