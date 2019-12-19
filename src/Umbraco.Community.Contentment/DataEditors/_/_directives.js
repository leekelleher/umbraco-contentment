/* Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

angular.module("umbraco.directives").directive("umbHtmlAttributes", [
    function () {
        return {
            restrict: "A",
            scope: {
                attributes: "&umbHtmlAttributes"
            },
            link: function (scope, element, attrs) {
                var attributes = scope.attributes();
                if (_.isArray(attributes) && attributes.length > 0) {
                    _.each(attributes, function (x) {
                        element.attr(x.name, x.value);
                    });
                }
            }
        };
    }
]);
