// SPDX-License-Identifier: MIT
// Copyright © 2021 A Beautiful Site, LLC
// This Source Code has been derived from an article by Cory LaViska.
// https://www.abeautifulsite.net/posts/finding-the-closest-element-through-shadow-roots/
// Copied under the permissions of the MIT License.
// Copyright © 2025 Lee Kelleher

export function closest(selector: string, root: Element) {
	function getNext(element: Element | HTMLElement, next = element && element.closest(selector)): Element | null {
		if (element instanceof Window || element instanceof Document || !element) {
			return null;
		}

		return next ? next : getNext((element.getRootNode() as ShadowRoot).host);
	}

	return getNext(root);
}
