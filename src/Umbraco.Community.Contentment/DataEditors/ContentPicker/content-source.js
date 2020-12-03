/* Copyright © 2020 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

angular.module("umbraco").controller("Umbraco.Community.Contentment.DataEditors.ContentSource.Controller", [
    "$scope",
    "editorService",
    "entityResource",
    function ($scope, editorService, entityResource) {

        //console.log("content-source.model", $scope.model);

        var defaultConfig = {
        };
        var config = Object.assign({}, defaultConfig, $scope.model.config);

        var vm = this;

        function init() {

            // NOTE: If it starts with "umb://" we assume it's an Umbraco UDI, otherwise it's an XPath.
            if ($scope.model.value && $scope.model.value.startsWith("umb://document/")) {
                vm.node = {};
                vm.loading = true;
                entityResource.getById($scope.model.value, "Document").then(function (item) {
                    populate(item);
                    vm.loading = false;
                })
            } else {
                vm.node = null;
            }

            vm.showHelp = false;
            vm.showSearch = false;

            vm.pick = pick;
            vm.remove = remove;

            vm.show = show;
            vm.hide = hide;

            vm.help = help;
            vm.clear = clear;
        };

        function pick() {

            editorService.treePicker({
                idType: "udi",
                section: "content",
                treeAlias: "content",
                multiPicker: false,
                submit: function submit(model) {

                    var item = model.selection[0];

                    populate(item);

                    $scope.model.value = item.udi;

                    editorService.close();
                },
                close: function close() {
                    editorService.close();
                }
            });

        };

        function remove() {
            $scope.model.value = null;
            vm.node = null;
        };

        function show() {
            vm.showSearch = true;
        };

        function hide() {
            vm.showSearch = false;
        };

        function help() {
            vm.showHelp = !vm.showHelp;
        }

        function clear() {
            vm.showSearch = false;
            $scope.model.value = null;
        };

        function populate(item) {
            vm.node = item;
            entityResource.getUrl(item.id, "Document").then(function (data) {
                vm.node.path = data;
            });
        }

        init();
    }
]);
