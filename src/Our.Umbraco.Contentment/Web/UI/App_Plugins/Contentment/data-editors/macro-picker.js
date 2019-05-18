/* Copyright © 2019 Lee Kelleher, Umbrella Inc and other contributors.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

angular.module("umbraco").controller("Our.Umbraco.Contentment.DataEditors.MacroPicker.Controller", [
    "$scope",
    "entityResource",
    "editorService",
    function ($scope, entityResource, editorService) {

        //console.log("macroPicker.model", $scope.model);

        var defaultConfig = { allowedMacros: { entityType: "Macro", items: [] }, maxItems: 0, disableSorting: 0 };
        var config = angular.extend({}, defaultConfig, $scope.model.config);

        var vm = this;
        var allowedMacros = [];

        function init() {

            vm.loading = true;

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

            if (config.allowedMacros.items.length > 0) {
                // NOTE: Can't use `entityResource.getByIds` as Macros aren't supported there.
                // https://github.com/umbraco/Umbraco-CMS/blob/release-8.0.2/src/Umbraco.Web/Editors/EntityController.cs#L742-L744
                // So we have to get all of them and filter.
                entityResource.getAll(config.allowedMacros.entityType).then(function (data) {
                    _.each(config.allowedMacros.items, function (udi) {
                        var entity = _.find(data, function (item) { return item.udi === udi });
                        if (entity) {
                            allowedMacros.push(entity.alias);
                        }
                    });
                });
            }

            vm.loading = false;
        };

        function add($event) {
            var macroPicker = {
                dialogData: { richTextEditor: false, macroData: { macroAlias: "" }, allowedMacros: allowedMacros },
                submit: function (model) {

                    $scope.model.value.push({
                        udi: model.selectedMacro.udi,
                        name: model.selectedMacro.name,
                        alias: model.selectedMacro.alias,
                        params: _.object(_.map(model.macroParams, function (p) { return [p.alias, p.value]; }))
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

        function edit($index, item) {
            var macroPicker = {
                dialogData: { richTextEditor: false, macroData: { macroAlias: item.alias, macroParamsDictionary: item.params }, allowedMacros: allowedMacros },
                submit: function (model) {
                    $scope.model.value[$index] = {
                        udi: model.selectedMacro.udi,
                        name: model.selectedMacro.name,
                        alias: model.selectedMacro.alias,
                        params: _.object(_.map(model.macroParams, function (p) { return [p.alias, p.value]; }))
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
