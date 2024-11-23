/* Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

angular.module("umbraco").controller("Umbraco.Community.Contentment.DataEditors.ItemPicker.Controller", [
    "$scope",
    "editorService",
    "eventsService",
    "focusService",
    "localizationService",
    "overlayService",
    function ($scope, editorService, eventsService, focusService, localizationService, overlayService) {

        // console.log("item-picker.model", $scope.model);

        var defaultConfig = {
            addButtonLabelKey: "general_add",
            allowClear: 0,
            allowDuplicates: 0,
            confirmRemoval: 0,
            defaultIcon: "icon-science",
            defaultValue: [],
            disableSorting: 0,
            displayMode: "list",
            enableFilter: 1,
            enableMultiple: 0,
            items: [],
            maxItems: 0,
            listType: "grid",
            overlayView: "",
            overlayOrderBy: "name",
            overlaySize: "small",
        };
        var config = Object.assign({}, defaultConfig, $scope.model.config);

        var vm = this;

        function init() {
            $scope.model.value = $scope.model.value || config.defaultValue;

            if (Array.isArray($scope.model.value) === false) {
                $scope.model.value = [$scope.model.value];
            }

            if (Number.isInteger(config.maxItems) === false) {
                config.maxItems = Number.parseInt(config.maxItems) || defaultConfig.maxItems;
            }

            if (Array.isArray(config.overlaySize) === true) {
                config.overlaySize = config.overlaySize[0];
            }

            config.confirmRemoval = Object.toBoolean(config.confirmRemoval);
            config.enableMultiple = Object.toBoolean(config.enableMultiple) && config.maxItems !== 1;

            vm.defaultIcon = config.defaultIcon;
            vm.displayMode = config.displayMode || "list";
            vm.allowAdd = config.maxItems === 0 || $scope.model.value.length < config.maxItems;
            vm.allowEdit = false;
            vm.allowRemove = true;
            vm.allowSort = Object.toBoolean(config.disableSorting) === false && config.maxItems !== 1;

            vm.addButtonLabelKey = config.addButtonLabelKey || "general_add";

            vm.add = add;
            vm.remove = remove;
            vm.sort = () => {
                $scope.model.value = vm.items.map(item => item.value);
            };

            vm.items = [];

            if ($scope.model.value.length > 0 && config.items.length > 0) {
                var orphaned = [];

                $scope.model.value.forEach(v => {
                    var item = config.items.find(x => x.value === v);
                    if (item) {
                        vm.items.push(Object.assign({}, item));
                    } else {
                        orphaned.push(v);
                    }
                });

                if (orphaned.length > 0) {
                    $scope.model.value = _.difference($scope.model.value, orphaned); // TODO: Replace Underscore.js dependency. [LK:2020-03-02]

                    if (config.maxItems === 0 || $scope.model.value.length < config.maxItems) {
                        vm.allowAdd = true;
                    }
                }
            }

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

        function add() {
            focusService.rememberFocus();

            var items = Object.toBoolean(config.allowDuplicates)
                ? config.items
                : config.items.filter(x => vm.items.some(y => x.value === y.value) === false);

            editorService.open({
                config: {
                    title: "Choose...",
                    enableFilter: Object.toBoolean(config.enableFilter),
                    enableMultiple: config.enableMultiple,
                    defaultIcon: config.defaultIcon,
                    items: items,
                    listType: config.listType,
                    orderBy: config.overlayOrderBy,
                    maxItems: config.maxItems === 0 ? config.maxItems : config.maxItems - vm.items.length
                },
                view: config.overlayView,
                size: config.overlaySize || "small",
                submit: function (selectedItems) {

                    selectedItems.forEach(item => {
                        vm.items.push(angular.copy(item)); // TODO: Replace AngularJS dependency. [LK:2020-12-17]
                        $scope.model.value.push(item.value);
                    });

                    if (config.maxItems !== 0 && $scope.model.value.length >= config.maxItems) {
                        vm.allowAdd = false;
                    }

                    editorService.close();

                    setDirty();
                    setFocus();
                },
                close: function () {
                    editorService.close();
                    setFocus();
                }
            });
        };

        function clear() {
            vm.items = [];
            $scope.model.value = [];
            setDirty();
        };

        function remove($index) {

            focusService.rememberFocus();

            if (config.confirmRemoval === true) {
                var keys = ["contentment_removeItemMessage", "general_remove", "general_cancel", "contentment_removeItemButton"];
                localizationService.localizeMany(keys).then(data => {
                    overlayService.open({
                        title: data[1],
                        content: data[0],
                        closeButtonLabel: data[2],
                        submitButtonLabel: data[3],
                        submitButtonStyle: "danger",
                        submit: function () {
                            removeItem($index);
                            overlayService.close();
                        },
                        close: function () {
                            overlayService.close();
                            setFocus();
                        }
                    });
                });
            } else {
                removeItem($index);
            }
        };

        function removeItem($index) {

            vm.items.splice($index, 1);

            $scope.model.value.splice($index, 1);

            if (config.maxItems === 0 || $scope.model.value.length < config.maxItems) {
                vm.allowAdd = true;
            }

            setDirty();
        };

        function setDirty() {
            if ($scope.propertyForm) {
                $scope.propertyForm.$setDirty();
            }
        };

        function setFocus() {
            var lastKnownFocus = focusService.getLastKnownFocus();
            if (lastKnownFocus) {
                lastKnownFocus.focus();
            }
        };

        init();
    }
]);
