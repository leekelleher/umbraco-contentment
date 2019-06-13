/* Copyright © 2019 Lee Kelleher, Umbrella Inc and other contributors.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

angular.module("umbraco").controller("Our.Umbraco.Contentment.DataEditors.ItemPicker.Controller", [
    "$scope",
    "editorService",
    function ($scope, editorService) {

        // console.log("item-picker.model", $scope.model);

        var defaultConfig = {
            items: [],
            maxItems: 0,
            allowDuplicates: 0,
            enableFilter: 1,
            disableSorting: 0,
            defaultIcon: "icon-science"
        };
        var config = angular.extend({}, defaultConfig, $scope.model.config);

        var vm = this;

        function init() {

            $scope.model.value = $scope.model.value || [];

            vm.items = [];
            vm.orphaned = []; // [LK:2019-06-13] TODO: What to do about the orphaned items? [LK]
            vm.icon = config.defaultIcon;
            vm.allowAdd = (config.maxItems === 0 || config.maxItems === "0") || $scope.model.value.length < config.maxItems;
            vm.allowEdit = false;
            vm.allowRemove = true;
            vm.published = true;
            vm.sortable = Object.toBoolean(config.disableSorting) === false && (config.maxItems !== 1 && config.maxItems !== "1");

            vm.sortableOptions = {
                axis: "y",
                containment: "parent",
                cursor: "move",
                disabled: vm.sortable === false,
                opacity: 0.7,
                scroll: true,
                tolerance: "pointer",
                stop: function (e, ui) {
                    $scope.model.value = _.map(vm.items, function (x) { return x.value });
                    setDirty();
                }
            };

            vm.add = add;
            vm.remove = remove;

            if ($scope.model.value.length > 0 && config.items.length > 0) {
                _.each($scope.model.value, function (v) {
                    var item = _.find(config.items, function (x) { return x.value === v });
                    if (item) {
                        vm.items.push(angular.copy(item));
                    } else {
                        vm.orphaned.push(v); //console.log("orphaned value", v); // TODO: [LK:2019-06-13] What to do about orphaned values? [LK]
                    }
                });

                ensureIcons(vm.items);
            }
        };

        function add($event) {

            var availableItems = Object.toBoolean(config.allowDuplicates) ? config.items : _.reject(config.items, function (x) {
                return _.find(vm.items, function (y) { return x.name === y.name; });
            });

            ensureIcons(availableItems);

            var itemPicker = {
                title: "Choose...",
                // TODO: [LK:2019-06-13] NOTE: I've copied over the "itempicker.html" from Umbraco v8.0.2, as it has `orderBy:'name'` hardcoded, and I need it display the items as provided.
                // https://github.com/umbraco/Umbraco-CMS/blob/release-8.0.2/src/Umbraco.Web.UI.Client/src/views/common/infiniteeditors/itempicker/itempicker.html#L28
                // PR will be submitted.
                // TODO: [LK:2019-06-13] Not happy with this being hard-coded, try passing in via config.
                view: "/App_Plugins/Contentment/editors/item-picker.overlay.html",
                size: "small",
                availableItems: availableItems,
                filter: Object.toBoolean(config.enableFilter),
                submit: function (model) {

                    vm.items.push(angular.copy(model.selectedItem))
                    $scope.model.value.push(model.selectedItem.value);

                    if ((config.maxItems !== 0 && config.maxItems !== "0") && $scope.model.value.length >= config.maxItems) {
                        vm.allowAdd = false;
                    }

                    setDirty();

                    editorService.close();
                },
                close: function () {
                    editorService.close();
                }
            };

            editorService.open(itemPicker);

        };

        function remove($index) {

            vm.items.splice($index, 1);
            $scope.model.value.splice($index, 1);

            if ((config.maxItems === 0 || config.maxItems === "0") || $scope.model.value.length < config.maxItems) {
                vm.allowAdd = true;
            }

            setDirty();
        };

        function ensureIcons(items) {
            _.each(items, function (x) {
                if (x.hasOwnProperty("icon") === false) {
                    x.icon = config.defaultIcon;
                }
            });
        }

        function setDirty() {
            if ($scope.propertyForm) {
                $scope.propertyForm.$setDirty();
            }
        };

        init();
    }
]);
