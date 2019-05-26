/* Copyright © 2019 Lee Kelleher, Umbrella Inc and other contributors.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

angular.module("umbraco").controller("Our.Umbraco.Contentment.DataEditors.ConfigurationEditor.Controller", [
    "$scope",
    "editorService",
    function ($scope, editorService) {

        // console.log("config-editor.model", $scope.model);

        var defaultConfig = { items: [], maxItems: 0, disableSorting: 0, overlaySize: "large" };
        var config = angular.extend({}, defaultConfig, $scope.model.config);

        var vm = this;

        function init() {

            $scope.model.value = $scope.model.value || [];

            // Ensure that the existing value is an array.
            if (_.isArray($scope.model.value) === false) {
                $scope.model.value = [$scope.model.value];
            }

            // TODO: If there are no available items, then show a messaging saying so. [LK]

            vm.allowAdd = (config.maxItems === 0 || config.maxItems === "0") || $scope.model.value.length < config.maxItems;
            vm.allowEdit = true;
            vm.allowRemove = true;
            vm.published = true;
            vm.sortable = Object.toBoolean(config.disableSorting) === false && (config.maxItems !== 1 && config.maxItems !== "1");

            vm.sortableOptions = {
                axis: "y",
                containment: "parent",
                cursor: "move",
                disabled: !vm.sortable,
                opacity: 0.7,
                scroll: true,
                tolerance: "pointer",
                stop: function (e, ui) {
                    setDirty();
                }
            };

            vm.add = add;
            vm.edit = edit;
            vm.remove = remove;

        };

        function add($event) {
            var configPicker = {
                view: "/App_Plugins/Contentment/data-editors/configuration-editor.overlay.html",
                size: "small",
                config: {
                    items: angular.copy(config.items),
                    overlaySize: config.overlaySize
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
            };

            editorService.open(configPicker);
        };

        function edit($index, item) {
            var configPicker = {
                view: "/App_Plugins/Contentment/data-editors/configuration-editor.overlay.html",
                size: config.overlaySize,
                config: {
                    items: angular.copy(config.items),
                    overlaySize: config.overlaySize
                },
                value: angular.copy($scope.model.value[$index]),
                submit: function (model) {

                    $scope.model.value[$index] = model;

                    setDirty();

                    editorService.close();
                },
                close: function () {
                    editorService.close();
                }
            };

            editorService.open(configPicker);
        };

        function remove($index) {
            $scope.model.value.splice($index, 1);

            if ((config.maxItems === 0 || config.maxItems === "0") || $scope.model.value.length < config.maxItems) {
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
