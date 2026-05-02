import { customElement, property, state } from '@umbraco-cms/backoffice/external/lit';
import { createExtensionElement, UmbExtensionsApiInitializer } from '@umbraco-cms/backoffice/extension-api';
import { umbExtensionsRegistry } from '@umbraco-cms/backoffice/extension-registry';
import { UmbLitElement } from '@umbraco-cms/backoffice/lit-element';
import { UmbPropertyContext } from '@umbraco-cms/backoffice/property';
import { UmbRoutePathAddendumContext } from '@umbraco-cms/backoffice/router';
import { UMB_MARK_ATTRIBUTE_NAME } from '@umbraco-cms/backoffice/const';
import type {
	ManifestPropertyEditorUi,
	UmbPropertyEditorConfigCollection,
	UmbPropertyEditorConfig,
} from '@umbraco-cms/backoffice/property-editor';
import type { UmbObserverController } from '@umbraco-cms/backoffice/observable-api';
import { UmbPropertyTypeBasedPropertyContext } from '@umbraco-cms/backoffice/content';

@customElement('contentment-input-list-property-editor')
export class ContentmentInputListPropertyEditorElement extends UmbLitElement {
	#configObserver?: UmbObserverController<UmbPropertyEditorConfigCollection | undefined>;
	#extensionsController?: UmbExtensionsApiInitializer<any>;
	#pathAddendum = new UmbRoutePathAddendumContext(this);
	#propertyContext = new UmbPropertyContext(this);
	#propertyTypeBasedPropertyContext = new UmbPropertyTypeBasedPropertyContext(this);
	#valueObserver?: UmbObserverController<unknown>;

	@property({ type: String })
	public set alias(alias: string) {
		this.setAttribute(UMB_MARK_ATTRIBUTE_NAME, 'property:' + alias);
		this.#propertyContext.setAlias(alias);
	}
	public get alias() {
		return this.#propertyContext.getAlias() ?? '';
	}

	@property({ type: String, attribute: 'property-editor-ui-alias' })
	public set propertyEditorUiAlias(value: string | undefined) {
		this._propertyEditorUiAlias = value;
		this._observePropertyEditorUI();
	}
	public get propertyEditorUiAlias(): string {
		return this._propertyEditorUiAlias ?? '';
	}
	private _propertyEditorUiAlias?: string;

	@property({ type: Array, attribute: false })
	public set config(value: UmbPropertyEditorConfig | undefined) {
		this.#propertyContext.setConfig(value);
	}
	public get config(): UmbPropertyEditorConfig | undefined {
		return this.#propertyContext.getConfig();
	}

	@state()
	private _element?: ManifestPropertyEditorUi['ELEMENT_TYPE'];

	@state()
	private _isReadOnly = false;

	constructor() {
		super();

		this.observe(
			this.#propertyContext.alias,
			(alias) => {
				if (!alias) return;
				this.#pathAddendum.setAddendum(alias);
				this.#propertyTypeBasedPropertyContext.setDataType({ unique: alias });
			},
			null
		);
	}

	private _onPropertyEditorChange = (e: CustomEvent): void => {
		const target = e.composedPath()[0] as any;
		if (this._element !== target) {
			console.error(
				"Property Editor received a Change Event who's target is not the Property Editor Element. Do not make bubble and composed change events."
			);
			return;
		}

		//this.value = target.value; // Sets value in context.
		this.#propertyContext.setValue(target.value);
		e.stopPropagation();
	};

	private _observePropertyEditorUI(): void {
		if (this._propertyEditorUiAlias) {
			this.observe(
				umbExtensionsRegistry.byTypeAndAlias('propertyEditorUi', this._propertyEditorUiAlias),
				(manifest) => {
					if (!manifest && this._propertyEditorUiAlias !== 'Umb.PropertyEditorUi.MissingUi') {
						this._propertyEditorUiAlias = 'Umb.PropertyEditorUi.MissingUi';
						this._observePropertyEditorUI();
						return;
					}
					this._gotEditorUI(manifest);
				},
				'_observePropertyEditorUI'
			);
		}
	}

	private async _gotEditorUI(manifest?: ManifestPropertyEditorUi | null): Promise<void> {
		this.#extensionsController?.destroy();
		this.#propertyContext.setEditor(undefined);
		this.#propertyContext.setEditorManifest(manifest ?? undefined);

		if (!manifest) {
			return;
		}

		const el = await createExtensionElement(manifest);

		if (el) {
			const oldElement = this._element;

			// cleanup:
			this.#valueObserver?.destroy();
			this.#configObserver?.destroy();
			oldElement?.removeEventListener('change', this._onPropertyEditorChange as any as EventListener);
			/** @deprecated The `UmbPropertyValueChangeEvent` has been deprecated, and will be removed in Umbraco 18. [LK] */
			oldElement?.removeEventListener('property-value-change', this._onPropertyEditorChange as any as EventListener);
			oldElement?.destroy?.();

			this._element = el as ManifestPropertyEditorUi['ELEMENT_TYPE'];

			this.#propertyContext.setEditor(this._element);

			if (this._element) {
				this._element.addEventListener('change', this._onPropertyEditorChange as any as EventListener);
				/** @deprecated The `UmbPropertyValueChangeEvent` has been deprecated, and will be removed in Umbraco 18. [LK] */
				this._element.addEventListener('property-value-change', this._onPropertyEditorChange as any as EventListener);
				// No need to observe mandatory or label, as we already do so and set it on the _element if present: [NL]
				this._element.manifest = manifest;

				// No need for a controller alias, as the clean is handled via the observer prop:
				this.#valueObserver = this.observe(
					this.#propertyContext.value,
					(value) => {
						// Set the value on the element:
						this._element!.value = value;
					},
					null
				);
				this.#configObserver = this.observe(
					this.#propertyContext.config,
					(config) => {
						if (config) {
							this._element!.config = config;
						}
					},
					null
				);

				this._element.readonly = this._isReadOnly;
				this._element.toggleAttribute('readonly', this._isReadOnly);

				this.#createController(manifest);
			}

			this.requestUpdate('element', oldElement);
		}
	}

	#createController(propertyEditorUiManifest: ManifestPropertyEditorUi): void {
		if (this.#extensionsController) {
			this.#extensionsController.destroy();
		}

		this.#extensionsController = new UmbExtensionsApiInitializer(
			this,
			umbExtensionsRegistry,
			'propertyContext',
			[],
			(manifest) => manifest.forPropertyEditorUis.includes(propertyEditorUiManifest.alias)
		);
	}

	override disconnectedCallback() {
		super.disconnectedCallback();
		this._element?.removeEventListener('change', this._onPropertyEditorChange as any as EventListener);
		/** @deprecated The `UmbPropertyValueChangeEvent` has been deprecated, and will be removed in Umbraco 18. [LK] */
		this._element?.removeEventListener('property-value-change', this._onPropertyEditorChange as any as EventListener);
	}

	override render() {
		return this._element;
	}
}

declare global {
	interface HTMLElementTagNameMap {
		'contentment-input-list-property-editor': ContentmentInputListPropertyEditorElement;
	}
}
