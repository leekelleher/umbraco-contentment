// SPDX-License-Identifier: MIT
// Copyright © 2026 Lee Kelleher

import { UmbModalBaseElement } from '@umbraco-cms/backoffice/modal';
import { UmbContextConsumer } from '@umbraco-cms/backoffice/context-api';
import { UMB_ROUTE_CONTEXT } from '@umbraco-cms/backoffice/router';

/**
 * Abstract base class for Contentment modals that need to host routable property-editors
 * (e.g. the Element Picker, which uses UmbModalRouteRegistrationController and requires
 * UMB_ROUTE_CONTEXT in its subtree).
 *
 * When a modal is opened imperatively via modalManager.open(), core's umb-modal installs
 * a UmbContextBoundary that blocks UMB_ROUTE_CONTEXT from reaching the modal's contents.
 * We bypass it by consuming UMB_ROUTE_CONTEXT from the opener's host element (which IS
 * in the full app routing chain) and re-providing it at our element level. Routable
 * property-editors then register their picker routes on the correct router context.
 *
 * Side-effect: navigationsuccess events from picker modal open/close cause
 * UmbModalManagerContext.#closeNoneRoutableModals() to call forceResolve() on all modals
 * with router === null — including ours. We neutralise this by patching forceResolve() on
 * our modal context instance to be a no-op unless the #allowForceResolve flag is set.
 * The flag is set only during _submitModal() so the legitimate call from submit() goes
 * through. reject() (cancel, Escape, X) settles the promise directly without forceResolve,
 * so it is unaffected.
 *
 * Subclasses extend this in place of UmbModalBaseElement and implement their own render().
 */
export abstract class ContentmentRoutableModalElement<
	D extends object = object,
	V = unknown,
> extends UmbModalBaseElement<D, V> {
	#allowForceResolve = false;
	#routeContextConsumer?: { destroy(): void };

	override connectedCallback() {
		super.connectedCallback();

		if (!this.modalContext) return;

		// Guard forceResolve() against spurious calls from UmbModalManagerContext.
		const original = this.modalContext.forceResolve.bind(this.modalContext);
		// eslint-disable-next-line @typescript-eslint/no-explicit-any
		(this.modalContext as any).forceResolve = () => {
			if (this.#allowForceResolve) original();
		};

		// Relay UMB_ROUTE_CONTEXT from the opener's host element into our subtree,
		// bypassing the UmbContextBoundary that umb-modal installs for imperative modals.
		const hostElement = this.modalContext.getHostElement();
		if (hostElement) {
			let provided = false;
			const consumer = new UmbContextConsumer(hostElement, UMB_ROUTE_CONTEXT, (ctx) => {
				if (ctx && !provided) {
					provided = true;
					this.provideContext(UMB_ROUTE_CONTEXT, ctx);
				}
			});
			this.#routeContextConsumer = consumer;
			consumer.hostConnected();
		}
	}

	override disconnectedCallback() {
		super.disconnectedCallback();
		this.#routeContextConsumer?.destroy();
		this.#routeContextConsumer = undefined;
	}

	protected override _submitModal(): void {
		this.#allowForceResolve = true;
		super._submitModal();
		this.#allowForceResolve = false;
	}
}
