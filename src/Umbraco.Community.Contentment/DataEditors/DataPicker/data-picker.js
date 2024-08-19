/* Copyright © 2023 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

angular.module("umbraco").controller("Umbraco.Community.Contentment.DataEditors.DataPicker.Controller", [
    "$scope",
    "$http",
    "editorService",
    "editorState",
    "localizationService",
    "overlayService",
    "umbRequestHelper",
    "Umbraco.Community.Contentment.Services.DevMode",
    function ($scope, $http, editorService, editorState, localizationService, overlayService, umbRequestHelper, devModeService) {

        if ($scope.model.hasOwnProperty("contentTypeId")) {
            // NOTE: This will prevents the editor attempting to load whilst in the Content Type Editor's property preview panel.
            return;
        }

        //console.log("data-picker", $scope.model);

        var defaultConfig = {
            addButtonLabelKey: "general_add",
            defaultIcon: "icon-science",
            defaultValue: [],
            displayMode: "cards",
            enableDevMode: 0,
            hideSearch: 0,
            maxItems: 0,
            overlaySize: "medium",
            overlayView: "",
            pageSize: 12,
        };
        var config = Object.assign({}, defaultConfig, $scope.model.config);

        var vm = this;

        function init() {

            // NOTE: [LK] Adds context of the current page, for potential future data-sources.
            // If the page is new, then it doesn't have an id, so the parentId will be used.
            config.currentPage = $scope.node || editorState.getCurrent();
            config.currentPageId = config.currentPage.id > 0 ? config.currentPage.id : config.currentPage.parentId;

            // Support Content not in Content / Media Tree
            if (!config.currentPageId) {
                config.currentPageId = -1;
            }

            ensureValueIsArray();

            if (Array.isArray(config.displayMode) === true) {
                config.displayMode = config.displayMode[0];
            }

            if (Number.isInteger(config.maxItems) === false) {
                config.maxItems = Number.parseInt(config.maxItems) || defaultConfig.maxItems;
            }

            if (Array.isArray(config.overlaySize) === true) {
                config.overlaySize = config.overlaySize[0];
            }

            vm.itemLookup = {};
            vm.loading = false;

            config.missingItem = {
                cardStyle: { "background-color": "#f0ac00" },
                description: "This item is no longer available.",
                icon: "icon-alert",
            };

            load();

            vm.allowAdd = config.maxItems === 0 || $scope.model.value.length < config.maxItems;
            vm.allowEdit = false;
            vm.allowRemove = true;
            vm.allowSort = config.maxItems !== 1;

            vm.addButtonLabelKey = config.addButtonLabelKey || "general_add";
            vm.defaultIcon = config.defaultIcon || "icon-science";
            vm.displayMode = config.displayMode || "cards";

            vm.add = add;
            vm.populate = populate;
            vm.remove = remove;

            vm.uniqueId = $scope.model.hasOwnProperty("dataTypeKey")
                ? [$scope.model.alias, $scope.model.dataTypeKey.substring(0, 8)].join("-")
                : $scope.model.alias;

            vm.propertyActions = [];

            if (Object.toBoolean(config.enableDevMode)) {
                vm.propertyActions.push({
                    labelKey: "contentment_editRawValue",
                    icon: "brackets",
                    method: () => devModeService.editValue($scope.model, validate)
                });
            }
        };

        function add() {
            editorService.open({
                view: config.overlayView,
                size: config.overlaySize || "medium",
                config: {
                    currentPageId: config.currentPageId,
                    dataTypeKey: $scope.model.dataTypeKey,
                    enableMultiple: config.maxItems !== 1,
                    hideSearch: Object.toBoolean(config.hideSearch),
                    listType: config.displayMode,
                    pageSize: config.pageSize,
                },
                submit: function (selection) {

                    ensureValueIsArray();

                    if (selection) {
                        Object.entries(selection).forEach(item => {
                            vm.itemLookup[item[0]] = item[1];
                            $scope.model.value.push(item[0]);
                        });
                    }

                    if (config.maxItems !== 0 && $scope.model.value.length >= config.maxItems) {
                        vm.allowAdd = false;
                    }

                    setDirty();

                    editorService.close();
                },
                close: function () {
                    editorService.close();
                }
            });
        };

        function ensureValueIsArray() {
            $scope.model.value = $scope.model.value || config.defaultValue;

            if (Array.isArray($scope.model.value) === false) {
                $scope.model.value = [$scope.model.value];
            }
        }

        function load() {
            if ($scope.model.value.length) {
                vm.loading = true;
                umbRequestHelper.resourcePromise(
                    $http.post("backoffice/Contentment/DataPickerApi/GetItems?id=" + config.currentPageId, $scope.model.value, {
                        params: { dataTypeKey: $scope.model.dataTypeKey }
                    }),
                    "Failed to retrieve item data.")
                    .then(function (data) {
                        vm.itemLookup = data;
                        vm.loading = false;
                    });
            }
        };

        function populate(key, $index, propertyName) {
            if (vm.itemLookup.hasOwnProperty(key) === false) {
                return propertyName === "name" ? key : config.missingItem[propertyName] || null;
            } else {
                if (propertyName === "cardStyle") {
                    var imageUrl = vm.itemLookup[key]["image"];
                    return imageUrl
                        ? { "background-image": `url('${imageUrl}')` }
                        : null;
                } else if (propertyName === "iconStyle") {
                    return null;
                } else if (propertyName === "icon") {
                    return vm.displayMode === "cards" && vm.itemLookup[key]["image"]
                        ? null
                        : vm.itemLookup[key][propertyName] || vm.defaultIcon;
                }
                return vm.itemLookup[key][propertyName] || key;
            }
        };

        function remove($index) {
            var keys = ["contentment_removeItemMessage", "general_remove", "general_cancel", "contentment_removeItemButton"];
            localizationService.localizeMany(keys).then(function (data) {
                overlayService.open({
                    title: data[1],
                    content: data[0],
                    closeButtonLabel: data[2],
                    submitButtonLabel: data[3],
                    submitButtonStyle: "danger",
                    submit: function () {

                        $scope.model.value.splice($index, 1);

                        if (config.maxItems === 0 || $scope.model.value.length < config.maxItems) {
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

        function setDirty() {
            if ($scope.propertyForm) {
                $scope.propertyForm.$setDirty();
            }
        };

        function validate() {
            // NOTE: Any future validation can be done here.
            load();
            setDirty();
        };

        init();
    }
]);
