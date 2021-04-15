/* Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

angular.module("umbraco").controller("Umbraco.Community.Contentment.DataEditors.DataTable.Controller", [
    "$scope",
    function ($scope) {

        // console.log("datatable.model", $scope.model);

        var defaultConfig = {
            fields: [],
            disableSorting: 0,
            maxItems: 0,
            restrictWidth: 0,
            usePrevalueEditors: 1
        };
        var config = Object.assign({}, defaultConfig, $scope.model.config);

        var vm = this;

        function init() {

            $scope.model.value = $scope.model.value || [];

            vm.headings = config.fields.map(function (x) { return x.label });
            vm.items = [];

            $scope.model.value.forEach(function (value, row) {

                var fields = [];

                config.fields.forEach(function (field, cell) {
                    fields.push(Object.assign({
                        alias: [$scope.model.alias, row, cell].join("_"),
                        value: value[field.key]
                    }, field));
                });

                vm.items.push(fields);
            });

            vm.styleTable = {
                "max-width": Object.toBoolean(config.restrictWidth) ? "800px" : "100%",
                "background-color": "#f6f4f4",
            };
            vm.styleButton = Object.toBoolean(config.restrictWidth) ? {} : { "max-width": "100%" };

            vm.usePrevalueEditors = Object.toBoolean(config.usePrevalueEditors) ? true : null;

            vm.allowAdd = (config.maxItems === 0 || config.maxItems === "0") || vm.items.length < config.maxItems;
            vm.allowRemove = true;

            vm.sortable = Object.toBoolean(config.disableSorting) === false && (config.maxItems !== 1 && config.maxItems !== "1");

            vm.sortableOptions = {
                axis: "y",
                containment: "parent",
                cursor: "move",
                disabled: vm.sortable === false,
                forcePlaceholderSize: true,
                handle: ".handle",
                helper: function (e, ui) {
                    ui.children().each(function () {
                        $(this).width($(this).width());
                    });
                    return ui.clone();
                },
                items: "> tr",
                opacity: 0.7,
                scroll: true,
                tolerance: "pointer",
                stop: function (e, ui) {
                    setDirty();
                }
            };

            vm.showPrompt = showPrompt;
            vm.hidePrompt = hidePrompt;

            vm.add = add;
            vm.remove = remove;
        };

        function add() {
            vm.items.push(angular.copy(config.fields)); // TODO: Replace AngularJS dependency. [LK:2020-12-17]

            if ((config.maxItems !== 0 && config.maxItems !== "0") && vm.items.length >= config.maxItems) {
                vm.allowAdd = false;
            }

            setDirty();
        };

        function remove($index) {
            vm.items.splice($index, 1);

            if ((config.maxItems === 0 || config.maxItems === "0") || vm.items.length < config.maxItems) {
                vm.allowAdd = true;
            }

            setDirty();
        };

        function showPrompt(row) {
            row.prompt = true;
        };

        function hidePrompt(row) {
            delete row.prompt;
        };

        function setDirty() {
            if ($scope.propertyForm) {
                $scope.propertyForm.$setDirty();
            }
        };

        var unsubscribe = $scope.$on("formSubmitting", function (ev, args) {

            $scope.model.value = [];

            vm.items.forEach(function (row) {

                var obj = {};

                row.forEach(function (cell) {
                    obj[cell.key] = cell.value;
                });

                $scope.model.value.push(obj);

            });
        });

        $scope.$on("$destroy", function () {
            unsubscribe();
        });

        init();
    }
]);
