/* Copyright © 2019 Lee Kelleher, Umbrella Inc and other contributors.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

angular.module("umbraco").controller("Our.Umbraco.Contentment.DataEditors.DropdownList.Controller", [
    "$scope",
    function ($scope) {

        //console.log("dropdown-list.model", $scope.model);

        var defaultConfig = { items: [], allowEmpty: 1, defaultValue: "" };
        var config = angular.extend({}, defaultConfig, $scope.model.config);

        var vm = this;

        function init() {
            $scope.model.value = $scope.model.value || config.defaultValue;

            if (_.isArray($scope.model.value)) {
                $scope.model.value = _.first($scope.model.value);
            }

            _.each(config.items, function (item) {
                if (item.hasOwnProperty("enabled")) {
                    item.disabled = item.enabled === "0" || item.enabled === 0;
                }
                return item;
            });

            vm.items = config.items;

            vm.allowEmpty = Object.toBoolean(config.allowEmpty);
        };

        init();
    }
]);
