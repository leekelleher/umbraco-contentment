/* Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

angular.module("umbraco").controller("Umbraco.Community.Contentment.DataEditors.ConfigurationEditor.Controller", [
    "$scope",
    "$interpolate",
    "$timeout",
    "editorService",
    "eventsService",
    "localizationService",
    "overlayService",
    "Umbraco.Community.Contentment.Services.DevMode",
    function ($scope, $interpolate, $timeout, editorService, eventsService, localizationService, overlayService, devModeService) {

        // console.log("config-editor.model", $scope.model);

        var defaultConfig = {
            addButtonLabelKey: "general_add",
            allowDuplicates: 0,
            allowRemove: 1,
            disableSorting: 0,
            displayMode: "list",
            enableDevMode: 0,
            enableFilter: 0,
            help: null,
            items: [],
            maxItems: 0,
            orderBy: "name",
            overlayView: "",
        };
        var config = Object.assign({}, defaultConfig, $scope.model.config);

        var vm = this;

        function init() {

            $scope.model.value = $scope.model.value || [];

            if (Array.isArray($scope.model.value) === false) {
                $scope.model.value = [$scope.model.value];
            }

            // NOTE: Patches a breaking-change. I'd renamed `type` to become `key`. [LK:2020-04-03]
            $scope.model.value.forEach(item => {
                if (item.hasOwnProperty("type")) {
                    item.key = item.type;
                    delete item.type;
                }
            });

            if (Number.isInteger(config.maxItems) === false) {
                config.maxItems = Number.parseInt(config.maxItems) || defaultConfig.maxItems;
            }

            config.itemLookup = {};
            config.allowEdit = {};
            config.expressions = {};
            config.missingItem = {};

            config.items.forEach(item => {

                if (config.itemLookup.hasOwnProperty(item.key) === false) {
                    config.itemLookup[item.key] = item;
                }

                if (config.allowEdit.hasOwnProperty(item.key) === false) {
                    config.allowEdit[item.key] = item.fields && item.fields.length > 0;
                }

                if (config.expressions.hasOwnProperty(item.key) === false) {

                    config.expressions[item.key] = {};

                    if (item.nameTemplate) { // TODO: [LK:2022-07-05] Deprecated.
                        config.expressions[item.key]["name"] = $interpolate(item.nameTemplate);
                    }

                    if (item.descriptionTemplate) { // TODO: [LK:2022-07-05] Deprecated.
                        config.expressions[item.key]["description"] = $interpolate(item.descriptionTemplate);
                    }

                    if (item.expressions) {
                        for (let [alias, value] of Object.entries(item.expressions)) {
                            if (value) {
                                config.expressions[item.key][alias] = $interpolate(value);
                            }
                        }
                    }
                }
            });

            localizationService.localizeMany(["contentment_missingItemName", "contentment_missingItemDescription"]).then(data => {
                config.missingItem["name"] = data[0];
                config.missingItem["description"] = data[1];
                config.missingItem["icon"] = "icon-alert";
            });

            vm.allowAdd = config.maxItems === 0 || $scope.model.value.length < config.maxItems;
            vm.allowEdit = (item, $index) => config.allowEdit[item.key];
            vm.allowRemove = Object.toBoolean(config.allowRemove);
            vm.allowSort = Object.toBoolean(config.disableSorting) === false && config.maxItems !== 1;

            vm.displayMode = config.displayMode;

            vm.sortableOptions = {
                axis: "y",
                containment: "parent",
                cursor: "move",
                disabled: vm.allowSort === false,
                opacity: 0.7,
                scroll: true,
                tolerance: "pointer",
                stop: (e, ui) => {
                    setDirty();
                }
            };

            vm.addButtonLabelKey = config.addButtonLabelKey || "general_add";

            vm.add = add;
            vm.edit = edit;
            vm.populate = populate;
            vm.remove = remove;

            vm.propertyActions = [];

            if (Object.toBoolean(config.enableDevMode)) {
                vm.propertyActions.push({
                    labelKey: "contentment_editRawValue",
                    icon: "brackets",
                    method: () => devModeService.editValue($scope.model, validate)
                });
            }

            // NOTE: Must wait, otherwise the preview may not be ready to receive the event.
            $timeout(() => emit(), 100);
        };

        function add() {

            var items = Object.toBoolean(config.allowDuplicates)
                ? config.items
                : config.items.filter(x => $scope.model.value.some(y => x.key === y.key) === false);

            editorService.open({
                view: config.overlayView,
                size: config.items.length === 1 ? config.items[0].overlaySize : "small",
                config: {
                    mode: "select",
                    autoSelect: config.items.length === 1,
                    label: $scope.model.label,
                    items: items,
                    enableFilter: Object.toBoolean(config.enableFilter),
                    orderBy: config.orderBy,
                    help: config.help,
                    cacheKey: $scope.model.alias,
                },
                value: {},
                submit: function (model) {

                    $scope.model.value.push(model);

                    if (config.maxItems !== 0 && $scope.model.value.length >= config.maxItems) {
                        vm.allowAdd = false;
                    }

                    setDirty();

                    editorService.close();
                },
                close: function () {
                    editorService.close();
                }
            });
        };

        function edit($index) {

            var value = $scope.model.value[$index];
            var editor = config.itemLookup[value.key];

            editorService.open({
                view: config.overlayView,
                size: editor.overlaySize,
                config: {
                    mode: "edit",
                    editor: editor,
                    cacheKey: $scope.model.alias,
                },
                value: value,
                submit: function (model) {

                    $scope.model.value[$index] = model;

                    setDirty();

                    editorService.close();
                },
                close: function () {
                    editorService.close();
                }
            });
        };

        function emit() {
            eventsService.emit("contentment.config-editor.model", $scope.model);
        };

        function populate(item, $index, propertyName) {
            var label = "";

            // check that the configuration editor exists, if not then return a default label.
            if (config.itemLookup.hasOwnProperty(item.key) === false && config.missingItem) {
                return config.missingItem[propertyName] || propertyName;
            }

            if (config.expressions.hasOwnProperty(item.key) === true) {
                var expressions = config.expressions[item.key];
                if (expressions.hasOwnProperty(propertyName) === true) {
                    var expression = expressions[propertyName];
                    if (expression) {
                        item.value.$index = $index + 1;
                        label = expression(item.value);
                        delete item.value.$index;
                    }
                }
            }

            return label || config.itemLookup[item.key][propertyName];
        };

        function remove($index) {
            var keys = ["contentment_removeItemMessage", "general_remove", "general_cancel", "contentment_removeItemButton"];
            localizationService.localizeMany(keys).then(function (data) {
                overlayService.open({
                    title: data[1],
                    content: data[0],
                    closeButtonLabel: data[2],
                    submitButtonLabel: data[3],
                    submitButtonStyle: "danger",
                    submit: function () {

                        $scope.model.value.splice($index, 1);

                        if (config.maxItems === 0 || $scope.model.value.length < config.maxItems) {
                            vm.allowAdd = true;
                        }

                        setDirty();

                        overlayService.close();
                    },
                    close: function () {
                        overlayService.close();
                    }
                });
            });
        };

        function validate() {
            // TODO: [LK:2019-06-30] Need to remove any extra items.
            if (config.maxItems !== 0 && $scope.model.value.length >= config.maxItems) {
                vm.allowAdd = false;
            } else {
                vm.allowAdd = true;
            }
            setDirty();
        };

        function setDirty() {
            if ($scope.propertyForm) {
                $scope.propertyForm.$setDirty();
            }
            emit();
        };

        init();
    }
]);
