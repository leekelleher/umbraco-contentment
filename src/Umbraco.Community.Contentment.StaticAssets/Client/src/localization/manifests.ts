/* Copyright © 2023 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

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
