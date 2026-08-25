// SPDX-License-Identifier: MIT
// Copyright © 2026 Lee Kelleher

import { css, customElement, html, repeat, state, styleMap } from '@umbraco-cms/backoffice/external/lit';
import { UmbLitElement } from '@umbraco-cms/backoffice/lit-element';
import { UmbTextStyles } from '@umbraco-cms/backoffice/style';

const COLS = 4;
const CELLS = COLS * COLS;
const GAP = -1;
const SHUFFLE_MOVES = 200;
const TILE_ICONS = ['icon-contentment', 'icon-umbraco', 'icon-umb-heart'];

type ContentmentGameStatus = 'playing' | 'won';

@customElement('contentment-game')
export default class ContentmentGameElement extends UmbLitElement {
	@state()
	private _board: Array<number> = [];

	@state()
	private _status: ContentmentGameStatus = 'playing';

	@state()
	private _moves = 0;

	@state()
	private _icon = TILE_ICONS[0];

	constructor() {
		super();
		this.#reset();
	}

	#neighbours(position: number) {
		const row = Math.floor(position / COLS);
		const col = position % COLS;
		const result: Array<number> = [];
		if (row > 0) result.push(position - COLS);
		if (row < COLS - 1) result.push(position + COLS);
		if (col > 0) result.push(position - 1);
		if (col < COLS - 1) result.push(position + 1);
		return result;
	}

	#isSolved(board: Array<number>) {
		return board.every((id, i) => (i === CELLS - 1 ? id === GAP : id === i));
	}

	#reset() {
		const board = Array.from({ length: CELLS }, (_, i) => (i === CELLS - 1 ? GAP : i));

		let previous = -1;
		for (let i = 0; i < SHUFFLE_MOVES; i++) {
			const gap = board.indexOf(GAP);
			const options = this.#neighbours(gap).filter((n) => n !== previous);
			const next = options[Math.floor(Math.random() * options.length)];
			[board[gap], board[next]] = [board[next], board[gap]];
			previous = gap;
		}

		if (this.#isSolved(board)) {
			this.#reset();
			return;
		}

		this._board = board;
		this._status = 'playing';
		this._moves = 0;
		this._icon = TILE_ICONS[Math.floor(Math.random() * TILE_ICONS.length)];
	}

	#move(position: number) {
		if (this._status === 'won') return;

		const gap = this._board.indexOf(GAP);
		if (!this.#neighbours(gap).includes(position)) return;

		const board = [...this._board];
		[board[gap], board[position]] = [board[position], board[gap]];

		this._board = board;
		this._moves++;
		this.#checkWin();
	}

	#checkWin() {
		if (this.#isSolved(this._board)) {
			this._status = 'won';
		}
	}

	#onGridClick(event: Event & { target: HTMLElement }) {
		const position = event.target.closest<HTMLElement>('[data-position]')?.dataset.position;
		if (position) this.#move(Number(position));
	}

	#onReset() {
		this.#reset();
	}

	override render() {
		return html`
			<uui-box headline="Tile slide">
				<div slot="header-actions">
					<span id="moves"><umb-icon name="icon-puzzle-piece"></umb-icon> ${this._moves}</span>
					<uui-button compact label="New game" title="New game" @click=${this.#onReset}>
						<umb-icon name="icon-refresh"></umb-icon>
					</uui-button>
				</div>

				<div id="grid" class=${this._status} role="presentation" aria-hidden="true" @click=${this.#onGridClick}>
					${this.#renderTiles()}
				</div>
			</uui-box>
		`;
	}

	#renderTiles() {
		const tiles = Array.from({ length: CELLS - 1 }, (_, id) => id);
		return repeat(
			tiles,
			(id) => id,
			(id) => this.#renderTile(id),
		);
	}

	#renderTile(id: number) {
		const position = this._board.indexOf(id);
		const row = Math.floor(position / COLS);
		const col = position % COLS;
		const homeRow = Math.floor(id / COLS);
		const homeCol = id % COLS;

		const tileStyle = styleMap({ transform: `translate(${col * 100}%, ${row * 100}%)` });
		const iconStyle = styleMap({ transform: `translate(${homeCol * -25}%, ${homeRow * -25}%)` });

		return html`
			<div class="tile" data-position=${position} style=${tileStyle}>
				<umb-icon name=${this._icon} style=${iconStyle}></umb-icon>
				<span class="badge">${id + 1}</span>
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

			#grid {
				position: relative;
				aspect-ratio: 1;
				container-type: inline-size;
			}

			.tile {
				position: absolute;
				top: 0;
				left: 0;
				width: calc(100% / 4);
				height: calc(100% / 4);
				box-sizing: border-box;
				overflow: hidden;
				cursor: pointer;

				background-color: var(--uui-color-background);
				border: 1px solid var(--uui-color-divider);
				border-right-color: var(--uui-color-border-standalone);
				border-bottom-color: var(--uui-color-border-standalone);

				transition: transform 150ms ease;

				umb-icon {
					position: absolute;
					top: 0;
					left: 0;
					font-size: calc(100cqw / 1.125);
					color: var(--uui-color-border-emphasis);
					transition: color 300ms ease;
				}

				.badge {
					position: absolute;
					top: 2px;
					left: 4px;
					padding: 0 4px;
					font-size: 12px;
					font-weight: bold;
					color: var(--uui-color-text);
					background-color: var(--uui-color-surface);
					border-radius: var(--uui-border-radius);
					transition: opacity 300ms ease;
				}
			}

			#grid.won {
				.badge {
					opacity: 0;
				}

				.tile umb-icon {
					color: var(--uui-color-positive-standalone);
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
