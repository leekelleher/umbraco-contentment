// SPDX-License-Identifier: MPL-2.0
// Copyright © 2023 Lee Kelleher

import type { ManifestLocalization } from '@umbraco-cms/backoffice/extension-registry';

export const manifests: Array<ManifestLocalization> = [
  {
    type: 'localization',
    name: '[Contentment] English (US)',
    alias: 'Umb.Contentment.Localization.En_US',
    js: () => import('./en-us.js'),
    meta: {
      culture: 'en-us',
    },
  },
];
