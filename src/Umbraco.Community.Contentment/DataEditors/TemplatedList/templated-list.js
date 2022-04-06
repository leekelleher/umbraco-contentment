/* Copyright © 2020 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

angular.module("umbraco").controller("Umbraco.Community.Contentment.DataEditors.TemplatedList.Controller", [
    "$scope",
    function ($scope) {

        //console.log("templated-list.model", $scope.model);

        var defaultConfig = {
            allowClear: 0,
            defaultValue: [],
            enableMultiple: 0,
            items: [],
            template: "{{ item.name }}",
            htmlAttributes: []
        };
        var config = Object.assign({}, defaultConfig, $scope.model.config);

        var vm = this;

        function init() {

            $scope.model.value = $scope.model.value || config.defaultValue;

            if (Array.isArray($scope.model.value) === false) {
                $scope.model.value = [$scope.model.value];
            }

            vm.multiple = Object.toBoolean(config.enableMultiple);

            if (vm.multiple === false && $scope.model.value.length > 0) {
                $scope.model.value.splice(1);
            }

            vm.items = config.items.slice();

            vm.items.forEach(item => {
                item.selected = $scope.model.value.indexOf(item.value) > -1;
            });

            vm.htmlAttributes = config.htmlAttributes;

            vm.template = config.template;

            vm.uniqueId = $scope.model.hasOwnProperty("dataTypeKey")
                ? [$scope.model.alias, $scope.model.dataTypeKey.substring(0, 8)].join("-")
                : $scope.model.alias;

            vm.select = select;

            if ($scope.umbProperty) {

                vm.propertyActions = [];

                if (Object.toBoolean(config.allowClear) === true) {
                    vm.propertyActions.push({
                        labelKey: "buttons_clearSelection",
                        icon: "trash",
                        method: clear
                    });
                }

                if (vm.propertyActions.length > 0) {
                    $scope.umbProperty.setPropertyActions(vm.propertyActions);
                }
            }
        };

        function clear() {
            $scope.model.value = [];
            vm.items.forEach(item => item.selected = false);
            setDirty();
        };

        function select(item) {

            item.selected = item.selected === false;

            $scope.model.value = [];

            vm.items.forEach(item => {

                if (vm.multiple === false) {
                    item.selected = item.value === item.value;
                }

                if (item.selected) {
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
