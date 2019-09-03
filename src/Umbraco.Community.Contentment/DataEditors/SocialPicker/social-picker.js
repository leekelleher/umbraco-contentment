/* Copyright © 2019 Lee Kelleher, Umbrella Inc and other contributors.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

angular.module("umbraco").controller("Umbraco.Community.Contentment.DataEditors.SocialPicker.Controller", [
    "$scope",
    "editorService",
    function ($scope, editorService) {

        //console.log("social-picker.model", $scope.model);

        var defaultConfig = {
            overlayView: "/App_Plugins/Contentment/editors/social-picker.overlay.html"
        };
        var config = angular.extend({}, defaultConfig, $scope.model.config);

        var vm = this;

        function init() {

            $scope.model.value = $scope.model.value || "";

            if ($scope.model.value !== "") {
                vm.button = "btn-" + $scope.model.value;
                vm.icon = "fa fa-" + $scope.model.value;
            }
            else {
                vm.button = "btn-lk-add";
                vm.icon = "icon-add";
            }

            vm.pick = pick;
        };

        function pick() {
            editorService.open({
                size: "small",
                view: config.overlayView,
                value: $scope.model.value,
                submit: function (model) {

                    $scope.model.value = model.network;

                    vm.button = "btn-" + model.network;
                    vm.icon = "fa fa-" + (model.icon || model.network);

                    console.log("submit", model, $scope.model.value);

                    setDirty();

                    editorService.close();
                },
                close: function () {
                    editorService.close();
                }
            });
        };

        function setDirty() {
            if ($scope.propertyForm) {
                $scope.propertyForm.$setDirty();
            }
        };

        init();
    }
]);
