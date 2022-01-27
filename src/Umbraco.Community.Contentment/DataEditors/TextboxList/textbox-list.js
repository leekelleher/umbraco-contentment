/* Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

angular.module("umbraco").controller("Umbraco.Community.Contentment.DataEditors.TextboxList.Controller", [
    "$scope",
    "Umbraco.Community.Contentment.Services.DevMode",
    function ($scope, devModeService) {

        //console.log("textbox-list.model", $scope.model);

        var defaultConfig = {
            defaultIcon: "icon-science",
            items: [],
            labelStyle: "both",
            enableDevMode: 0,
        };
        var config = Object.assign({}, defaultConfig, $scope.model.config);

        var vm = this;

        function init() {

            $scope.model.value = $scope.model.value || {};

            vm.keys = [];
            vm.icons = {};
            vm.names = {};

            vm.hideIcon = config.labelStyle === "text";
            vm.hideName = config.labelStyle === "icon";

            vm.defaultIcon = config.defaultIcon;

            config.items.forEach(function (item) {

                vm.keys.push(item.value);
                vm.names[item.value] = item.name;

                if (vm.hideIcon === false) {
                    vm.icons[item.value] = (item.icon || config.defaultIcon) + " medium";
                }

                if ($scope.model.value.hasOwnProperty(item.value) === false) {
                    $scope.model.value[item.value] = "";
                }

            });

            // Handle orphaned values
            Object.keys($scope.model.value).forEach(v => {
                if (vm.keys.indexOf(v) === -1) {
                    delete $scope.model.value[v];
                }
            });

            vm.uniqueId = $scope.model.hasOwnProperty("dataTypeKey")
                ? [$scope.model.alias, $scope.model.dataTypeKey.substring(0, 8)].join("-")
                : $scope.model.alias;

            if ($scope.umbProperty) {

                vm.propertyActions = [];

                if (Object.toBoolean(config.enableDevMode) === true) {
                    vm.propertyActions.push({
                        labelKey: "contentment_editRawValue",
                        icon: "brackets",
                        method: function () {
                            devModeService.editValue($scope.model, function () { });
                        }
                    });
                }

                if (vm.propertyActions.length > 0) {
                    $scope.umbProperty.setPropertyActions(vm.propertyActions);
                }
            }
        };

        init();
    }
]);
