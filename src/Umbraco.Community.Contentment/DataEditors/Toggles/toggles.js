/* Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

angular.module("umbraco").controller("Umbraco.Community.Contentment.DataEditors.Toggles.Controller", [
    "$scope",
    function ($scope) {

        // console.log("toggles.model", $scope.model);

        var defaultConfig = { items: [], showDescriptions: 1, defaultValue: [] };
        var config = angular.extend({}, defaultConfig, $scope.model.config);

        var vm = this;

        function init() {
            $scope.model.value = $scope.model.value || config.defaultValue;

            if (_.isArray($scope.model.value) === false) {
                $scope.model.value = [$scope.model.value];
            }

            vm.items = angular.copy(config.items);

            var removeDescriptions = Object.toBoolean(config.showDescriptions) == false;

            _.each(vm.items, function (item) {
                if (removeDescriptions) delete item.description;
                item.checked = _.contains($scope.model.value, item.value);
            });

            vm.changed = changed;
        };

        function changed(item) {

            $scope.model.value = [];

            _.each(vm.items, function (x) {
                if (x.checked) {
                    $scope.model.value.push(x.value);
                }
            });

            setDirty();
        };

        function setDirty() {
            if ($scope.propertyForm) {
                $scope.propertyForm.$setDirty();
            }
        };

        init();
    }
]);
