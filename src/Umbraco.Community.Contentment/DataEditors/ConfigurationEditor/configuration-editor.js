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
            allowDuplicates: 0,
            items: [],
            maxItems: 0,
            disableSorting: 0,
            allowRemove: 1,
            enableFilter: 0,
            orderBy: "name",
            overlayView: "",
            enableDevMode: 0,
            addButtonLabelKey: "general_add",
        };
        var config = Object.assign({}, defaultConfig, $scope.model.config);

        var vm = this;

        function init() {

            $scope.model.value = $scope.model.value || [];

            if (Array.isArray($scope.model.value) === false) {
                $scope.model.value = [$scope.model.value];
            }

            // NOTE: Patches a breaking-change. I'd renamed `type` to become `key`. [LK:2020-04-03]
            $scope.model.value.forEach(function (item) {
                if (item.hasOwnProperty("type")) {
                    item.key = item.type;
                    delete item.type;
                }
            });

            config.itemLookup = {};
            config.allowEdit = {};
            config.nameTemplates = {};
            config.descriptionTemplates = {};

            config.items.forEach(function (item) {
                config.itemLookup[item.key] = item;

                config.allowEdit[item.key] = item.fields && item.fields.length > 0;

                if (item.nameTemplate) {
                    config.nameTemplates[item.key] = $interpolate(item.nameTemplate);
                }

                if (item.descriptionTemplate) {
                    config.descriptionTemplates[item.key] = $interpolate(item.descriptionTemplate);
                }
            });

            vm.allowAdd = (config.maxItems === 0 || config.maxItems === "0") || $scope.model.value.length < config.maxItems;
            vm.allowEdit = function (item, $index) { return config.allowEdit[item.key]; };
            vm.allowRemove = Object.toBoolean(config.allowRemove);
            vm.allowSort = Object.toBoolean(config.disableSorting) === false && (config.maxItems !== 1 && config.maxItems !== "1");

            vm.sortableOptions = {
                axis: "y",
                containment: "parent",
                cursor: "move",
                disabled: vm.allowSort === false,
                opacity: 0.7,
                scroll: true,
                tolerance: "pointer",
                stop: function (e, ui) {
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
                    method: function () {
                        devModeService.editValue($scope.model, validate);
                    }
                });
            }

            // NOTE: Must wait, otherwise the preview may not be ready to receive the event.
            $timeout(function () { emit(); }, 100);
        };

        function add() {

            var items = Object.toBoolean(config.allowDuplicates) ? config.items : _.reject(config.items, function (x) { // TODO: Replace Underscore.js dependency. [LK:2020-03-02]
                return _.find($scope.model.value, function (y) { return x.key === y.key; }); // TODO: Replace Underscore.js dependency. [LK:2020-03-02]
            });

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
                },
                value: {},
                submit: function (model) {

                    $scope.model.value.push(model);

                    if ((config.maxItems !== 0 && config.maxItems !== "0") && $scope.model.value.length >= config.maxItems) {
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

            if (propertyName === 'name' && config.nameTemplates.hasOwnProperty(item.key) === true) {
                var expression = config.nameTemplates[item.key];
                if (expression) {
                    item.value.$index = $index + 1;
                    label = expression(item.value);
                    delete item.value.$index;
                }
            }

            if (propertyName === 'description' && config.descriptionTemplates.hasOwnProperty(item.key) === true) {
                var expression = config.descriptionTemplates[item.key];
                if (expression) {
                    item.value.$index = $index + 1;
                    label = expression(item.value);
                    delete item.value.$index;
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

                        if ((config.maxItems === 0 || config.maxItems === "0") || $scope.model.value.length < config.maxItems) {
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
            if ((config.maxItems !== 0 && config.maxItems !== "0") && $scope.model.value.length >= config.maxItems) {
                vm.allowAdd = false;
            } else {
                vm.allowAdd = true;
            }
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
