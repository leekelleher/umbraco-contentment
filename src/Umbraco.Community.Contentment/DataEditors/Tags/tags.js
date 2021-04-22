/* Copyright © 2021 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

angular.module("umbraco").controller("Umbraco.Community.Contentment.DataEditors.Tags.Controller", [
    "$rootScope",
    "$scope",
    "$element",
    "angularHelper",
    "assetsService",
    "localizationService",
    "overlayService",
    function ($rootScope, $scope, $element, angularHelper, assetsService, localizationService, overlayService) {

        //console.log("tags.model", $scope.model);

        var defaultConfig = {
            allowClear: 0,
            confirmRemoval: 0,
            defaultValue: [],
            items: [],
            showIcons: 0,
        };
        var config = Object.assign({}, defaultConfig, $scope.model.config);

        var vm = this;

        function init() {

            $scope.model.value = $scope.model.value || config.defaultValue;

            if (Array.isArray($scope.model.value) === false) {
                $scope.model.value = [$scope.model.value];
            }

            config.confirmRemoval = Object.toBoolean(config.confirmRemoval);

            vm.items = [];

            vm.showIcons = Object.toBoolean(config.showIcons);

            vm.uniqueId = $scope.model.hasOwnProperty("dataTypeKey")
                ? ["tags", $scope.model.alias, $scope.model.dataTypeKey.substring(0, 8)].join("-")
                : ["tags", $scope.model.alias].join("-");

            vm.add = add;
            vm.keyDown = keyDown;
            vm.keyUp = keyUp;
            vm.remove = remove;

            vm.loading = true;

            assetsService.loadJs("lib/typeahead.js/typeahead.bundle.min.js").then(function () {

                $scope.model.value.forEach(function (v) {
                    var item = config.items.find(x => x.value === v);
                    if (item) {
                        vm.items.push(Object.assign({}, item));
                    }
                });

                vm.loading = false;

                var engine = new Bloodhound({
                    datumTokenizer: Bloodhound.tokenizers.obj.whitespace("name", "value"),
                    queryTokenizer: Bloodhound.tokenizers.whitespace,
                    initialize: false,
                    local: config.items.filter(x => !x.disabled),
                    identify: d => d.value,
                });

                engine.initialize().then(function () {

                    var opts = {
                        hint: true,
                        highlight: true,
                        minLength: 1
                    };

                    var sources = {
                        display: "name",
                        minLength: 0,
                        source: function (q, sync) {
                            if (q && q.length > 0) {
                                engine.search(q, sync);
                            } else {
                                sync(engine.all());
                            }
                        }
                    };

                    vm.editor = $element.find("input.typeahead")
                        .typeahead(opts, sources)
                        .bind("typeahead:select", add)
                        .bind("typeahead:autocomplete", add);
                });

            });

            if ($scope.umbProperty) {

                vm.propertyActions = [];

                if (Object.toBoolean(config.allowClear) === true) {
                    vm.propertyActions.push({
                        labelKey: "buttons_clearSelection",
                        icon: "trash",
                        method: clear
                    });
                }

                if (vm.propertyActions.length > 0) {
                    $scope.umbProperty.setPropertyActions(vm.propertyActions);
                }
            }
        };

        function add($event, item) {
            angularHelper.safeApply($rootScope, function () {

                vm.items.push(Object.assign({}, item));

                $scope.model.value.push(item.value);

                vm.editor.typeahead("val", "");

                setDirty();
            });
        };

        function clear() {
            vm.items = [];
            $scope.model.value = [];
            setDirty();
        };

        function keyDown($event) {
            if ($event.keyCode == 13) {
                var tt = vm.editor.data("ttTypeahead");
                if (tt && tt.menu && tt.menu.isOpen() === true) {
                    var selection = tt.menu.getActiveSelectable() || tt.menu.getTopSelectable();
                    tt.menu.trigger("selectableClicked", selection);
                    $event.preventDefault();
                }
            }
        };

        function keyUp($event, $index) {
            if ($event.keyCode === 8 || $event.keyCode === 46) {
                remove($index);
            }
        };

        function remove($index) {
            if (config.confirmRemoval === true) {
                var keys = ["contentment_removeItemMessage", "general_remove", "general_cancel", "contentment_removeItemButton"];
                localizationService.localizeMany(keys).then(function (data) {
                    overlayService.open({
                        title: data[1],
                        content: data[0],
                        closeButtonLabel: data[2],
                        submitButtonLabel: data[3],
                        submitButtonStyle: "danger",
                        submit: function () {
                            removeItem($index);
                            overlayService.close();
                        },
                        close: function () {
                            overlayService.close();
                        }
                    });
                });
            } else {
                removeItem($index);
            }
        };

        function removeItem($index) {

            vm.items.splice($index, 1);

            $scope.model.value.splice($index, 1);

            setDirty();
        };

        function setDirty() {
            if ($scope.propertyForm) {
                $scope.propertyForm.$setDirty();
            }
        };

        init();
    }
]);
