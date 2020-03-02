/* Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

angular.module("umbraco").controller("Umbraco.Community.Contentment.DataEditors.MacroPicker.Controller", [
    "$scope",
    "entityResource",
    "editorService",
    function ($scope, entityResource, editorService) {

        // console.log("macro-picker.model", $scope.model);

        var defaultConfig = {
            availableMacros: [],
            maxItems: 0,
            disableSorting: 0
        };
        var config = angular.extend({}, defaultConfig, $scope.model.config); // TODO: Replace AngularJS dependency. [LK:2020-03-02]

        var vm = this;

        function init() {

            $scope.model.value = $scope.model.value || [];

            vm.icon = "icon-settings-alt";
            vm.allowAdd = (config.maxItems === 0 || config.maxItems === "0") || $scope.model.value.length < config.maxItems;
            vm.allowEdit = true;
            vm.allowRemove = true;
            vm.published = true;
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
            vm.edit = edit;
            vm.remove = remove;
        };

        function add() {
            var macroPicker = {
                dialogData: {
                    richTextEditor: false,
                    macroData: { macroAlias: "" },
                    allowedMacros: config.availableMacros
                },
                submit: function (model) {

                    $scope.model.value.push({
                        udi: model.selectedMacro.udi,
                        name: model.selectedMacro.name,
                        alias: model.selectedMacro.alias,
                        params: _.object(_.map(model.macroParams, function (p) { return [p.alias, p.value]; })) // TODO: Replace Underscore.js dependency. [LK:2020-03-02]
                    });

                    if ((config.maxItems !== 0 && config.maxItems !== "0") && $scope.model.value.length >= config.maxItems) {
                        vm.allowAdd = false;
                    }

                    setDirty();

                    editorService.close();
                },
                close: function () {
                    editorService.close();
                }
            }

            editorService.macroPicker(macroPicker);
        };

        function edit($index) {
            var item = $scope.model.value[$index];
            var macroPicker = {
                dialogData: {
                    richTextEditor: false,
                    macroData: { macroAlias: item.alias, macroParamsDictionary: item.params },
                    allowedMacros: config.availableMacros
                },
                submit: function (model) {
                    $scope.model.value[$index] = {
                        udi: model.selectedMacro.udi,
                        name: model.selectedMacro.name,
                        alias: model.selectedMacro.alias,
                        params: _.object(_.map(model.macroParams, function (p) { return [p.alias, p.value]; })) // TODO: Replace Underscore.js dependency. [LK:2020-03-02]
                    };

                    setDirty();

                    editorService.close();
                },
                close: function () {
                    editorService.close();
                }
            }

            editorService.macroPicker(macroPicker);
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
