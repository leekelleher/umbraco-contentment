// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

import { UmbActionBase } from '@umbraco-cms/backoffice/action';
import { UmbPropertyValueChangeEvent } from '@umbraco-cms/backoffice/property-editor';
import { UMB_CODE_EDITOR_MODAL, UMB_MODAL_MANAGER_CONTEXT } from '@umbraco-cms/backoffice/modal';
import { UMB_PROPERTY_CONTEXT } from '@umbraco-cms/backoffice/property';
import type { UmbPropertyAction } from '@umbraco-cms/backoffice/property-action';

export class ContentmentPropertyActionEditJsonElement<ArgsMetaType = never>
	extends UmbActionBase<ArgsMetaType>
	implements UmbPropertyAction<ArgsMetaType>
{
	args: any;

	async getHref(): Promise<string | undefined> {
		return Promise.resolve(undefined);
	}

	async execute(): Promise<void> {
		const propertyContext = await this.getContext(UMB_PROPERTY_CONTEXT);

		const value = propertyContext?.getValue();

		if (!value) return;

		const modalManager = await this.getContext(UMB_MODAL_MANAGER_CONTEXT);

		const modal = modalManager.open(this, UMB_CODE_EDITOR_MODAL, {
			data: {
				headline: this.args?.meta?.label ?? 'Edit raw value',
				content: JSON.stringify(value, null, 2),
				language: 'json',
				confirmLabel: '#bulk_done',
			},
		});

		if (!modal) return;

		const { content } = await modal.onSubmit();

		propertyContext?.setValue(JSON.parse(content));

		this.dispatchEvent(new UmbPropertyValueChangeEvent());

		return Promise.resolve();
	}
}

export { ContentmentPropertyActionEditJsonElement as api };
