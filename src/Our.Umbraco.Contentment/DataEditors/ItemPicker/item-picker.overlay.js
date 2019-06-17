/* Copyright © 2019 Lee Kelleher, Umbrella Inc and other contributors.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

angular.module("umbraco").controller("Our.Umbraco.Contentment.Overlays.ItemPicker.Controller", [
    "$scope",
    function ($scope) {

        // console.log("item-picker-overlay.model", $scope.model);

        var defaultConfig = {
            title: "Select...",
            enableFilter: false,
            items: [],
            listType: "grid",
            orderBy: "name",
        };
        var config = angular.extend({}, defaultConfig, $scope.model.config);

        var vm = this;

        function init() {

            vm.title = config.title;
            vm.enableFilter = config.enableFilter;
            vm.items = config.items;
            vm.listType = config.listType;
            vm.orderBy = config.orderBy;

            vm.select = select;
            vm.submit = submit;
            vm.close = close;
        };

        function select(item) {
            // TODO: [LK:2019-06-17] Explore what would be involved in making this multiple selection.
            $scope.model.selectedItem = item;
            submit($scope.model);
        };

        function submit(model) {
            if ($scope.model.submit) {
                $scope.model.submit(model);
            }
        }

        function close() {
            if ($scope.model.close) {
                $scope.model.close();
            }
        }

        init();
    }
]);
