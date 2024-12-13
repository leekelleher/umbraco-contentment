/* Copyright © 2020 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

angular.module("umbraco").controller("Umbraco.Community.Contentment.DataEditors.Buttons.Controller", [
    "$scope",
    "eventsService",
    function ($scope, eventsService) {

        // console.log("buttons.model", $scope.model);

        var defaultConfig = {
            allowClear: 0,
            defaultIcon: "icon-science",
            defaultValue: [],
            items: [],
            enableMultiple: 0,
            labelStyle: "both",
            size: "m",
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

            vm.hideIcon = config.labelStyle === "text";
            vm.hideName = config.labelStyle === "icon";

            vm.uniqueId = $scope.model.hasOwnProperty("dataTypeKey")
                ? [$scope.model.alias, $scope.model.dataTypeKey.substring(0, 8)].join("-")
                : $scope.model.alias;

            var sizes = {
                "s": "small",
                "m": "medium",
                "l": "large",
            };

            vm.size = config.size;

            vm.defaultIcon = config.defaultIcon;
            vm.iconExtras = (vm.hideName === false ? " mr2 " : " mr0 ") + sizes[config.size];

            vm.items = [];

            config.items.forEach(x => {
                var item = Object.assign({}, x);
                item.selected = $scope.model.value.indexOf(item.value) > -1;
                vm.items.push(item);
            });

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

        function clear() {
            $scope.model.value = [];
            vm.items.forEach(item => item.selected = false);
            setDirty();
        };

        function select(item) {

            item.selected = item.selected === false;

            $scope.model.value = [];

            vm.items.forEach(x => {

                if (vm.multiple === false) {
                    x.selected = item.selected && x.value === item.value;
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
