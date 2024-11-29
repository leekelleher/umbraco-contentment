export { CancelablePromise } from '@umbraco-cms/backoffice/external/backend-api';

export class CancelError extends Error {
	constructor(message: string) {
		super(message);
		this.name = 'CancelError';
	}

	public get isCancelled(): boolean {
		return true;
	}
}

export interface OnCancel {
	readonly isResolved: boolean;
	readonly isRejected: boolean;
	readonly isCancelled: boolean;

	(cancelHandler: () => void): void;
}
