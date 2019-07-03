/* Copyright © 2019 Lee Kelleher, Umbrella Inc and other contributors.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

angular.module("umbraco").controller("Our.Umbraco.Contentment.DataEditors.Elements.Controller", [
    "$scope",
    "editorService",
    function ($scope, editorService) {

        if (_.has($scope.model, "contentTypeId")) {
            // NOTE: This will prevents the editor attempting to load whilst in the Content Type Editor's property preview panel.
            return;
        }

        console.log("elements.model", $scope.model);

        var defaultConfig = {
            items: [],
            maxItems: 0,
            disableSorting: 0,
            overlayView: "",
        };
        var config = angular.extend({}, defaultConfig, $scope.model.config);

        var vm = this;

        function init() {

            console.log("init");

            $scope.model.value = $scope.model.value || [];

            $scope.model.value = config.items;

            vm.allowAdd = (config.maxItems === 0 || config.maxItems === "0") || $scope.model.value.length < config.maxItems;

            vm.sortableOptions = {
                axis: "y",
                containment: "parent",
                cursor: "move",
                disabled: true,
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
            vm.reorder = reorder;
            vm.showPrompt = showPrompt;
            vm.hidePrompt = hidePrompt;
        };

        function add() {
            console.log("add");

            $scope.model.value.push({
                contentTypeName: "Hero",
                name: "Block " + ($scope.model.value.length + 1)
            });

        };

        function edit($index, item) {
            console.log("edit", $index, item);
        };

        function remove($index) {
            console.log("remove", $index);

            $scope.model.value.splice($index, 1);

            if ((config.maxItems === 0 || config.maxItems === "0") || $scope.model.value.length < config.maxItems) {
                vm.allowAdd = true;
            }

            setDirty();
        };

        function reorder() {
            console.log("reorder");
        }

        function showPrompt(item) {
            item.prompt = true;
        }

        function hidePrompt(item) {
            delete item.prompt;
        }

        function setDirty() {
            if ($scope.propertyForm) {
                $scope.propertyForm.$setDirty();
            }
        };

        init();
    }
]);
