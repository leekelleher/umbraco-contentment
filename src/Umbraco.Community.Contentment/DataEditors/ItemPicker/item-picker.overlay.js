/* Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

angular.module("umbraco").controller("Umbraco.Community.Contentment.Overlays.ItemPicker.Controller", [
    "$scope",
    function ($scope) {

        // console.log("item-picker-overlay.model", $scope.model);

        var defaultConfig = {
            title: "Select...",
            enableFilter: false,
            enableMultiple: false,
            items: [],
            listType: "grid",
            orderBy: "name",
        };
        var config = angular.extend({}, defaultConfig, $scope.model.config);

        var vm = this;

        function init() {

            vm.title = config.title;
            vm.enableFilter = config.enableFilter;
            vm.enableMultiple = config.enableMultiple;
            vm.items = config.items;
            vm.listType = config.listType;
            vm.orderBy = config.orderBy;

            vm.select = select;
            vm.submit = submit;
            vm.close = close;
        };

        function select(item) {
            if (vm.enableMultiple) {
                item.selected = !item.selected;
            } else {
                $scope.model.value = item;
                submit();
            }
        };

        function submit() {
            if ($scope.model.submit) {

                var selectedItems = [];

                if (vm.enableMultiple) {
                    _.each(vm.items, function (x) {
                        if (x.selected) {
                            delete x.selected;
                            selectedItems.push(x);
                        }
                    });
                } else {
                    selectedItems.push($scope.model.value);
                }

                $scope.model.submit(selectedItems);
            }
        };

        function close() {
            if ($scope.model.close) {
                $scope.model.close();
            }
        };

        init();
    }
]);
