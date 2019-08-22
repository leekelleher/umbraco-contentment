/* Copyright © 2019 Lee Kelleher, Umbrella Inc and other contributors.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

angular.module("umbraco").controller("Umbraco.Community.Contentment.DataEditors.IconPicker.Controller", [
    "$scope",
    "editorService",
    function ($scope, editorService) {

        // console.log("icon-picker.model", $scope.model);

        var defaultConfig = { defaultIcon: "icon-document" };
        var config = angular.extend({}, defaultConfig, $scope.model.config);

        var vm = this;

        function init() {
            $scope.model.value = $scope.model.value || config.defaultIcon;
            vm.pick = pick;
            vm.clear = clear;
        };

        function pick() {

            var parts = $scope.model.value.split(" ");

            var iconPicker = {
                icon: parts[0],
                color: parts[1],
                submit: function (model) {
                    $scope.model.value = [model.icon, model.color].filter(s => s).join(" ");
                    setDirty();
                    editorService.close();
                },
                close: function () {
                    editorService.close();
                }
            };

            editorService.iconPicker(iconPicker);
        };

        function clear() {
            // TODO: [LK:2019-08-22] Style up the clear button.
            $scope.model.value = "";
        };

        function setDirty() {
            if ($scope.propertyForm) {
                $scope.propertyForm.$setDirty();
            }
        };

        init();
    }
]);
