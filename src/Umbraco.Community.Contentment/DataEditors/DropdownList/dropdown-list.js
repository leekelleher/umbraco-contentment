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
            $scope.model.value = $scope.model.value || config.defaultValue;

            if (Array.isArray($scope.model.value)) {
                $scope.model.value = $scope.model.value[0];
            }

            vm.items = config.items.slice();

            vm.allowEmpty = Object.toBoolean(config.allowEmpty);

            vm.htmlAttributes = config.htmlAttributes;

            vm.uniqueId = $scope.model.hasOwnProperty("dataTypeKey")
                ? [$scope.model.alias, $scope.model.dataTypeKey.substring(0, 8)].join("-")
                : $scope.model.alias;
        };

        init();
    }
]);
