// SPDX-License-Identifier: MIT
// Copyright © 2026 Lee Kelleher

import { css, customElement, html, nothing, repeat, state } from '@umbraco-cms/backoffice/external/lit';
import { UmbLitElement } from '@umbraco-cms/backoffice/lit-element';
import { UmbTextStyles } from '@umbraco-cms/backoffice/style';

import '../components/info-box/info-box.element.js';

const ROWS = 7;
const COLS = 7;
const MINES = 8;

type ContentmentGameCell = {
	mine: boolean;
	adjacent: number;
	state: ContentmentGameCellState;
};

type ContentmentGameCellState = 'covered' | 'flagged' | 'revealed';

type ContentmentGameStatus = 'idle' | 'playing' | 'won' | 'lost';

@customElement('contentment-game')
export default class ContentmentGameElement extends UmbLitElement {
	@state()
	private _cells: Array<ContentmentGameCell> = [];

	@state()
	private _status: ContentmentGameStatus = 'idle';

	constructor() {
		super();
		this.#reset();
	}

	#reset() {
		this._cells = Array.from({ length: ROWS * COLS }, () => ({ mine: false, adjacent: 0, state: 'covered' }));
		this._status = 'idle';
	}

	#neighbours(index: number) {
		const row = Math.floor(index / COLS);
		const col = index % COLS;
		const result: Array<number> = [];

		for (let r = row - 1; r <= row + 1; r++) {
			for (let c = col - 1; c <= col + 1; c++) {
				if (r === row && c === col) continue;
				if (r < 0 || r >= ROWS || c < 0 || c >= COLS) continue;
				result.push(r * COLS + c);
			}
		}

		return result;
	}

	#seed(safeIndex: number) {
		const excluded = new Set([safeIndex, ...this.#neighbours(safeIndex)]);
		const candidates = this._cells.map((_, index) => index).filter((index) => !excluded.has(index));

		for (let i = candidates.length - 1; i > 0; i--) {
			const j = Math.floor(Math.random() * (i + 1));
			[candidates[i], candidates[j]] = [candidates[j], candidates[i]];
		}

		const mines = new Set(candidates.slice(0, MINES));

		this._cells = this._cells.map((cell, index) => ({
			...cell,
			mine: mines.has(index),
		}));

		this._cells = this._cells.map((cell, index) => ({
			...cell,
			adjacent: this.#neighbours(index).filter((n) => this._cells[n].mine).length,
		}));
	}

	#flood(startIndex: number) {
		const stack = [startIndex];
		const cells = [...this._cells];

		while (stack.length) {
			const index = stack.pop()!;
			const cell = cells[index];
			if (cell.state === 'revealed' || cell.mine) continue;

			cells[index] = { ...cell, state: 'revealed' };

			if (cell.adjacent === 0) {
				for (const n of this.#neighbours(index)) {
					if (cells[n].state === 'covered') stack.push(n);
				}
			}
		}

		this._cells = cells;
	}

	#checkWin() {
		const revealed = this._cells.filter((cell) => cell.state === 'revealed').length;
		if (revealed === ROWS * COLS - MINES) {
			this._status = 'won';
		}
	}

	#lose() {
		this._cells = this._cells.map((cell) => (cell.mine ? { ...cell, state: 'revealed' } : cell));
		this._status = 'lost';
	}

	#dig(index: number) {
		if (this._status === 'won' || this._status === 'lost') return;

		const cell = this._cells[index];
		if (cell.state !== 'covered') return;

		if (this._status === 'idle') {
			this.#seed(index);
			this._status = 'playing';
		}

		if (this._cells[index].mine) {
			this.#lose();
			return;
		}

		this.#flood(index);
		this.#checkWin();
	}

	#toggleFlag(index: number) {
		if (this._status === 'won' || this._status === 'lost') return;

		const cell = this._cells[index];
		if (cell.state === 'revealed') return;

		const flagged = this._cells.filter((c) => c.state === 'flagged').length;
		if (cell.state === 'covered' && flagged >= MINES) return;

		this._cells = this._cells.map((c, i) =>
			i === index ? { ...c, state: c.state === 'flagged' ? 'covered' : 'flagged' } : c,
		);
	}

	#onGridClick(event: Event & { target: HTMLElement }) {
		const index = event.target.closest<HTMLElement>('[data-index]')?.dataset.index;
		if (index) this.#dig(Number(index));
	}

	#onGridContextMenu(event: MouseEvent & { target: HTMLElement }) {
		event.preventDefault();
		const index = event.target.closest<HTMLElement>('[data-index]')?.dataset.index;
		if (index) this.#toggleFlag(Number(index));
	}

	#onReset() {
		this.#reset();
	}

	override render() {
		const flagged = this._cells.filter((cell) => cell.state === 'flagged').length;
		const remaining = MINES - flagged;

		return html`
			<uui-box headline="Minesweeper">
				<div slot="header-actions">
					<span id="remaining"><umb-icon name="icon-flag-alt"></umb-icon> ${remaining}</span>
					<uui-button compact label="New game" title="New game" @click=${this.#onReset}>
						<umb-icon name="icon-refresh"></umb-icon>
					</uui-button>
				</div>

				<div id="board">
					${this.#renderEndState()}

					<div
						id="grid"
						class=${this._status === 'idle' ? 'idle' : ''}
						role="presentation"
						aria-hidden="true"
						@click=${this.#onGridClick}
						@contextmenu=${this.#onGridContextMenu}>
						${repeat(
							this._cells,
							(_cell, index) => index,
							(cell, index) => this.#renderCell(cell, index),
						)}
					</div>
				</div>
			</uui-box>
		`;
	}

	#renderEndState() {
		if (this._status === 'won') {
			return html`
				<contentment-info-box compact type="positive" icon="icon-trophy" headline="You win!">
					<uui-button color="positive" look="secondary" label="Play again" @click=${this.#onReset}></uui-button>
				</contentment-info-box>
			`;
		}

		if (this._status === 'lost') {
			return html`
				<contentment-info-box compact type="danger" icon="icon-bomb" headline="Boom! Game over.">
					<uui-button color="danger" look="secondary" label="Play again" @click=${this.#onReset}></uui-button>
				</contentment-info-box>
			`;
		}

		return nothing;
	}

	#renderCell(cell: ContentmentGameCell, index: number) {
		if (cell.state === 'revealed') {
			return html`
				<div class="cell revealed" data-index=${index} data-adjacent=${cell.adjacent}>
					${cell.mine ? html`<umb-icon name="icon-bomb"></umb-icon>` : cell.adjacent || nothing}
				</div>
			`;
		}

		return html`
			<div class="cell" data-index=${index}>
				${cell.state === 'flagged' ? html`<umb-icon name="icon-flag-alt"></umb-icon>` : nothing}
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

				#remaining {
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
				color: var(--uui-color-selected);
				cursor: pointer;

				font-size: var(--uui-size-5);
				font-weight: bold;
				user-select: none;

				&.revealed {
					background-color: var(--uui-color-surface);
					border-color: var(--uui-color-divider);
					cursor: default;

					umb-icon {
						color: var(--uui-color-danger-standalone);
					}
				}

				&[data-adjacent='1'] {
					color: var(--uui-palette-malibu-dark);
				}
				&[data-adjacent='2'] {
					color: var(--uui-palette-forest-green);
				}
				&[data-adjacent='3'] {
					color: var(--uui-palette-maroon-flush);
				}
				&[data-adjacent='4'] {
					color: var(--uui-palette-violet-blue);
				}
				&[data-adjacent='5'] {
					color: var(--uui-palette-cocoa-brown);
				}
				&[data-adjacent='6'] {
					color: var(--uui-palette-jungle-green);
				}
				&[data-adjacent='7'] {
					color: var(--uui-palette-space-cadet);
				}
				&[data-adjacent='8'] {
					color: var(--uui-palette-mine-grey);
				}

				umb-icon {
					font-size: var(--uui-size-6);
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
