/* Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

angular.module("umbraco").controller("Umbraco.Community.Contentment.DataEditors.ContentBlocks.Controller", [
    "$scope",
    "$q",
    "$http",
    "$interpolate",
    "clipboardService",
    "contentResource",
    "editorService",
    "editorState",
    "localizationService",
    "notificationsService",
    "overlayService",
    "umbRequestHelper",
    "Umbraco.Community.Contentment.Services.DevMode",
    function ($scope, $q, $http, $interpolate, clipboardService, contentResource, editorService, editorState, localizationService, notificationsService, overlayService, umbRequestHelper, devModeService) {

        // console.log("content-blocks.model", $scope.model);

        var defaultConfig = {
            allowCopy: 1,
            allowCreateContentTemplate: 0,
            allowEdit: 1,
            allowRemove: 1,
            disableSorting: 0,
            contentBlockTypes: [],
            enableFilter: 0,
            enablePreview: 0,
            maxItems: 0,
            overlayView: "",
            enableDevMode: 0,
        };
        var config = Object.assign({}, defaultConfig, $scope.model.config);

        var vm = this;

        function init() {

            // NOTE: [LK] Some of the editors may need the context of the current page.
            // If the page is new, then it doesn't have an id, so the parentId will be used.
            var currentNode = editorState.getCurrent();
            config.currentPageId = currentNode.id > 0 ? currentNode.id : currentNode.parentId;

            $scope.model.value = $scope.model.value || [];

            if ($scope.model.value === "") {
                $scope.model.value = [];
            }

            if (Array.isArray($scope.model.value) === false) {
                $scope.model.value = [$scope.model.value];
            }

            config.elementTypeScaffoldCache = {}; // because, reasons! ¯\_(ツ)_/¯
            config.elementTypeLookup = {};
            config.nameTemplates = {};

            config.contentBlockTypes.forEach(function (blockType) {
                config.elementTypeLookup[blockType.key] = blockType;
                config.nameTemplates[blockType.key] = $interpolate(blockType.nameTemplate || "Item {{ $index }}");
            });

            vm.enablePreview = Object.toBoolean(config.enablePreview);

            vm.allowAdd = (config.maxItems === 0 || config.maxItems === "0") || $scope.model.value.length < config.maxItems;
            vm.allowCopy = Object.toBoolean(config.allowCopy) && clipboardService.isSupported();
            vm.allowEdit = Object.toBoolean(config.allowEdit);
            vm.allowRemove = Object.toBoolean(config.allowRemove);
            vm.allowSort = Object.toBoolean(config.disableSorting) === false && (config.maxItems !== 1 && config.maxItems !== "1");

            vm.add = add;
            vm.copy = copy;
            vm.edit = edit;
            vm.remove = remove;
            vm.populateName = populateName;
            vm.sort = function () {
                populatePreviews();
            };

            vm.previews = [];
            vm.blockActions = [];

            for (var i = 0; i < $scope.model.value.length; i++) {
                vm.blockActions.push(actionsFactory(i));
            }

            populatePreviews();

            vm.propertyActions = [];

            if (vm.allowCopy === true) {
                vm.propertyActions.push({
                    labelKey: "contentment_copyAllBlocks",
                    icon: "documents",
                    method: function () {
                        for (var i = 0; i < $scope.model.value.length; i++) {
                            copy(i);
                        }
                    }
                });
            }

            if (Object.toBoolean(config.enableDevMode) === true) {
                vm.propertyActions.push({
                    labelKey: "contentment_editRawValue",
                    icon: "brackets",
                    method: function () {
                        devModeService.editValue($scope.model, function () {
                            // TODO: [LK:2020-01-02] Ensure that the edits are valid! e.g. check min/max items, elementType GUIDs, etc.
                        });
                    }
                });
            }

        };

        function actionsFactory($index) {
            var actions = [];

            if (vm.allowCopy === true) {
                actions.push({
                    labelKey: "contentment_copyContentBlock",
                    icon: "documents",
                    method: function () {
                        copy($index);
                    }
                });
            }

            if (Object.toBoolean(config.allowCreateContentTemplate) === true) {
                actions.push({
                    labelKey: "contentment_createContentTemplate",
                    icon: "blueprint",
                    method: function () {
                        saveBlueprint($index);
                    }
                });
            }

            return actions;
        };

        function add() {
            editorService.open({
                config: {
                    elementTypes: config.contentBlockTypes,
                    enableFilter: config.enableFilter,
                    currentPageId: config.currentPageId,
                },
                size: config.contentBlockTypes.length === 1 ? config.contentBlockTypes[0].overlaySize : "small",
                value: null,
                view: config.overlayView,
                submit: function (model) {

                    $scope.model.value.push(model);

                    var idx = $scope.model.value.length - 1;

                    vm.blockActions.push(actionsFactory(idx));

                    preview(idx);

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

            var item = $scope.model.value[$index];
            var elementType = config.elementTypeLookup[item.elementType];
            var name = populateName(item, $index);

            // if it's in the cache, use it, otherwise make the request
            var getScaffold = config.elementTypeScaffoldCache.hasOwnProperty(elementType.alias) === false
                ? contentResource.getScaffold(config.currentPageId, elementType.alias)
                : $q.when(config.elementTypeScaffoldCache[elementType.alias]);

            // NOTE: Let's bloat up the value to be copied (NC needs it all) ¯\_(ツ)_/¯
            getScaffold.then(function (scaffold) {

                // add to the cache if it isn't already in there
                if (config.elementTypeScaffoldCache.hasOwnProperty(elementType.alias) === false) {
                    config.elementTypeScaffoldCache[elementType.alias] = scaffold;
                }

                scaffold.name = name;
                scaffold.variants[0].name = name;

                if (item.value) {
                    for (var t = 0; t < scaffold.variants[0].tabs.length; t++) {
                        var tab = scaffold.variants[0].tabs[t];
                        for (var p = 0; p < tab.properties.length; p++) {
                            var property = tab.properties[p];
                            if (item.value.hasOwnProperty(property.alias)) {
                                // NOTE: Gah, NC adds `propertyAlias` to the object! ¯\_(ツ)_/¯
                                property.propertyAlias = property.alias;
                                property.value = item.value[property.alias];
                            }
                        }
                    }
                }

                clipboardService.copy("elementType", elementType.alias, scaffold);

            });
        };

        // TODO: [UP-FOR-GRABS] Add a Copy All feature.

        function edit($index) {

            var item = $scope.model.value[$index];
            var elementType = config.elementTypeLookup[item.elementType];

            editorService.open({
                config: {
                    elementType: elementType,
                    currentPageId: config.currentPageId,
                },
                size: elementType.overlaySize,
                value: item,
                view: config.overlayView,
                submit: function (model) {

                    $scope.model.value[$index] = model;

                    preview($index);

                    setDirty();

                    editorService.close();
                },
                close: function () {
                    editorService.close();
                }
            });
        };

        function preview($index) {
            if (vm.enablePreview === true) {

                var item = $scope.model.value[$index];

                if (config.elementTypeLookup[item.elementType].previewEnabled) {

                    vm.previews[item.key] = { loading: true };

                    umbRequestHelper.resourcePromise(
                        $http.post(
                            "backoffice/Contentment/ContentBlocksApi/GetPreviewMarkup",
                            item,
                            { params: { elementIndex: $index, elementKey: item.key, contentId: config.currentPageId } }
                        ),
                        "Failed to retrieve preview markup")
                        .then(function (result) {
                            if (result && result.elementKey && result.markup) {
                                vm.previews[result.elementKey] = {
                                    loading: false,
                                    markup: result.markup
                                };
                            }
                        });
                }
            }
        };

        function populatePreviews() {
            if (vm.enablePreview === true) {
                for (var i = 0; i < $scope.model.value.length; i++) {
                    preview(i);
                }
            }
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

                        vm.blockActions.pop();

                        populatePreviews();

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
                                properties: Object.entries(item.value).map(x => ({ id: 0, alias: x[0], value: x[1] }))
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
