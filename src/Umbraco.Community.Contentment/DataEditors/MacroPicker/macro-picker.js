/* Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

angular.module("umbraco").controller("Umbraco.Community.Contentment.DataEditors.MacroPicker.Controller", [
    "$scope",
    "entityResource",
    "editorService",
    "localizationService",
    "overlayService",
    function ($scope, entityResource, editorService, localizationService, overlayService) {

        // console.log("macro-picker.model", $scope.model);

        var defaultConfig = {
            availableMacros: [],
            maxItems: 0,
            disableSorting: 0,
            addButtonLabelKey: "defaultdialogs_selectMacro",
        };
        var config = Object.assign({}, defaultConfig, $scope.model.config);

        var vm = this;

        function init() {

            $scope.model.value = $scope.model.value || [];

            vm.defaultIcon = "icon-settings-alt";
            vm.allowAdd = (config.maxItems === 0 || config.maxItems === "0") || $scope.model.value.length < config.maxItems;
            vm.allowEdit = true;
            vm.allowRemove = true;
            vm.allowSort = Object.toBoolean(config.disableSorting) === false && (config.maxItems !== 1 && config.maxItems !== "1");

            vm.addButtonLabelKey = config.addButtonLabelKey || "defaultdialogs_selectMacro";

            vm.add = add;
            vm.edit = edit;
            vm.remove = remove;
            vm.populateDescription = populateDescription;
        };

        function add() {
            editorService.macroPicker({
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
            });
        };

        function edit($index) {
            var item = $scope.model.value[$index];

            editorService.macroPicker({
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
            });
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

        function populateDescription(item, $index) {
            return item.alias;
        };

        function setDirty() {
            if ($scope.propertyForm) {
                $scope.propertyForm.$setDirty();
            }
        };

        init();
    }
]);
