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
                        element.attr(x.name, x.value);
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
