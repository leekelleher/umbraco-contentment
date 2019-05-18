/* Copyright © 2019 Lee Kelleher, Umbrella Inc and other contributors.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

angular.module("umbraco").controller("Our.Umbraco.Contentment.DataEditors.ConfigurationEditor.Controller", [
    "$scope",
    "editorService",
    function ($scope, editorService) {

        console.log("picker.model", $scope.model);

        var defaultConfig = { items: [], maxItems: 0, disableSorting: 0 };
        var config = angular.extend({}, defaultConfig, $scope.model.config);

        var vm = this;

        function init() {

            $scope.model.value = $scope.model.value || [];

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
                    items: angular.copy(config.items)
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
                size: "large",
                config: {
                    items: angular.copy(config.items)
                },
                value: $scope.model.value[$index],
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

angular.module("umbraco").controller("Our.Umbraco.Contentment.Overlays.ConfigurationEditor.Controller", [
    "$scope",
    "formHelper",
    function ($scope, formHelper) {

        console.log("overlay.model", $scope.model);

        var defaultConfig = { items: [] };
        var config = angular.extend({}, defaultConfig, $scope.model.config);

        var vm = this;

        function init() {

            vm.selectedItem = $scope.model.value || { type: "", name: "", icon: "", value: {} };

            if (_.isEmpty(vm.selectedItem.type)) {

                vm.title = "Select...";
                vm.mode = "select";
                vm.items = config.items;

            } else {
                vm.title = "Configure";
                vm.mode = "edit";

                vm.editor = _.find(config.items, function (x) {
                    return x.type === vm.selectedItem.type;
                });

                if (!vm.editor) {
                    // TODO: What to do if we don't find the config? [LK]
                    console.log("Unable to find error:", vm.selectedItem.type)
                }

                if (vm.editor.fields.length > 0) {
                    _.each(vm.editor.fields, function (x) {
                        x.alias = x.key;
                        x.value = vm.selectedItem.value[x.key];
                    });
                }
            }

            vm.close = close;
            vm.save = save;
            vm.select = select;

        };

        function select(item) {
            vm.title = item.name;
            vm.mode = "edit";
            $scope.model.size = "large";
            vm.editor = item;
        };

        function close() {
            if ($scope.model.close) {
                $scope.model.close();
            }
        };

        function save(item) {

            // TODO: Not sure if we need to use `formHelper.submitForm` here? e.g. `formHelper.submitForm({ scope: $scope, formCtrl: this.configurationEditorForm })`
            // https://github.com/umbraco/Umbraco-CMS/blob/release-8.0.2/src/Umbraco.Web.UI.Client/src/common/services/formhelper.service.js#L26
            $scope.$broadcast("formSubmitting", { scope: $scope });

            var obj = {
                type: item.type,
                name: item.name,
                icon: item.icon,
                value: {}
            };

            _.each(item.fields, function (x) {
                obj.value[x.key] = x.value;
            });

            if ($scope.model.submit) {
                $scope.model.submit(obj);
            }
        };

        init();
    }
]);
