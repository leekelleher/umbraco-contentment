/* Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

angular.module("umbraco").controller("Umbraco.Community.Contentment.DataEditors.IconPicker.Controller", [
    "$scope",
    "editorService",
    function ($scope, editorService) {

        // console.log("icon-picker.model", $scope.model);

        var defaultConfig = {
            defaultIcon: "",
            hideColors: 0, // NOTE: This only applies to Umbraco 11.2+.
            size: "large"
        };
        var config = Object.assign({}, defaultConfig, $scope.model.config);

        var vm = this;

        function init() {
            $scope.model.value = $scope.model.value || config.defaultIcon;

            if (Array.isArray(config.size) === true) {
                config.size = config.size[0];
            }

            vm.label = $scope.model.value.replace(" ", "<br>");
            vm.size = config.size;

            vm.allowAdd = $scope.model.value === "";

            vm.pick = pick;
            vm.remove = remove;
        };

        function pick() {

            var parts = $scope.model.value.split(" ");

            var iconPicker = {
                icon: parts[0],
                color: parts[1],
                hideColors: Object.toBoolean(config.hideColors),
                submit: function (model) {
                    $scope.model.value = [model.icon, model.color].filter(s => s).join(" ");

                    vm.label = $scope.model.value.replace(" ", "<br>");

                    vm.allowAdd = false;

                    setDirty();

                    editorService.close();
                },
                close: function () {
                    editorService.close();
                }
            };

            editorService.iconPicker(iconPicker);
        };

        function remove() {
            $scope.model.value = "";

            vm.label = "";

            vm.allowAdd = true;

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
