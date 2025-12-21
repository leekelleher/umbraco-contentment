// SPDX-License-Identifier: MIT
// Copyright © 2025 Lee Kelleher

import { CONTENTMENT_LIQUID_CONTEXT } from './liquid.context-token.js';
import { Liquid } from '../../external/liquidjs/index.js';
import { UmbContextBase } from '@umbraco-cms/backoffice/class-api';
import type { Liquid, Template } from '../../external/liquidjs.js';
import type { UmbControllerHost } from '@umbraco-cms/backoffice/controller-api';

export class ContentmentLiquidContext extends UmbContextBase {
	#engine?: Liquid;

	public get engine(): Liquid {
		if (!this.#engine) {
			this.#engine = new Liquid({ cache: true });
		}
		return this.#engine;
	}

	constructor(host: UmbControllerHost) {
		super(host, CONTENTMENT_LIQUID_CONTEXT);
	}

	parse(template: string): Array<Template> {
		try {
			return this.engine.parse(template);
		} catch (error) {
			throw new Error(`Failed to parse Liquid template: ${(error instanceof Error) ? error.message : String(error)}`);
		}
	}

	async render(templates: Array<Template>, scope: object): Promise<string> {
		try {
			return await this.engine.render(templates, scope);
		} catch (error) {
			throw new Error(`Liquid template rendering failed: ${error instanceof Error ? error.message : String(error)}`);
		}
	}
}

export default ContentmentLiquidContext;

export { ContentmentLiquidContext as api };
