/* Copyright © 2019 Lee Kelleher, Umbrella Inc and other contributors.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

angular.module("umbraco").controller("Our.Umbraco.Contentment.DataEditors.Dropdown.Controller", [
    "$scope",
    function ($scope) {

        //console.log("model", $scope.model);

        var defaultConfig = { items: [], allowEmpty: 1 };
        var config = angular.extend({}, defaultConfig, $scope.model.config);

        var vm = this;
        vm.allowEmpty = Object.toBoolean(config.allowEmpty); // TODO: Review the naming of `allowEmpty` [LK]

        function init() {
            $scope.model.value = $scope.model.value || "";

            _.each(config.items, function (item) {
                if (item.hasOwnProperty("enabled")) {
                    item.disabled = item.enabled === "0" || item.enabled === 0;
                }
                return item;
            });

            vm.items = config.items;
        };

        init();
    }
]);
