/* Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

angular.module("umbraco").controller("Umbraco.Community.Contentment.DataEditors.Checkbox.Controller", [
    "$scope",
    function ($scope) {

        //console.log("checkbox.model", $scope.model);

        var defaultConfig = { showInline: 0, defaultValue: 0 };
        var config = angular.extend({}, defaultConfig, $scope.model.config); // TODO: Replace AngularJS dependency. [LK:2020-03-02]

        var vm = this;

        function init() {

            $scope.model.value = $scope.model.value || config.defaultValue;

            vm.alias = $scope.model.alias;
            vm.true = 1;
            vm.false = 0;

            if (Object.toBoolean(config.showInline)) {
                vm.showInline = true;
                vm.label = $scope.model.label;
                vm.description = $scope.model.description;
            }

            $scope.model.value = Object.toBoolean($scope.model.value) ? vm.true : vm.false;

        };

        init();
    }
]);
