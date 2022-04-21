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
            defaultIcon: "icon-science",
            items: [],
            listType: "grid",
            orderBy: "name",
            maxItems: 0,
        };
        var config = Object.assign({}, defaultConfig, $scope.model.config);

        var vm = this;

        function init() {

            vm.title = config.title;
            vm.enableFilter = config.enableFilter;
            vm.enableMultiple = config.enableMultiple;
            vm.defaultIcon = config.defaultIcon;
            vm.items = config.items;
            vm.listType = config.listType;
            vm.orderBy = config.orderBy;

            vm.maxItems = config.maxItems;
            vm.itemCount = 0;
            vm.allowSubmit = false;

            vm.select = select;
            vm.submit = submit;
            vm.close = close;
        };

        function select(item) {

            if (item.disabled === true) {
                return;
            }

            if (vm.enableMultiple === true) {
                item.selected = !item.selected;
                vm.itemCount = vm.items.filter(x => x.selected === true).length;
                vm.allowSubmit = vm.itemCount > 0 && (config.maxItems === 0 || vm.itemCount <= config.maxItems);
            } else {
                $scope.model.value = item;
                submit();
            }
        };

        function submit() {
            if ($scope.model.submit) {

                var selectedItems = [];

                if (vm.enableMultiple === true) {
                    vm.items.forEach(item => {
                        if (item.selected) {
                            delete item.selected;
                            selectedItems.push(item);
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
