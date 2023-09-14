/* Copyright © 2020 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

angular.module("umbraco").controller("Umbraco.Community.Contentment.DataEditors.DictionaryPicker.Controller", [
    "$scope",
    "dictionaryResource",
    "editorService",
    function ($scope, dictionaryResource, editorService) {

        // console.log("dictionary-picker.model", $scope.model);

        var defaultConfig = {
            addButtonLabelKey: "general_add",
            disableSorting: 0,
            maxItems: 0,
        };
        var config = Object.assign({}, defaultConfig, $scope.model.config);

        var vm = this;

        function init() {

            $scope.model.value = $scope.model.value || [];

            if (Number.isInteger(config.maxItems) === false) {
                config.maxItems = Number.parseInt(config.maxItems) || defaultConfig.maxItems;
            }

            vm.addButtonLabelKey = config.addButtonLabelKey || "general_add";
            vm.defaultIcon = "icon-book-alt";
            vm.displayMode = "list";

            vm.allowAdd = (config.maxItems === 0 || config.maxItems === "0") || $scope.model.value.length < config.maxItems;
            vm.allowSort = Object.toBoolean(config.disableSorting) === false && config.maxItems !== 1;

            vm.add = add;
            vm.remove = remove;
        };

        function add() {
            editorService.treePicker({
                section: "translation",
                treeAlias: "dictionary",
                entityType: "dictionary",
                multiPicker: false,
                title: "Dictionary item",
                emptyStateMessage: "No dictionary items to choose from",
                select: function (model) {
                    dictionaryResource.getById(model.id).then(function (item) {

                        $scope.model.value.push({ key: item.key, name: item.name });

                        if ((config.maxItems !== 0 && config.maxItems !== "0") && $scope.model.value.length >= config.maxItems) {
                            vm.allowAdd = false;
                        }

                        setDirty();

                        editorService.close();
                    });
                },
                close: function (model) {
                    editorService.close();
                }
            });
        };

        function remove($index) {

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

        init();
    }
]);
