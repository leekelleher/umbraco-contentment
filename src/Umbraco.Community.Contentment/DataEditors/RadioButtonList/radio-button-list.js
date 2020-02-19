/* Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

angular.module("umbraco").controller("Umbraco.Community.Contentment.DataEditors.RadioButtonList.Controller", [
    "$scope",
    function ($scope) {

        // console.log("radiobuttonlist.model", $scope.model);

        var defaultConfig = {
            items: [],
            showDescriptions: 1,
            showIcons: 0,
            defaultValue: ""
        };
        var config = angular.extend({}, defaultConfig, $scope.model.config);

        var vm = this;

        function init() {
            $scope.model.value = $scope.model.value || config.defaultValue;

            if (_.isArray($scope.model.value)) {
                $scope.model.value = _.first($scope.model.value);
            }

            vm.items = angular.copy(config.items);

            vm.showDescriptions = Object.toBoolean(config.showDescriptions);
            vm.showIcons = Object.toBoolean(config.showIcons);

            vm.uniqueId = _.has($scope.model, "dataTypeKey")
                ? [$scope.model.alias, $scope.model.dataTypeKey.substring(0, 8)].join("-")
                : $scope.model.alias;
        };

        init();
    }
]);
