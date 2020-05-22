/* Copyright © 2020 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

angular.module("umbraco").controller("Umbraco.Community.Contentment.DataEditors.TemplatedList.Controller", [
    "$scope",
    function ($scope) {

        //console.log("templated-list.model", $scope.model);

        var defaultConfig = {
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

            vm.items.forEach(function (item) {
                item.selected = $scope.model.value.indexOf(item.value) > -1;
            });

            vm.htmlAttributes = config.htmlAttributes;

            vm.template = config.template;

            vm.uniqueId = $scope.model.hasOwnProperty("dataTypeKey")
                ? [$scope.model.alias, $scope.model.dataTypeKey.substring(0, 8)].join("-")
                : $scope.model.alias;

            vm.select = select;
        };

        function select(item) {

            //console.log("select", item);

            item.selected = item.selected === false;
            $scope.model.value = [];

            vm.items.forEach(function (x) {

                if (vm.multiple === false) {
                    x.selected = x.value === item.value;
                }

                if (x.selected) {
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
