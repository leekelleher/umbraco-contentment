/* Copyright © 2019 Lee Kelleher, Umbrella Inc and other contributors.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

angular.module("umbraco").controller("Umbraco.Community.Contentment.DataEditors.SocialLinks.Controller", [
    "$scope",
    "editorService",
    function ($scope, editorService) {

        // console.log("social-links.model", $scope.model);

        var defaultConfig = {
            overlayView: "/App_Plugins/Contentment/editors/social-picker.overlay.html"
        };
        var config = angular.extend({}, defaultConfig, $scope.model.config);

        var vm = this;

        function init() {

            $scope.model.value = $scope.model.value || [];

            vm.sortableOptions = {
                axis: "y",
                containment: "parent",
                cursor: "move",
                handle: ".handle",
                opacity: 0.7,
                scroll: true,
                tolerance: "pointer",
                stop: function (e, ui) {
                    setDirty();
                }
            };

            vm.promptIsVisible = -1;
            vm.showPrompt = showPrompt;
            vm.hidePrompt = hidePrompt;

            vm.add = add;
            vm.pick = pick;
            vm.remove = remove;
        };

        function add() {
            $scope.model.value.push({
                network: "",
                button: "btn-lk-add",
                icon: "icon-add",
                name: "",
                url: "",
            });
            pick($scope.model.value.length - 1);
        };

        function pick($index) {

            var item = $scope.model.value[$index];

            editorService.open({
                size: "small",
                view: config.overlayView,
                value: item,
                submit: function (model) {

                    item.network = model.network;
                    item.button = "btn-" + model.network;
                    item.icon = "fa fa-" + (model.icon || model.network);

                    if (item.name === '') {
                        item.name = model.name;
                    }

                    if (item.url === '') {
                        item.url = model.url;
                    }

                    setDirty();

                    editorService.close();
                },
                close: function () {
                    editorService.close();
                }
            });
        };

        function remove($index) {
            vm.hidePrompt();
            $scope.model.value.splice($index, 1);
        };

        function showPrompt($index) {
            vm.promptIsVisible = $index;
        };

        function hidePrompt() {
            vm.promptIsVisible = -1;
        };

        function setDirty() {
            if ($scope.propertyForm) {
                $scope.propertyForm.$setDirty();
            }
        };

        init();
    }
]);
