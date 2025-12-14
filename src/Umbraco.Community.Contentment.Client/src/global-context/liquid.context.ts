// SPDX-License-Identifier: MPL-2.0
// Copyright © 2025 Lee Kelleher

import { UmbContextBase } from '@umbraco-cms/backoffice/class-api';
import { Liquid } from '../external/liquidjs/index.js';
import { CONTENTMENT_LIQUID_CONTEXT } from './liquid.context-token.js';
import type { UmbControllerHost } from '@umbraco-cms/backoffice/controller-api';
import type { Template } from '../external/liquidjs/index.js';

export class ContentmentLiquidContext extends UmbContextBase {
	#engine = new Liquid({ cache: true });

	constructor(host: UmbControllerHost) {
		super(host, CONTENTMENT_LIQUID_CONTEXT);
	}

	parse(template: string): Array<Template> {
		return this.#engine.parse(template);
	}

	async render(templates: Array<Template>, scope: object): Promise<string> {
		return this.#engine.render(templates, scope);
	}
}

export default ContentmentLiquidContext;
