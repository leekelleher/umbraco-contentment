/* Copyright © 2023 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

angular.module("umbraco").controller("Umbraco.Community.Contentment.Overlays.DataPicker.Controller", [
    "$scope",
    "$http",
    "$routeParams",
    "umbRequestHelper",
    function ($scope, $http, $routeParams, umbRequestHelper) {

        //console.log("data-picker.overlay", $scope.model);

        var defaultConfig = {
            currentPageId: -1,
            dataTypeKey: null,
            defaultIcon: "icon-science",
            enableMultiple: false,
            hideSearch: false,
            listType: "cards",
            maxItems: 0,
            pageSize: 12,
            selectedItems: [],
        };
        var config = Object.assign({}, defaultConfig, $scope.model.config);

        var vm = this;

        function init() {

            vm.title = "Select items...";
            vm.defaultIcon = config.defaultIcon || "icon-science";
            vm.enableMultiple = config.enableMultiple;
            vm.hideSearch = config.hideSearch;
            vm.listType = config.listType;
            vm.maxItems = config.maxItems;

            vm.loading = true;

            vm.items = [];

            vm.allowSubmit = false;
            vm.selection = {};
            vm.selectionCount = 0;

            vm.searchOptions = {
                dataTypeKey: config.dataTypeKey,
                pageNumber: 1,
                pageSize: config.pageSize,
                query: "",
                totalPages: 0,
            };

            vm.close = close;
            vm.getImage = getImage;
            vm.isSelected = isSelected;
            vm.pagination = pagination;
            vm.search = search;
            vm.select = select;
            vm.submit = submit;

            _query();
        };

        const _debounce = _.debounce(() => $scope.$apply(_query), 500);

        function _query() {

            vm.loading = true;

            umbRequestHelper.resourcePromise(
                $http.post("backoffice/Contentment/DataPickerApi/Search", config.selectedItems, {
                    params: {
                        id: config.currentPageId,
                        dataTypeKey: vm.searchOptions.dataTypeKey,
                        pageNumber: vm.searchOptions.pageNumber,
                        pageSize: vm.searchOptions.pageSize,
                        query: encodeURIComponent(vm.searchOptions.query),
                        culture: $routeParams.cculture ?? $routeParams.mculture,
                        segment: $routeParams.csegment
                    }
                }),
                "Failed to retrieve search data.")
                .then(function (data) {
                    vm.loading = false;
                    vm.items = data.items || [];
                    vm.searchOptions.totalPages = data.totalPages;
                });
        };

        function close() {
            if ($scope.model.close) {
                $scope.model.close();
            }
        };

        function getImage(item) {
            return item && item.image
                ? { "background-image": `url('${item.image}')` }
                : null;
        };

        function isSelected(item) {
            return vm.selection.hasOwnProperty(item.value) === true;
        };

        function pagination(pageNumber) {
            vm.loading = true;
            vm.searchOptions.pageNumber = pageNumber;
            _query();
        };

        function search() {
            vm.loading = true;
            vm.searchOptions.pageNumber = 1;
            vm.searchOptions.totalPages = 0;
            _debounce();
        }

        function select(item, $event) {
            $event.stopPropagation();

            if (item.disabled === true) {
                return;
            }

            if (vm.enableMultiple === true) {

                if (vm.selection.hasOwnProperty(item.value) === false) {
                    vm.selection[item.value] = item;
                    vm.selectionCount++;
                } else {
                    delete vm.selection[item.value];
                    vm.selectionCount--;
                }

                vm.allowSubmit = vm.selectionCount > 0 && (config.maxItems === 0 || vm.selectionCount <= config.maxItems);

            } else {

                vm.selection = {};
                vm.selection[item.value] = item;
                submit();

            }
        };

        function submit() {
            if ($scope.model.submit) {
                $scope.model.submit(vm.selection);
            }
        };

        init();
    }
]);
