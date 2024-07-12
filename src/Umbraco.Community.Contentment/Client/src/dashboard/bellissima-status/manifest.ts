import type { ManifestDashboard } from '@umbraco-cms/backoffice/extension-registry';

export const manifest: ManifestDashboard = {
	type: 'dashboard',
	alias: 'Umb.Contentment.Dashboard.BellissimaStatus',
	name: '[Contentment] Bellissima Status Dashboard',
	element: () => import('./bellissima-status.element.js'),
	weight: 1000,
	meta: {
		label: 'Contentment Status',
		pathname: 'contentment-bellissima-status',
	},
	conditions: [
		{
			alias: 'Umb.Condition.SectionAlias',
			match: 'Umb.Section.Settings',
		},
	],
};
