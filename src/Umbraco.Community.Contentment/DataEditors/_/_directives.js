/* Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

angular.module("umbraco.directives.html").directive("lkHtmlAttributes", [
    function () {
        return {
            restrict: "A",
            scope: {
                attributes: "&lkHtmlAttributes"
            },
            link: function (scope, element, attrs) {
                var attributes = scope.attributes();
                if (Array.isArray(attributes) && attributes.length > 0) {
                    attributes.forEach(function (x) {
                        if (x.name === "class") {
                            // NOTE: Slight bug, it did not account for existing class values.
                            element.addClass(x.value);
                        } else {
                            element.attr(x.name, x.value);
                        }
                    });
                }
            }
        };
    }
]);

angular.module("umbraco.directives.html").directive("lkBindHtmlTemplate", [
    "$compile",
    function ($compile) {
        return {
            restrict: "A",
            replace: true,
            link: function (scope, element, attrs) {
                scope.$watch(
                    function (scope) {
                        return scope.$eval(attrs.lkBindHtmlTemplate);
                    },
                    function (value) {
                        element.html(value);
                        $compile(element.contents())(scope);
                    }
                );
            }
        };
    }
]);

angular.module("umbraco.directives.html").directive("lkHidePropertyGroup", [
    function () {
        return {
            restrict: "A",
            link: function (scope, element, attrs) {
                if (attrs.lkHidePropertyGroup === "true") {

                    /* Copyright © 2020 Perplex Digital
                     * Parts of this Source Code has been derived from Perplex.ContentBlocks.
                     * https://github.com/PerplexDigital/Perplex.ContentBlocks/blob/v2.1.0/src/Perplex.ContentBlocks/App_Plugins/Perplex.ContentBlocks/perplex.content-blocks.controller.js#L1137-L1156
                     * Modified under the permissions of the MIT License.
                     * Modifications are licensed under the Mozilla Public License. */
                    var propertyContainer = element.closest(".umb-property-editor");
                    var isNestedProperty = propertyContainer.parent().closest(".umb-property-editor").length > 0;
                    if (isNestedProperty) {
                        // NOTE: Do not hide the containing property group if we are nested in some other property editor like NestedContent.
                        return;
                    }

                    // Starting from Umbraco 8.17 (and 9.0) it is possible to put properties in tabs again and not in a group at all. So we check for either.
                    var container = element.closest(".umb-group-panel,.umb-box");
                    if (container) {
                        container.addClass("umb-group-panel--hide");
                    }

                }
            }
        };
    }
]);
