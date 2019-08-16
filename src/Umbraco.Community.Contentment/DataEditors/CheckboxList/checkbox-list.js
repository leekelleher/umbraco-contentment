/* Copyright © 2019 Lee Kelleher, Umbrella Inc and other contributors.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

angular.module("umbraco").controller("Umbraco.Community.Contentment.DataEditors.CheckboxList.Controller", [
    "$scope",
    function ($scope) {

        // console.log("checkboxlist.model", $scope.model);

        var defaultConfig = { items: [], checkAll: 0, showDescriptions: 1, defaultValue: [] };
        var config = angular.extend({}, defaultConfig, $scope.model.config);

        var vm = this;

        function init() {
            $scope.model.value = $scope.model.value || config.defaultValue;

            if (_.isArray($scope.model.value) === false) {
                $scope.model.value = [$scope.model.value];
            }

            vm.items = angular.copy(config.items);

            _.each(vm.items, function (item) {
                item.checked = _.contains($scope.model.value, item.value);
            });

            vm.showDescriptions = Object.toBoolean(config.showDescriptions);

            vm.changed = changed;

            vm.toggleAll = Object.toBoolean(config.checkAll);

            if (vm.toggleAll) {
                vm.toggle = toggle;
                vm.toggleChecked = _.every(vm.items, function (item) {
                    return item.checked;
                });
            }
        };

        function changed(item) {

            vm.toggleChecked = _.every(vm.items, function (item) {
                return item.checked;
            });

            $scope.model.value = [];

            _.each(vm.items, function (x) {
                if (x.checked) {
                    $scope.model.value.push(x.value);
                }
            });

            setDirty();
        };

        function toggle() {
            $scope.model.value = [];

            _.each(vm.items, function (item) {

                item.checked = vm.toggleChecked;

                if (item.checked) {
                    $scope.model.value.push(item.value);
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
