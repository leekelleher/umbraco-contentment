// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

import { UmbPropertyActionBase } from '@umbraco-cms/backoffice/property-action';
import { UMB_CODE_EDITOR_MODAL } from '@umbraco-cms/backoffice/code-editor';
import { UMB_MODAL_MANAGER_CONTEXT } from '@umbraco-cms/backoffice/modal';
import { UMB_PROPERTY_CONTEXT } from '@umbraco-cms/backoffice/property';
import type { MetaPropertyActionDefaultKind } from '@umbraco-cms/backoffice/property-action';

export class ContentmentPropertyActionEditJsonElement extends UmbPropertyActionBase<MetaPropertyActionDefaultKind> {
	override async execute() {
		const propertyContext = await this.getContext(UMB_PROPERTY_CONTEXT);
		if (!propertyContext) return;

		const value = propertyContext.getValue();

		const modalManager = await this.getContext(UMB_MODAL_MANAGER_CONTEXT);
		if (!modalManager) return;
		const modal = modalManager.open(this, UMB_CODE_EDITOR_MODAL, {
			data: {
				headline: this.args.meta.label ?? 'Edit raw value',
				content: JSON.stringify(value, null, 2),
				language: 'json',
				confirmLabel: '#bulk_done',
			},
		});

		const data = await modal.onSubmit().catch(() => undefined);
		if (!data) return;

		const json = JSON.parse(data.content);
		propertyContext.setValue(json);
	}
}

export { ContentmentPropertyActionEditJsonElement as api };
