/* Copyright © 2020 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

angular.module("umbraco").controller("Umbraco.Community.Contentment.DataEditors.DataList.Editor.Controller", [
    "$scope",
    "editorService",
    "localizationService",
    "overlayService",
    "Umbraco.Community.Contentment.Services.DevMode",
    function ($scope, editorService, localizationService, overlayService, devModeService) {

        // console.log("data-list.editor.model", $scope.model);

        var defaultConfig = {
            confirmRemoval: 0,
            defaultIcon: "icon-stop",
            enableDevMode: 0,
            maxItems: 0,
            notes: null,
        };
        var config = Object.assign({}, defaultConfig, $scope.model.config);

        var vm = this;

        function init() {

            $scope.model.value = $scope.model.value || [];

            if (Number.isInteger(config.maxItems) === false) {
                config.maxItems = Number.parseInt(config.maxItems) || defaultConfig.maxItems;
            }

            config.confirmRemoval = Object.toBoolean(config.confirmRemoval);

            vm.allowAdd = config.maxItems === 0 || $scope.model.value.length < config.maxItems;
            vm.focusName = false;

            vm.sortableOptions = {
                axis: "y",
                containment: "parent",
                cursor: "move",
                opacity: 0.7,
                scroll: true,
                tolerance: "pointer",
                stop: (e, ui) => setDirty()
            };

            vm.notes = config.notes;

            vm.add = add;
            vm.blur = blur;
            vm.edit = edit;
            vm.open = open;
            vm.remove = remove;

            if (Object.toBoolean(config.enableDevMode) === true && $scope.umbProperty) {
                $scope.umbProperty.setPropertyActions([{
                    labelKey: "contentment_editRawValue",
                    icon: "brackets",
                    method: edit
                }, {
                    labelKey: "clipboard_labelForRemoveAllEntries",
                    icon: "trash",
                    method: () => {
                        $scope.model.value = [];
                    }
                }]);
            }

        };

        function add() {

            vm.focusName = false;

            $scope.model.value.push({
                icon: config.defaultIcon,
                name: "",
                value: "",
                description: "",
            });

            setDirty();

        };

        function blur(item) {
            if (item.name && item.value == null || item.value === "") {
                item.value = item.name.toCamelCase();
            }
        };

        function edit() {
            devModeService.editValue($scope.model, () => {
                // NOTE: Any future validation can be done here.
            });
        };

        function open(item) {

            var parts = item.icon.split(" ");

            editorService.iconPicker({
                icon: parts[0],
                color: parts[1],
                submit: function (model) {

                    item.icon = [model.icon, model.color].filter(s => s).join(" ");

                    vm.focusName = true;

                    setDirty();

                    editorService.close();
                },
                close: function () {
                    editorService.close();
                }
            });
        };

        function remove($index) {
            if (config.confirmRemoval === true) {
                var keys = ["contentment_removeItemMessage", "general_remove", "general_cancel", "contentment_removeItemButton"];
                localizationService.localizeMany(keys).then(data => {
                    overlayService.open({
                        title: data[1],
                        content: data[0],
                        closeButtonLabel: data[2],
                        submitButtonLabel: data[3],
                        submitButtonStyle: "danger",
                        submit: function () {
                            removeItem($index);
                            overlayService.close();
                        },
                        close: function () {
                            overlayService.close();
                        }
                    });
                });
            } else {
                removeItem($index);
            }
        };

        function removeItem($index) {

            $scope.model.value.splice($index, 1);

            if (config.maxItems === 0 || vm.items.length < config.maxItems) {
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
