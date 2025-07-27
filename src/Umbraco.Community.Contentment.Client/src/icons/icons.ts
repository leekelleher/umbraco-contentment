// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

import type { UmbIconDictionary } from '@umbraco-cms/backoffice/icon';

const iconPath = '/App_Plugins/Contentment/icons';

const contentment: UmbIconDictionary = [{ name: 'icon-contentment', path: `${iconPath}/icon-contentment.js` }];

const fontAwesome: UmbIconDictionary = [
	{ name: 'icon-fa-arrow-down-1-9', path: `${iconPath}/fa/icon-fa-arrow-down-1-9.js` },
	{ name: 'icon-fa-arrow-pointer', path: `${iconPath}/fa/icon-fa-arrow-pointer.js` },
	{ name: 'icon-fa-circle-exclamation', path: `${iconPath}/fa/icon-fa-circle-exclamation.js` },
	{ name: 'icon-fa-codepen', path: `${iconPath}/fa/icon-fa-codepen.js` },
	{ name: 'icon-fa-css3', path: `${iconPath}/fa/icon-fa-css3.js` },
	{ name: 'icon-fa-earth-africa', path: `${iconPath}/fa/icon-fa-earth-africa.js` },
	{ name: 'icon-fa-folder-tree', path: `${iconPath}/fa/icon-fa-folder-tree.js` },
	{ name: 'icon-fa-file-lines', path: `${iconPath}/fa/icon-fa-file-lines.js` },
	{ name: 'icon-fa-language', path: `${iconPath}/fa/icon-fa-language.js` },
	{ name: 'icon-fa-list-check', path: `${iconPath}/fa/icon-fa-list-check.js` },
	{ name: 'icon-fa-list-ul', path: `${iconPath}/fa/icon-fa-list-ul.js` },
	{ name: 'icon-fa-server', path: `${iconPath}/fa/icon-fa-server.js` },
	{ name: 'icon-fa-signal', path: `${iconPath}/fa/icon-fa-signal.js` },
	{ name: 'icon-fa-square-caret-down', path: `${iconPath}/fa/icon-fa-square-caret-down.js` },
	{ name: 'icon-fa-table-list', path: `${iconPath}/fa/icon-fa-table-list.js` },
	{ name: 'icon-fa-tags', path: `${iconPath}/fa/icon-fa-tags.js` },
];

const socials: UmbIconDictionary = [
	{ name: 'icon-bluesky', path: `${iconPath}/social/icon-bluesky.js` },
	{ name: 'icon-discord', path: `${iconPath}/social/icon-discord.js` },
	{ name: 'icon-instagram', path: `${iconPath}/social/icon-instagram.js` },
	{ name: 'icon-threads', path: `${iconPath}/social/icon-threads.js` },
	{ name: 'icon-tiktok', path: `${iconPath}/social/icon-tiktok.js` },
	{ name: 'icon-x-twitter', path: `${iconPath}/social/icon-x-twitter.js` },
	{ name: 'icon-youtube', path: `${iconPath}/social/icon-youtube.js` },
];

export default [...contentment, ...fontAwesome, ...socials];
