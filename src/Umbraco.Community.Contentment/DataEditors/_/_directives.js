/* Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

angular.module("umbraco.directives.html").directive("umbHtmlAttributes", [
    function () {
        return {
            restrict: "A",
            scope: {
                attributes: "&umbHtmlAttributes"
            },
            link: function (scope, element, attrs) {
                var attributes = scope.attributes();
                if (_.isArray(attributes) && attributes.length > 0) { // TODO: Replace Underscore.js dependency. [LK:2020-03-02]
                    _.each(attributes, function (x) { // TODO: Replace Underscore.js dependency. [LK:2020-03-02]
                        element.attr(x.name, x.value);
                    });
                }
            }
        };
    }
]);
