/* Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

angular.module("umbraco").controller("Umbraco.Community.Contentment.DataEditors.ContentBlocks.Controller", [
    "$interpolate",
    "$scope",
    "clipboardService",
    "contentResource",
    "editorService",
    "localizationService",
    "notificationsService",
    "overlayService",
    "Umbraco.Community.Contentment.Services.DevMode",
    function ($interpolate, $scope, clipboardService, contentResource, editorService, localizationService, notificationsService, overlayService, devModeService) {

        // console.log("content-blocks.model", $scope.model);

        var defaultConfig = {
            allowCopy: 1,
            allowEdit: 1,
            allowRemove: 1,
            disableSorting: 0,
            contentBlockTypes: [],
            enableFilter: 0,
            maxItems: 0,
            overlayView: "",
            enableDevMode: 0,
        };
        var config = Object.assign({}, defaultConfig, $scope.model.config);

        var vm = this;

        function init() {

            $scope.model.value = $scope.model.value || [];

            if ($scope.model.value === "") {
                $scope.model.value = [];
            }

            if (Array.isArray($scope.model.value) === false) {
                $scope.model.value = [$scope.model.value];
            }

            config.elementTypeLookup = {};
            config.nameTemplates = {};

            config.contentBlockTypes.forEach(function (blockType) {
                config.elementTypeLookup[blockType.key] = blockType;
                config.nameTemplates[blockType.key] = $interpolate(blockType.nameTemplate || "Item {{ $index }}");
            });

            vm.allowAdd = (config.maxItems === 0 || config.maxItems === "0") || $scope.model.value.length < config.maxItems;
            vm.allowCopy = Object.toBoolean(config.allowCopy) && clipboardService.isSupported();
            vm.allowEdit = Object.toBoolean(config.allowEdit);
            vm.allowRemove = Object.toBoolean(config.allowRemove);
            vm.sortable = Object.toBoolean(config.disableSorting) === false && (config.maxItems !== 1 && config.maxItems !== "1");

            vm.sortableOptions = {
                axis: "y",
                containment: "parent",
                cursor: "move",
                disabled: vm.sortable === false,
                opacity: 0.7,
                scroll: true,
                tolerance: "pointer",
                stop: function (e, ui) {
                    setDirty();
                }
            };

            vm.add = add;
            vm.copy = copy;
            vm.edit = edit;
            vm.remove = remove;
            vm.populateName = populateName;

            vm.blockActions = [];

            for (var i = 0; i < $scope.model.value.length; i++) {
                vm.blockActions.push(actionsFactory(i));
            }

            if ($scope.umbProperty) {

                var propertyActions = [];

                if (vm.allowCopy) {
                    propertyActions.push({
                        labelKey: "contentment_copyAllBlocks",
                        icon: "documents",
                        method: function () {
                            for (var i = 0; i < $scope.model.value.length; i++) {
                                copy(i);
                            }
                        }
                    });
                }

                if (Object.toBoolean(config.enableDevMode)) {
                    propertyActions.push({
                        labelKey: "contentment_editRawValue",
                        icon: "brackets",
                        method: function () {
                            devModeService.editValue($scope.model, function () {
                                // TODO: [LK:2020-01-02] Ensure that the edits are valid! e.g. check min/max items, elementType GUIDs, etc.
                            });
                        }
                    });
                }

                if (propertyActions.length > 0) {
                    $scope.umbProperty.setPropertyActions(propertyActions);
                }
            }
        };

        function actionsFactory($index) {
            var actions = [];

            if (vm.allowCopy) {
                actions.push({
                    labelKey: "contentment_copyContentBlock",
                    icon: "documents",
                    method: function () {
                        copy($index);
                    }
                });
            }

            actions.push({
                labelKey: "contentment_createContentTemplate",
                icon: "blueprint",
                method: function () {
                    saveBlueprint($index);
                }
            });

            return actions;
        };

        function add() {
            editorService.open({
                config: {
                    elementTypes: config.contentBlockTypes,
                    enableFilter: config.enableFilter
                },
                size: config.contentBlockTypes.length === 1 ? config.contentBlockTypes[0].overlaySize : "small",
                value: null,
                view: config.overlayView,
                submit: function (model) {

                    $scope.model.value.push(model);

                    vm.blockActions.push(actionsFactory($scope.model.value.length - 1));

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

        function copy($index) {

            var tmp = $scope.model.value[$index];
            var item = Object.assign({}, tmp, { name: populateName(tmp, $index) });

            clipboardService.copy("contentment.element", item.elementType, item);
        };

        function edit($index) {

            var item = $scope.model.value[$index];
            var elementType = config.elementTypeLookup[item.elementType];

            editorService.open({
                config: {
                    elementType: elementType
                },
                size: elementType.overlaySize,
                value: item,
                view: config.overlayView,
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

        function populateName(item, $index) {

            var name = "";

            var expression = config.nameTemplates[item.elementType];

            if (expression) {

                item.value.$index = $index + 1;
                name = expression(item.value);
                delete item.value.$index;

            } else {

                name = config.elementTypeLookup[item.elementType].name;

            }

            return name;
        };

        function remove($index) {
            var keys = ["content_nestedContentDeleteItem", "general_delete", "general_cancel", "contentTypeEditor_yesDelete"];
            localizationService.localizeMany(keys).then(function (data) {
                overlayService.open({
                    title: data[1],
                    content: data[0],
                    closeButtonLabel: data[2],
                    submitButtonLabel: data[3],
                    submitButtonStyle: "danger",
                    submit: function () {

                        $scope.model.value.splice($index, 1);
                        vm.blockActions.pop();

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

        function saveBlueprint($index) {
            var keys = [
                "blueprints_createBlueprintFrom",
                "blueprints_blueprintDescription",
                "blueprints_createdBlueprintHeading",
                "blueprints_createdBlueprintMessage",
                "general_cancel",
                "general_create"
            ];

            localizationService.localizeMany(keys).then(function (labels) {

                var item = $scope.model.value[$index];
                var itemName = populateName(item, $index);
                var elementType = config.elementTypeLookup[item.elementType];

                overlayService.open({
                    disableBackdropClick: true,
                    title: localizationService.tokenReplace(labels[0], [itemName]),
                    description: labels[1],
                    blueprintName: itemName,
                    view: "/App_Plugins/Contentment/editors/content-blocks.blueprint.html",
                    closeButtonLabel: labels[4],
                    submitButtonLabel: labels[5],
                    submitButtonStyle: "action",
                    submit: function (model) {

                        delete model.error;
                        model.submitButtonState = "busy";

                        var variant = {
                            save: true,
                            name: model.blueprintName,
                            tabs: [{
                                properties: _.map(_.pairs(item.value), function (x) { // TODO: Replace Underscore.js dependency. [LK:2020-03-02]
                                    return { id: 0, alias: x[0], value: x[1] };
                                })
                            }]
                        };

                        var content = {
                            action: "saveNew",
                            id: 0,
                            parentId: -1,
                            contentTypeAlias: elementType.alias,
                            expireDate: null,
                            releaseDate: null,
                            templateAlias: null,
                            variants: [Object.assign({}, variant, { save: true })]
                        };

                        contentResource
                            .saveBlueprint(content, true, [], false)
                            .then(function (data) {

                                model.submitButtonState = "success";

                                notificationsService.success(labels[2], localizationService.tokenReplace(labels[3], [itemName]));

                                elementType.blueprints.push({ id: data.id, name: data.variants[0].name });

                                overlayService.close();

                            }, function (error) {

                                model.submitButtonState = "error";
                                model.error = error.data.ModelState.Name[0];

                            });
                    },
                    close: function () {
                        overlayService.close();
                    }
                });
            });
        };

        function setDirty() {
            if ($scope.propertyForm) {
                $scope.propertyForm.$setDirty();
            }
        };

        init();
    }
]);
