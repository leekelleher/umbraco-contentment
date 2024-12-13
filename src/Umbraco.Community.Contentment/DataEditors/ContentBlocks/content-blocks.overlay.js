﻿/* Copyright © 2019 Lee Kelleher.
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
            currentPage: null,
            currentPageId: -2,
        };
        var config = Object.assign({}, defaultConfig, $scope.model.config);

        var vm = this;

        function init() {

            // HACK: Faking `umbVariantContentController` to support v13 RTE (and subsequently the BlockList).
            vm.constructor = { name: "umbVariantContentController" };
            vm.editor = { content: { name: "?", language: null } };
            // END HACK!

            vm.submit = submit;
            vm.close = close;

            // NOTE: Fixes https://github.com/leekelleher/umbraco-contentment/issues/250
            vm.contentNodeModel = config.currentPage || { variants: [] };

            if (config.elementType && $scope.model.value) {

                edit(config.elementType, $scope.model.value);

            } else {

                vm.mode = "select";
                vm.items = config.elementTypes;
                vm.selectedElementType = null;

                vm.clipboardItems = clipboardService.retrieveDataOfType("elementType", config.elementTypes.map(item => item.alias));

                if (config.elementTypes.length > 1 || vm.clipboardItems.length > 0) {

                    vm.title = "Add content";
                    vm.description = "Select a content type...";
                    vm.icon = "icon-page-add";
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
            clipboardService.clearEntriesOfType("elementType", config.elementTypes.map(item => item.alias));
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
                    vm.icon = "icon-blueprint";
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
            vm.loading = true;

            vm.title = "Edit content";
            vm.description = elementType.name;
            vm.icon = elementType.icon;

            vm.content = {
                elementType: elementType.key,
                key: String.CreateGuid()
            };

            // TODO: [v9] [LK] Review this, get error with blueprint API request, 404.
            // "Failed to retrieve blueprint for id 1082"
            // e.g. /umbraco/backoffice/umbracoapi/content/GetEmpty?blueprintId=1082&parentId=1076
            var getScaffold = blueprint && blueprint.id > 0
                ? contentResource.getBlueprintScaffold(config.currentPageId, blueprint.id)
                : contentResource.getScaffold(config.currentPageId, elementType.alias);

            getScaffold.then(data => {
                Object.assign(vm.content, data.variants[0]);
                vm.loading = false;
            });

        };

        function paste(bloat) {

            var elementType = config.elementTypes.find(x => x.alias === bloat.contentTypeAlias);

            $scope.model.size = elementType.overlaySize;

            var item = {
                elementType: elementType.key,
                key: String.CreateGuid(),
                value: {}
            };

            // NOTE: De-bloat the copied value (so much bloat from NC) ¯\_(ツ)_/¯
            if (bloat.variants.length > 0) {
                for (var t = 0; t < bloat.variants[0].tabs.length; t++) {
                    var tab = bloat.variants[0].tabs[t];
                    for (var p = 0; p < tab.properties.length; p++) {
                        var property = tab.properties[p];
                        if (typeof property.value !== "function") {
                            // NOTE: Gah, NC adds `propertyAlias` property! ¯\_(ツ)_/¯
                            item.value[property.propertyAlias] = property.value;
                        }
                    }
                }
            }

            edit(elementType, item);
        };

        function edit(elementType, element) {

            vm.mode = "edit";
            vm.loading = true;

            vm.title = "Edit content";
            vm.description = elementType.name;
            vm.icon = elementType.icon;

            vm.content = {
                elementType: elementType.key,
                key: element.key
            };

            contentResource.getScaffold(config.currentPageId, elementType.alias).then(data => {

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
