// SPDX-License-Identifier: MPL-2.0
// Copyright © 2023 Lee Kelleher

import type { ManifestLocalization } from '@umbraco-cms/backoffice/extension-registry';

export const manifests: Array<ManifestLocalization> = [
  {
    type: 'localization',
    name: '[Contentment] English',
    alias: 'Umb.Contentment.Localization.En',
    js: () => import('./en.js'),
    meta: {
      culture: 'en',
    },
  },
];
