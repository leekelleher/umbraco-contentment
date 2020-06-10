/* Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

angular.module("umbraco").controller("Umbraco.Community.Contentment.Overlays.ContentBlocks.Controller", [
    "$scope",
    "blueprintConfig",
    "clipboardService",
    "contentResource",
    function ($scope, blueprintConfig, clipboardService, contentResource) {

        // console.log("content-blocks-overlay.model", $scope.model, blueprintConfig);

        var defaultConfig = {
            elementType: null,
            elementTypes: [],
            enableFilter: true,
            currentPageId: -2,
        };
        var config = Object.assign({}, defaultConfig, $scope.model.config);

        var vm = this;

        function init() {

            vm.submit = submit;
            vm.close = close;

            if (config.elementType && $scope.model.value) {

                edit(config.elementType, $scope.model.value);

            } else {

                vm.mode = "select";
                vm.items = config.elementTypes;
                vm.selectedElementType = null;

                vm.clipboardItems = clipboardService.retriveDataOfType("contentment.element", config.elementTypes.map(function (x) { return x.key }));

                if (config.elementTypes.length > 1 || vm.clipboardItems.length > 0) {

                    vm.title = "Add content";
                    vm.description = "Select a content type...";
                    vm.selectBlueprint = false;
                    vm.enableFilter = Object.toBoolean(config.enableFilter);

                    vm.select = select;
                    vm.paste = paste;

                    vm.clearClipboard = clearClipboard;
                    vm.prompt = false;
                    vm.showPrompt = showPrompt;
                    vm.hidePrompt = hidePrompt;

                } else if (config.elementTypes.length === 1) {

                    select(config.elementTypes[0]);

                }
            }
        };

        function clearClipboard() {
            vm.clipboardItems = [];
            clipboardService.clearEntriesOfType("contentment.element", config.elementTypes.map(function (x) { return x.key }));
        };

        function showPrompt() {
            vm.prompt = true;
        };

        function hidePrompt() {
            vm.prompt = false;
        };

        function select(elementType) {
            if (elementType.blueprints && elementType.blueprints.length > 0) {
                if (elementType.blueprints.length === 1 && blueprintConfig.skipSelect) {
                    create(elementType, elementType.blueprints[0]);
                }
                else {
                    vm.title = "Add content";
                    vm.description = "Select a content blueprint...";
                    vm.selectBlueprint = true;
                    vm.selectedElementType = elementType;
                    vm.blueprintAllowBlank = blueprintConfig.allowBlank;
                    vm.create = create;
                }
            } else {
                create(elementType);
            }
        };

        function create(elementType, blueprint) {

            $scope.model.size = elementType.overlaySize;

            vm.mode = "edit";
            vm.title = "Edit content";
            vm.description = elementType.name;
            vm.loading = true;

            vm.content = {
                elementType: elementType.key,
                icon: elementType.icon,
                key: String.CreateGuid()
            };

            var getScaffold = blueprint && blueprint.id > 0
                ? contentResource.getBlueprintScaffold(config.currentPageId, blueprint.id)
                : contentResource.getScaffold(config.currentPageId, elementType.alias);

            getScaffold.then(function (data) {
                Object.assign(vm.content, data.variants[0]);
                vm.loading = false;
            });

        };

        function paste(element) {

            var elementType = _.find(config.elementTypes, function (x) { // TODO: Replace Underscore.js dependency. [LK:2020-03-02]
                return x.key === element.elementType;
            });

            $scope.model.size = elementType.overlaySize;

            element.key = String.CreateGuid();

            edit(elementType, element);
        };

        function edit(elementType, element) {

            vm.mode = "edit";
            vm.title = "Edit content";
            vm.description = elementType.name;
            vm.loading = true;

            vm.content = {
                elementType: elementType.key,
                icon: elementType.icon,
                key: element.key
            };

            contentResource.getScaffold(config.currentPageId, elementType.alias).then(function (data) {

                if (element.value) {
                    for (var t = 0; t < data.variants[0].tabs.length; t++) {
                        var tab = data.variants[0].tabs[t];
                        for (var p = 0; p < tab.properties.length; p++) {
                            var property = tab.properties[p];
                            if (element.value.hasOwnProperty(property.alias)) {
                                property.value = element.value[property.alias];
                            }
                        }
                    }
                }

                Object.assign(vm.content, data.variants[0]);
                vm.loading = false;
            });

        };

        function submit() {

            if ($scope.model.submit) {

                $scope.$broadcast("formSubmitting", { scope: $scope });

                var item = {
                    elementType: vm.content.elementType,
                    key: vm.content.key,
                    icon: vm.content.icon,
                    value: {},
                };

                if (vm.content.tabs.length > 0) {
                    for (var t = 0; t < vm.content.tabs.length; t++) {
                        var tab = vm.content.tabs[t];
                        for (var p = 0; p < tab.properties.length; p++) {
                            var property = tab.properties[p];
                            if (typeof property.value !== "function") {
                                item.value[property.alias] = property.value;
                            }
                        }
                    }
                }

                $scope.model.submit(item);
            }
        };

        function close() {
            if ($scope.model.close) {
                $scope.model.close();
            }
        };

        init();
    }
]);
