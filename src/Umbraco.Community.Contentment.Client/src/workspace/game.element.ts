// SPDX-License-Identifier: MIT
// Copyright © 2026 Lee Kelleher

import { css, customElement, html, nothing, repeat, state } from '@umbraco-cms/backoffice/external/lit';
import { UmbLitElement } from '@umbraco-cms/backoffice/lit-element';
import { UmbTextStyles } from '@umbraco-cms/backoffice/style';

import icons from '../icons/icons.js';
import '../components/info-box/info-box.element.js';

const COLS = 4;
const PAIRS = 8;
const FLIP_BACK_DELAY = 800;

const CARD_BACK = 'icon-umbraco';
const ICON_POOL = icons.map((icon) => icon.name).filter((name) => name !== CARD_BACK);

type ContentmentGameCard = { icon: string; state: ContentmentGameCardState };

type ContentmentGameCardState = 'down' | 'up' | 'matched';

type ContentmentGameStatus = 'playing' | 'won';

@customElement('contentment-game')
export default class ContentmentGameElement extends UmbLitElement {
	@state()
	private _cards: Array<ContentmentGameCard> = [];

	@state()
	private _status: ContentmentGameStatus = 'playing';

	@state()
	private _moves = 0;

	@state()
	private _locked = false;

	#timer?: ReturnType<typeof setTimeout>;

	constructor() {
		super();
		this.#reset();
	}

	override disconnectedCallback() {
		super.disconnectedCallback();
		clearTimeout(this.#timer);
	}

	#reset() {
		clearTimeout(this.#timer);

		const pool = [...ICON_POOL];
		for (let i = pool.length - 1; i > 0; i--) {
			const j = Math.floor(Math.random() * (i + 1));
			[pool[i], pool[j]] = [pool[j], pool[i]];
		}

		const cards = [...pool.slice(0, PAIRS), ...pool.slice(0, PAIRS)].map(
			(icon): ContentmentGameCard => ({ icon, state: 'down' }),
		);

		for (let i = cards.length - 1; i > 0; i--) {
			const j = Math.floor(Math.random() * (i + 1));
			[cards[i], cards[j]] = [cards[j], cards[i]];
		}

		this._cards = cards;
		this._status = 'playing';
		this._moves = 0;
		this._locked = false;
	}

	#flip(index: number) {
		if (this._locked || this._status === 'won') return;

		const card = this._cards[index];
		if (card.state !== 'down') return;

		this._cards = this._cards.map((c, i) => (i === index ? { ...c, state: 'up' } : c));

		const up = this._cards.filter((c) => c.state === 'up');
		if (up.length === 2) {
			this._moves++;
			this.#resolve();
		}
	}

	#resolve() {
		const up = this._cards.filter((c) => c.state === 'up');
		const [first, second] = up;

		if (first.icon === second.icon) {
			this._cards = this._cards.map((c) => (c.state === 'up' ? { ...c, state: 'matched' } : c));
			this.#checkWin();
			return;
		}

		this._locked = true;
		this.#timer = setTimeout(() => this.#flipBack(), FLIP_BACK_DELAY);
	}

	#flipBack() {
		this._cards = this._cards.map((c) => (c.state === 'up' ? { ...c, state: 'down' } : c));
		this._locked = false;
	}

	#checkWin() {
		if (this._cards.every((c) => c.state === 'matched')) {
			this._status = 'won';
		}
	}

	#onGridClick(event: Event & { target: HTMLElement }) {
		const index = event.target.closest<HTMLElement>('[data-index]')?.dataset.index;
		if (index) this.#flip(Number(index));
	}

	#onReset() {
		this.#reset();
	}

	override render() {
		return html`
			<uui-box headline="Icon match">
				<div slot="header-actions">
					<span id="moves"><umb-icon name="icon-playing-cards"></umb-icon> ${this._moves}</span>
					<uui-button compact label="New game" title="New game" @click=${this.#onReset}>
						<umb-icon name="icon-refresh"></umb-icon>
					</uui-button>
				</div>

				<div id="board">
					${this.#renderEndState()}

					<div id="grid" role="presentation" aria-hidden="true" @click=${this.#onGridClick}>
						${repeat(
							this._cards,
							(_card, index) => index,
							(card, index) => this.#renderCard(card, index),
						)}
					</div>
				</div>
			</uui-box>
		`;
	}

	#renderEndState() {
		if (this._status !== 'won') return nothing;

		return html`
			<contentment-info-box compact type="positive" icon="icon-trophy" headline="Matched in ${this._moves} moves!">
				<uui-button color="positive" look="secondary" label="Play again" @click=${this.#onReset}></uui-button>
			</contentment-info-box>
		`;
	}

	#renderCard(card: ContentmentGameCard, index: number) {
		return html`
			<div class="cell ${card.state}" data-index=${index}>
				<umb-icon name=${card.state === 'down' ? CARD_BACK : card.icon}></umb-icon>
			</div>
		`;
	}

	static override styles = [
		UmbTextStyles,
		css`
			uui-box {
				--uui-color-interactive: var(--uui-color-text);
			}

			div[slot='header-actions'] {
				display: flex;
				align-items: center;
				gap: var(--uui-size-space-2);

				umb-icon {
					font-size: var(--uui-size-6);
				}

				#moves {
					display: flex;
					gap: var(--uui-size-2);
				}
			}

			#board {
				position: relative;
			}

			contentment-info-box {
				position: absolute;
				inset: 0;
				z-index: 1;
				margin: auto;
				width: fit-content;
				height: fit-content;
				max-width: 90%;

				uui-button {
					margin-top: var(--uui-size-space-3);
				}
			}

			#grid {
				display: grid;
				grid-template-columns: repeat(${COLS}, 1fr);
				gap: 2px;
			}

			.cell {
				aspect-ratio: 1;
				display: grid;
				place-items: center;

				background-color: var(--uui-color-background);
				border: 1px solid var(--uui-color-divider);
				border-right-color: var(--uui-color-border-standalone);
				border-bottom-color: var(--uui-color-border-standalone);
				cursor: pointer;

				umb-icon {
					font-size: var(--uui-size-6);
					color: var(--uui-color-border-emphasis);
				}

				&.up,
				&.matched {
					background-color: var(--uui-color-surface);

					umb-icon {
						color: var(--uui-color-selected);
					}
				}

				&.matched {
					cursor: default;

					umb-icon {
						color: var(--uui-color-positive-standalone);
					}
				}
			}
		`,
	];
}

declare global {
	interface HTMLElementTagNameMap {
		'contentment-game': ContentmentGameElement;
	}
}
