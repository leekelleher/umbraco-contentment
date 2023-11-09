/* Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

angular.module("umbraco").controller("Umbraco.Community.Contentment.DataEditors.DropdownList.Controller", [
    "$scope",
    function ($scope) {

        //console.log("dropdown-list.model", $scope.model);

        var defaultConfig = {
            items: [],
            allowEmpty: 1,
            defaultValue: "",
            htmlAttributes: []
        };
        var config = Object.assign({}, defaultConfig, $scope.model.config);

        var vm = this;

        function init() {

            config.showEmpty = Object.toBoolean(config.allowEmpty);

            vm.items = config.items.slice();

            $scope.model.value = $scope.model.value || config.defaultValue;

            if (Array.isArray($scope.model.value)) {
                $scope.model.value = $scope.model.value.length > 0 ? $scope.model.value[0] : config.defaultValue;
            }

            if (config.showEmpty === false && !$scope.model.value && vm.items.length > 0) {
                $scope.model.value = vm.items[0].value;
            }

            vm.showEmpty = config.showEmpty === true && vm.items.some(x => x.value === $scope.model.value);

            vm.htmlAttributes = config.htmlAttributes;

            vm.uniqueId = $scope.model.hasOwnProperty("dataTypeKey")
                ? [$scope.model.alias, $scope.model.dataTypeKey.substring(0, 8)].join("-")
                : $scope.model.alias;

            vm.change = change;
        };

        function change() {
            vm.showEmpty = config.showEmpty === true && vm.items.some(x => x.value === $scope.model.value);
        };

        init();
    }
]);
