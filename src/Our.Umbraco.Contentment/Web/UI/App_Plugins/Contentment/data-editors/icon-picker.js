/* Copyright © 2019 Lee Kelleher, Umbrella Inc and other contributors.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

angular.module("umbraco").controller("Our.Umbraco.Contentment.DataEditors.IconPicker.Controller", [
    "$scope",
    "editorService",
    function ($scope, editorService) {

        console.log("icon-picker.model", $scope.model);

        var defaultConfig = {};
        var config = angular.extend({}, defaultConfig, $scope.model.config);

        var vm = this;

        function init() {

            vm.pick = pick;

        };

        function pick() {

            // TODO: You got up to here! [LK]
            // Next bit is to set the size/style of the preview box thing
            var iconPicker = {
                icon: $scope.model.value.split(' ')[0],
                color: $scope.model.value.split(' ')[1],
                submit: function (model) {

                    console.log("pick.submit", model);

                    if (model.icon) {

                        if (model.color) {

                            $scope.model.value = model.icon + " " + model.color;

                        } else {

                            $scope.model.value = model.icon;

                        }

                        setDirty();
                    }

                    editorService.close();
                },
                close: function () {
                    editorService.close();
                }
            };

            editorService.iconPicker(iconPicker);
        };

        function setDirty() {
            if ($scope.propertyForm) {
                $scope.propertyForm.$setDirty();
            }
        };

        init();
    }
]);
