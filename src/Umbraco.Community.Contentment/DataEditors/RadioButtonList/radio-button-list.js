/* Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

angular.module("umbraco").controller("Umbraco.Community.Contentment.DataEditors.RadioButtonList.Controller", [
    "$scope",
    "eventsService",
    function ($scope, eventsService) {

        // console.log("radiobuttonlist.model", $scope.model);

        var defaultConfig = {
            allowClear: 0,
            items: [],
            showDescriptions: 0,
            showIcons: 0,
            defaultValue: ""
        };
        var config = Object.assign({}, defaultConfig, $scope.model.config);

        var vm = this;

        function init() {
            $scope.model.value = $scope.model.value || config.defaultValue;

            if (Array.isArray($scope.model.value)) {
                $scope.model.value = $scope.model.value[0];
            }

            vm.items = config.items.slice();

            vm.showDescriptions = Object.toBoolean(config.showDescriptions);
            vm.showIcons = Object.toBoolean(config.showIcons);

            vm.uniqueId = $scope.model.hasOwnProperty("dataTypeKey")
                ? [$scope.model.alias, $scope.model.dataTypeKey.substring(0, 8)].join("-")
                : $scope.model.alias;

            if ($scope.umbProperty) {

                vm.propertyActions = [];

                if (Object.toBoolean(config.allowClear) === true) {
                    vm.propertyActions.push({
                        labelKey: "buttons_clearSelection",
                        icon: "trash",
                        method: () => {
                            $scope.model.value = config.defaultValue;
                            setDirty();
                        }
                    });
                }

                if (vm.propertyActions.length > 0) {
                    $scope.umbProperty.setPropertyActions(vm.propertyActions);
                }
            }

            var events = [];

            events.push(eventsService.on("contentment.update.value", (event, args) => {
                if (args.alias === $scope.model.alias) {
                    $scope.model.value = args.value;
                    init();
                }
            }));

            $scope.$on("$destroy", () => {
                for (var event in events) {
                    eventsService.unsubscribe(events[event]);
                }
            });
        };

        init();
    }
]);
