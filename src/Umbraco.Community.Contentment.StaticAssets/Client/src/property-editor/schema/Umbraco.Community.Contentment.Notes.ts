/* Copyright © 2024 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

import type { ManifestPropertyEditorSchema } from '@umbraco-cms/backoffice/extension-registry';

export const manifest: ManifestPropertyEditorSchema = {
  type: 'propertyEditorSchema',
  name: '[Contentment] Notes',
  alias: 'Umbraco.Community.Contentment.Notes',
  meta: {
    defaultPropertyEditorUiAlias: 'Umb.Contentment.PropertyEditorUi.Notes',
  },
};
