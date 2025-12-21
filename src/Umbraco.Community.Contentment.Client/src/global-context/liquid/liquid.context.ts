// SPDX-License-Identifier: MIT
// Copyright © 2025 Lee Kelleher

import { UmbContextBase } from '@umbraco-cms/backoffice/class-api';
import type { Liquid, Template } from '../../external/liquidjs.js';
import type { UmbControllerHost } from '@umbraco-cms/backoffice/controller-api';
import { UmbContextToken } from '@umbraco-cms/backoffice/context-api';

export class ContentmentLiquidContext extends UmbContextBase {
	#engine?: Liquid;

	async #getEngine(): Promise<Liquid> {
		if (!this.#engine) {
			const { Liquid } = await import('../../external/liquidjs.js');
			this.#engine = new Liquid({ cache: true });
		}

		return this.#engine;
	}

	public get engine(): Promise<Liquid> {
		return this.#getEngine();
	}

	constructor(host: UmbControllerHost) {
		super(host, CONTENTMENT_LIQUID_CONTEXT);
	}

	async parse(template: string): Promise<Array<Template>> {
		const engine = await this.#getEngine();
		try {
			return engine.parse(template);
		} catch (error) {
			throw new Error(`Failed to parse Liquid template: ${error instanceof Error ? error.message : String(error)}`);
		}
	}

	async render(templates: Array<Template>, scope: object): Promise<string> {
		const engine = await this.#getEngine();
		try {
			return await engine.render(templates, scope);
		} catch (error) {
			throw new Error(`Liquid template rendering failed: ${error instanceof Error ? error.message : String(error)}`);
		}
	}
}

export default ContentmentLiquidContext;

export { ContentmentLiquidContext as api };

export const CONTENTMENT_LIQUID_CONTEXT = new UmbContextToken<ContentmentLiquidContext>('ContentmentLiquidContext');
