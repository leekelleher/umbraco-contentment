// SPDX-License-Identifier: MIT
// Copyright © 2025 Lee Kelleher

import { appendToFrozenArray } from '@umbraco-cms/backoffice/observable-api';
import { ContentmentElementPropertyDatasetContext } from './element-property-dataset.context.js';
import { UmbContentTypeStructureManager } from '@umbraco-cms/backoffice/content-type';
import { UmbControllerBase } from '@umbraco-cms/backoffice/class-api';
import { UmbDataTypeItemRepositoryManager } from '@umbraco-cms/backoffice/data-type';
import { UmbDocumentTypeDetailRepository } from '@umbraco-cms/backoffice/document-type';
import { UmbElementWorkspaceDataManager, UMB_CONTENT_WORKSPACE_CONTEXT } from '@umbraco-cms/backoffice/content';
import { UmbReadOnlyVariantGuardManager } from '@umbraco-cms/backoffice/utils';
import { UmbValidationController } from '@umbraco-cms/backoffice/validation';
import { UmbVariantId } from '@umbraco-cms/backoffice/variant';
import { UmbVariantPropertyGuardManager } from '@umbraco-cms/backoffice/property';
import type { ContentmentElementDataModel } from './types.js';
import type { Observable } from '@umbraco-cms/backoffice/external/rxjs';
import type { UmbClassInterface } from '@umbraco-cms/backoffice/class-api';
import type { UmbContentTypeModel } from '@umbraco-cms/backoffice/content-type';
import type { UmbControllerHost } from '@umbraco-cms/backoffice/controller-api';
import type {
	UmbElementPropertyDataOwner,
	UmbElementValueModel,
	UmbContentWorkspaceContext,
} from '@umbraco-cms/backoffice/content';

export class ContentmentElementManager
	extends UmbControllerBase
	implements UmbElementPropertyDataOwner<ContentmentElementDataModel, UmbContentTypeModel>
{
	// Satisfies IS_CONTENT_WORKSPACE_CONTEXT guard on UMB_CONTENT_WORKSPACE_CONTEXT token
	readonly IS_CONTENT_WORKSPACE_CONTEXT = true as const;

	readonly #data = new UmbElementWorkspaceDataManager<ContentmentElementDataModel>(this);

	#loadedResolve!: () => void;
	readonly #loaded = new Promise<void>((resolve) => {
		this.#loadedResolve = resolve;
	});

	readonly structure = new UmbContentTypeStructureManager<UmbContentTypeModel>(
		this,
		new UmbDocumentTypeDetailRepository(this),
	);

	readonly #dataTypeItemManager = new UmbDataTypeItemRepositoryManager(this);

	readonly validation = new UmbValidationController(this);

	readonly propertyViewGuard = new UmbVariantPropertyGuardManager(this);
	readonly propertyWriteGuard = new UmbVariantPropertyGuardManager(this);
	readonly readOnlyGuard = new UmbReadOnlyVariantGuardManager(this);

	// UmbElementPropertyDataOwner required observables
	readonly unique = this.#data.createObservablePartOfCurrent((data) => data?.key);
	readonly values = this.#data.createObservablePartOfCurrent((data) => data?.values);

	constructor(host: UmbControllerHost) {
		super(host);
		this.propertyViewGuard.fallbackToPermitted();
		this.propertyWriteGuard.fallbackToPermitted();

		// Keep data-type item manager in sync with structure changes so setPropertyValue can resolve editorAlias
		this.observe(
			this.structure.contentTypeDataTypeUniques,
			(dataTypeUniques: Array<string>) => {
				this.#dataTypeItemManager.setUniques(dataTypeUniques);
			},
			null,
		);
	}

	async init(elementType: string, key: string, value: Record<string, unknown>, readonly = false): Promise<void> {
		// Invariant-only — no culture/segment variation
		this.#data.setVaries(false);
		this.#data.setVariesByCulture(false);
		this.#data.setVariesBySegment(false);

		const model: ContentmentElementDataModel = {
			key,
			values: Object.entries(value).map(
				([alias, v]) =>
					({
						alias,
						value: v,
						culture: null,
						segment: null,
						// editorAlias starts as '' — overwritten by setPropertyValue when editors write back
						editorAlias: '',
					}) as UmbElementValueModel,
			),
		};

		this.#data.setPersisted(model);
		this.#data.setCurrent(model);

		if (readonly) {
			this.propertyWriteGuard.addRule({ unique: 'readonly', permitted: false });
		}

		this.structure.loadType(elementType);
		try {
			await this.structure.whenLoaded();
		} finally {
			this.#loadedResolve();
		}
	}

	isLoaded(): Promise<void> {
		return this.#loaded;
	}

	getUnique(): string | undefined {
		return this.#data.getCurrent()?.key;
	}

	getEntityType(): string {
		return 'element';
	}

	getValues(): ContentmentElementDataModel['values'] | undefined {
		return this.#data.getCurrent()?.values;
	}

	async propertyValueByAlias<ReturnType = unknown>(
		propertyAlias: string,
		variantId?: UmbVariantId,
	): Promise<Observable<ReturnType | undefined> | undefined> {
		return this.#data.createObservablePartOfCurrent(
			(data) =>
				data?.values?.find((x) => x?.alias === propertyAlias && (variantId ? variantId.compare(x) : true))
					?.value as ReturnType,
		);
	}

	getPropertyValue<ReturnType = unknown>(alias: string, variantId?: UmbVariantId): ReturnType | undefined {
		const currentData = this.#data.getCurrent();
		if (currentData) {
			const entry = currentData.values?.find((x) => x.alias === alias && (variantId ? variantId.compare(x) : true));
			return entry?.value as ReturnType;
		}
		return undefined;
	}

	async setPropertyValue<ValueType = unknown>(
		alias: string,
		value: ValueType,
		variantId?: UmbVariantId,
	): Promise<void> {
		this.initiatePropertyValueChange();
		try {
			variantId ??= UmbVariantId.CreateInvariant();

			const property = await this.structure.getPropertyStructureByAlias(alias);
			if (!property) {
				throw new Error(`Property alias "${alias}" not found.`);
			}

			const dataTypeItem = await this.#dataTypeItemManager.getItemByUnique(property.dataType.unique);
			const editorAlias = dataTypeItem?.propertyEditorSchemaAlias;

			if (!editorAlias) {
				throw new Error(`Editor Alias of "${property.dataType.unique}" not found.`);
			}

			const entry = { editorAlias, ...variantId.toObject(), alias, value } as UmbElementValueModel<ValueType>;

			const currentData = this.#data.getCurrent();
			if (currentData) {
				const values = appendToFrozenArray(
					currentData.values ?? [],
					entry,
					(x) => x.alias === alias && variantId!.compare(x),
				);
				this.#data.updateCurrent({ values });
			}
		} finally {
			this.finishPropertyValueChange();
		}
	}

	initiatePropertyValueChange(): void {
		this.#data.initiatePropertyValueChange();
	}

	finishPropertyValueChange = (): void => {
		this.#data.finishPropertyValueChange();
	};

	// Satisfies 'propertyStructureById' in context guard on UMB_PROPERTY_STRUCTURE_WORKSPACE_CONTEXT
	propertyStructureById(id: string) {
		return this.structure.propertyStructureById(id);
	}

	getHasUnpersistedChanges(): boolean {
		return this.#data.getHasUnpersistedChanges();
	}

	getResult(): Record<string, unknown> {
		const values = this.getValues() ?? [];
		return Object.fromEntries(values.map((v) => [v.alias, v.value]));
	}

	setup(host: UmbClassInterface): void {
		// 1. Dataset context provides UMB_PROPERTY_DATASET_CONTEXT + UMB_VARIANT_CONTEXT (invariant)
		new ContentmentElementPropertyDatasetContext(host, this, UmbVariantId.CreateInvariant());

		// 2. Validation context: property-level required/regex messages surface automatically
		this.validation.provideAt(host);

		// 3. Faux workspace context — one provide satisfies both:
		//    UMB_CONTENT_WORKSPACE_CONTEXT  (guard: IS_CONTENT_WORKSPACE_CONTEXT === true)
		//    UMB_PROPERTY_STRUCTURE_WORKSPACE_CONTEXT  (guard: 'propertyStructureById' in context)
		//    Both tokens share alias 'UmbWorkspaceContext' — providing once resolves both.
		host.provideContext(UMB_CONTENT_WORKSPACE_CONTEXT, this as unknown as UmbContentWorkspaceContext);
	}

	override destroy(): void {
		this.structure.destroy();
		super.destroy();
	}
}
