/* Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

angular.module("umbraco").controller("Umbraco.Community.Contentment.DataEditors.ItemPicker.Controller", [
    "$scope",
    "editorService",
    "localizationService",
    "overlayService",
    function ($scope, editorService, localizationService, overlayService) {

        // console.log("item-picker.model", $scope.model);

        var defaultConfig = {
            allowDuplicates: 0,
            defaultIcon: "icon-science",
            defaultValue: [],
            disableSorting: 0,
            enableFilter: 1,
            enableMultiple: 0,
            items: [],
            maxItems: 0,
            listType: "grid",
            overlayView: "",
            overlayOrderBy: "name",
            enableDevMode: 0,
        };
        var config = angular.extend({}, defaultConfig, $scope.model.config); // TODO: Replace AngularJS dependency. [LK:2020-03-02]

        var vm = this;

        function init() {

            $scope.model.value = $scope.model.value || config.defaultValue;

            if (_.isArray($scope.model.value) === false) {
                $scope.model.value = [$scope.model.value];
            }

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

            vm.enableDevMode = Object.toBoolean(config.enableDevMode);

            vm.add = add;
            vm.bind = bind;
            vm.remove = remove;

            bind();
        };

        function add() {

            var items = Object.toBoolean(config.allowDuplicates) ? config.items : _.reject(config.items, function (x) {
                return _.find(vm.items, function (y) { return x.name === y.name; });
            });

            ensureIcons(items);

            var itemPicker = {
                config: {
                    title: "Choose...",
                    enableFilter: Object.toBoolean(config.enableFilter),
                    enableMultiple: Object.toBoolean(config.enableMultiple),
                    items: items,
                    listType: config.listType,
                    orderBy: config.overlayOrderBy,
                },
                view: config.overlayView,
                size: "small",
                submit: function (selectedItems) {

                    _.each(selectedItems, function (x) {
                        vm.items.push(angular.copy(x)); // TODO: Replace AngularJS dependency. [LK:2020-03-02]
                        $scope.model.value.push(x.value);
                    });

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

        function bind() {

            vm.items = [];

            if ($scope.model.value.length > 0 && config.items.length > 0) {
                var orphaned = [];

                _.each($scope.model.value, function (v) {
                    var item = _.find(config.items, function (x) { return x.value === v });
                    if (item) {
                        vm.items.push(angular.copy(item)); // TODO: Replace AngularJS dependency. [LK:2020-03-02]
                    } else {
                        orphaned.push(v);
                    }
                });

                if (orphaned.length > 0) {
                    $scope.model.value = _.difference($scope.model.value, orphaned);

                    if ((config.maxItems === 0 || config.maxItems === "0") || $scope.model.value.length < config.maxItems) {
                        vm.allowAdd = true;
                    }
                }

                ensureIcons(vm.items);
            }
        };

        function remove($index) {
            var keys = ["content_nestedContentDeleteItem", "general_delete", "general_cancel", "contentTypeEditor_yesDelete"];
            localizationService.localizeMany(keys).then(function (data) {
                overlayService.open({
                    title: data[1],
                    content: data[0],
                    closeButtonLabel: data[2],
                    submitButtonLabel: data[3],
                    submitButtonStyle: "danger",
                    submit: function () {

                        vm.items.splice($index, 1);
                        $scope.model.value.splice($index, 1);

                        if ((config.maxItems === 0 || config.maxItems === "0") || $scope.model.value.length < config.maxItems) {
                            vm.allowAdd = true;
                        }

                        setDirty();

                        overlayService.close();
                    },
                    close: function () {
                        overlayService.close();
                    }
                });
            });
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
