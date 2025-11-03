import { ContentmentContentBlocksEntryContext } from './content-blocks-entry.context.js';
import { css, customElement, html, nothing, property, state, when } from '@umbraco-cms/backoffice/external/lit';
import { UmbDataPathBlockElementDataQuery } from '@umbraco-cms/backoffice/block';
import { UmbLitElement, umbDestroyOnDisconnect } from '@umbraco-cms/backoffice/lit-element';
import { UmbObserveValidationStateController } from '@umbraco-cms/backoffice/validation';
import { UUIBlinkAnimationValue, UUIBlinkKeyframes } from '@umbraco-cms/backoffice/external/uui';
import type { UmbBlockEditorCustomViewProperties } from '@umbraco-cms/backoffice/block-custom-view';
import type { UmbBlockListLayoutModel } from '@umbraco-cms/backoffice/block-list';
import type { UmbPropertyEditorUiElement } from '@umbraco-cms/backoffice/property-editor';

@customElement('contentment-content-blocks-entry')
export class ContentmentContentBlocksEntryElement extends UmbLitElement implements UmbPropertyEditorUiElement {
	@property({ type: Number })
	public set index(value: number | undefined) {
		this.#context.setIndex(value);
	}
	public get index(): number | undefined {
		return this.#context.getIndex();
	}

	@property({ attribute: false })
	public set contentKey(value: string | undefined) {
		if (!value) return;
		this._contentKey = value;
		this.#context.setContentKey(value);

		new UmbObserveValidationStateController(
			this,
			`$.contentData[${UmbDataPathBlockElementDataQuery({ key: value })}]`,
			(hasMessages) => {
				this._contentInvalid = hasMessages;
				this._blockViewProps.contentInvalid = hasMessages;
			},
			'observeMessagesForContent'
		);
	}
	public get contentKey(): string | undefined {
		return this._contentKey;
	}
	private _contentKey?: string | undefined;

	#context = new ContentmentContentBlocksEntryContext(this);

	@state()
	private _contentTypeAlias?: string;

	@state()
	private _contentTypeName?: string;

	@state()
	private _showContentEdit = false;

	@state()
	private _label = '';

	@state()
	private _icon?: string;

	@state()
	private _exposed?: boolean;

	@state()
	private _unsupported?: boolean;

	@state()
	private _showActions?: boolean;

	@state()
	private _workspaceEditContentPath?: string;

	// 'content-invalid' attribute is used for styling purpose.
	@property({ type: Boolean, attribute: 'content-invalid', reflect: true })
	private _contentInvalid?: boolean;

	@state()
	private _blockViewProps: UmbBlockEditorCustomViewProperties<UmbBlockListLayoutModel> = {
		contentKey: undefined!,
		config: { showContentEdit: false, showSettingsEdit: false },
	}; // Set to undefined cause it will be set before we render.

	#updateBlockViewProps(incoming: Partial<UmbBlockEditorCustomViewProperties<UmbBlockListLayoutModel>>) {
		this._blockViewProps = { ...this._blockViewProps, ...incoming };
		this.requestUpdate('_blockViewProps');
	}

	@state()
	private _isReadOnly = false;

	constructor() {
		super();
		this.#init();
	}

	#init() {
		this.observe(
			this.#context.showContentEdit,
			(showContentEdit) => {
				this._showContentEdit = showContentEdit;
				this.#updateBlockViewProps({ config: { ...this._blockViewProps.config!, showContentEdit } });
			},
			null
		);

		this.observe(
			this.#context.blockType,
			(blockType) => {
				this.#updateBlockViewProps({ blockType });
			},
			null
		);

		this.observe(this.#context.index, (index) => this.#updateBlockViewProps({ index }), null);

		this.observe(
			this.#context.label,
			(label) => {
				this.#updateBlockViewProps({ label });
				this._label = label;
			},
			null
		);

		this.observe(
			this.#context.contentElementTypeIcon,
			(icon) => {
				this.#updateBlockViewProps({ icon });
				this._icon = icon;
			},
			null
		);

		this.observe(
			this.#context.hasExpose,
			(exposed) => {
				this.#updateBlockViewProps({ unpublished: !exposed });
				this._exposed = exposed;
			},
			null
		);

		this.observe(
			this.#context.unsupported,
			(unsupported) => {
				if (unsupported === undefined) return;
				this.#updateBlockViewProps({ unsupported: unsupported });
				this._unsupported = unsupported;
				this.toggleAttribute('unsupported', unsupported);
			},
			null
		);

		this.observe(
			this.#context.actionsVisibility,
			(showActions) => {
				this._showActions = showActions;
			},
			null
		);

		// Data props:
		this.observe(
			this.#context.layout,
			(layout) => {
				this.#updateBlockViewProps({ layout });
			},
			null
		);

		this.#observeData();

		this.observe(
			this.#context.workspaceEditContentPath,
			(path) => {
				this._workspaceEditContentPath = path;
				this.#updateBlockViewProps({ config: { ...this._blockViewProps.config!, editContentPath: path } });
			},
			null
		);
		this.observe(
			this.#context.readOnlyGuard.permitted,
			(isReadOnly) => (this._isReadOnly = isReadOnly),
			'umbReadOnlyObserver'
		);
	}

	async #observeData() {
		this.observe(
			await this.#context.contentValues(),
			(content) => {
				this.#updateBlockViewProps({ content });
			},
			null
		);
	}

	override connectedCallback(): void {
		super.connectedCallback();
		// element styling:
		this.observe(
			this.#context.contentElementTypeKey,
			(contentElementTypeKey) => {
				if (contentElementTypeKey) {
					this.setAttribute('data-content-element-type-key', contentElementTypeKey);
				}
			},
			'contentElementTypeKey'
		);
		this.observe(
			this.#context.contentElementTypeAlias,
			(contentElementTypeAlias) => {
				if (contentElementTypeAlias) {
					this._contentTypeAlias = contentElementTypeAlias;
					this.setAttribute('data-content-element-type-alias', contentElementTypeAlias);
				}
			},
			'contentElementTypeAlias'
		);
		this.observe(
			this.#context.contentElementTypeName,
			(contentElementTypeName) => {
				this._contentTypeName = contentElementTypeName;
			},
			'contentElementTypeName'
		);
	}

	#expose = () => {
		this.#context.expose();
	};

	#renderRefBlock() {
		return html`<umb-ref-list-block
			.label=${this._label}
			.icon=${this._icon}
			.index=${this._blockViewProps.index}
			.unpublished=${!this._exposed}
			.config=${this._blockViewProps.config}
			.content=${this._blockViewProps.content}
			.settings=${this._blockViewProps.settings}
			${umbDestroyOnDisconnect()}></umb-ref-list-block>`;
	}

	#renderUnsupportedBlock() {
		return html`<umb-unsupported-list-block
			.config=${this._blockViewProps.config}
			.content=${this._blockViewProps.content}
			.settings=${this._blockViewProps.settings}
			${umbDestroyOnDisconnect()}></umb-unsupported-list-block>`;
	}

	#renderBuiltinBlockView = () => {
		if (this._unsupported) {
			return this.#renderUnsupportedBlock();
		}

		return this.#renderRefBlock();
	};

	#renderBlock() {
		if (!this.contentKey || (this._contentTypeAlias && this._unsupported)) return nothing;
		return html`
			<div class="umb-block-list__block">
				${this.#renderBuiltinBlockView()} ${this.#renderActionBar()}
				${when(
					!this._showContentEdit && this._contentInvalid,
					() => html`<uui-badge attention color="invalid" label="Invalid content">!</uui-badge>`
				)}
			</div>
		`;
	}

	#renderActionBar() {
		if (!this._showActions) return nothing;
		return html` <uui-action-bar> ${this.#renderEditContentAction()} ${this.#renderDeleteAction()} </uui-action-bar> `;
	}

	#renderEditContentAction() {
		return this._showContentEdit && this._workspaceEditContentPath
			? html`
					<uui-button
						label="edit"
						look="secondary"
						color=${this._contentInvalid ? 'invalid' : ''}
						href=${this._workspaceEditContentPath}>
						<uui-icon
							name=${this._exposed === false && this._isReadOnly === false ? 'icon-add' : 'icon-edit'}></uui-icon>
						${when(
							this._contentInvalid,
							() => html`<uui-badge attention color="invalid" label="Invalid content">!</uui-badge>`
						)}
					</uui-button>
			  `
			: this._showContentEdit === false && this._exposed === false
			? html`
					<uui-button
						@click=${this.#expose}
						label=${this.localize.term('blockEditor_createThisFor', this._contentTypeName)}
						look="secondary">
						<uui-icon name="icon-add"></uui-icon>
					</uui-button>
			  `
			: nothing;
	}

	#renderDeleteAction() {
		if (this._isReadOnly) return nothing;
		return html`
			<uui-button label="delete" look="secondary" @click=${() => this.#context.requestDelete()}>
				<uui-icon name="icon-remove"></uui-icon>
			</uui-button>
		`;
	}

	override render() {
		return this.#renderBlock();
	}

	static override styles = [
		UUIBlinkKeyframes,
		css`
			:host {
				position: relative;
				display: block;
				--umb-block-list-entry-actions-opacity: 0;
			}

			:host([settings-invalid]),
			:host([content-invalid]),
			:host(:hover),
			:host(:focus-within) {
				--umb-block-list-entry-actions-opacity: 1;
			}

			:host::after {
				content: '';
				position: absolute;
				z-index: 1;
				pointer-events: none;
				inset: 0;
				border: 1px solid transparent;
				border-radius: var(--uui-border-radius);

				transition: border-color 240ms ease-in;
			}

			:host([settings-invalid])::after,
			:host([content-invalid])::after {
				border-color: var(--uui-color-invalid);
			}

			umb-extension-slot::part(component) {
				position: relative;
				z-index: 0;
			}

			uui-action-bar {
				position: absolute;
				top: var(--uui-size-2);
				right: var(--uui-size-2);
				opacity: var(--umb-block-list-entry-actions-opacity, 0);
				transition: opacity 120ms;
			}

			uui-badge {
				z-index: 2;
			}

			:host::after {
				content: '';
				position: absolute;
				z-index: 1;
				pointer-events: none;
				inset: 0;
				border: 1px solid transparent;
				border-radius: var(--uui-border-radius);

				transition: border-color 240ms ease-in;
			}
			:host(:hover):not(:drop)::after {
				display: block;
				border-color: var(--uui-color-interactive-emphasis);
				box-shadow: 0 0 0 1px rgba(255, 255, 255, 0.7), inset 0 0 0 1px rgba(255, 255, 255, 0.7);
			}

			:host([drag-placeholder])::after {
				display: block;
				border-width: 2px;
				border-color: var(--uui-color-interactive-emphasis);
				animation: ${UUIBlinkAnimationValue};
			}
			:host([drag-placeholder])::before {
				content: '';
				position: absolute;
				pointer-events: none;
				inset: 0;
				border-radius: var(--uui-border-radius);
				background-color: var(--uui-color-interactive-emphasis);
				opacity: 0.12;
			}
			:host([drag-placeholder]) .umb-block-list__block {
				transition: opacity 50ms 16ms;
				opacity: 0;
			}
		`,
	];
}

export default ContentmentContentBlocksEntryElement;

declare global {
	interface HTMLElementTagNameMap {
		'contentment-content-blocks-entry': ContentmentContentBlocksEntryElement;
	}
}
