export { CancelablePromise, CancelError } from '@umbraco-cms/backoffice/external/backend-api';

export interface OnCancel {
	readonly isResolved: boolean;
	readonly isRejected: boolean;
	readonly isCancelled: boolean;

	(cancelHandler: () => void): void;
}
