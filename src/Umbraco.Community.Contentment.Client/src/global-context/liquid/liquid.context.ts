// SPDX-License-Identifier: MIT
// Copyright © 2025 Lee Kelleher

import { CONTENTMENT_LIQUID_CONTEXT } from './liquid.context-token.js';
import { Liquid } from '../../external/liquidjs/index.js';
import { UmbContextBase } from '@umbraco-cms/backoffice/class-api';
import type { Template } from '../../external/liquidjs/index.js';
import type { UmbControllerHost } from '@umbraco-cms/backoffice/controller-api';

export class ContentmentLiquidContext extends UmbContextBase {

	constructor(host: UmbControllerHost) {
		super(host, CONTENTMENT_LIQUID_CONTEXT);
	}

	parse(template: string): Array<Template> {
	}

	async render(templates: Array<Template>, scope: object): Promise<string> {
	}
}

export default ContentmentLiquidContext;
