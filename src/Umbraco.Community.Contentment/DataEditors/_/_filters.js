/* Copyright © 2020 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

angular.module("umbraco.filters").filter("lkNodeName", [
    "$filter",
    function ($filter) {
        return function (input) {

            if (Array.isArray(input) === false) {
                return $filter("ncNodeName")(input);
            }

            return input.map(function (x) {
                return $filter("ncNodeName")(x);
            }).join(", ");

        };
    }
]);

angular.module("umbraco.filters").filter("lkGroupBy", [
    function () {
        return _.memoize(function (items, field) {
            return _.chain(items)
                .sortBy(field)
                .groupBy(field)
                .value();
        });
    }
]);
