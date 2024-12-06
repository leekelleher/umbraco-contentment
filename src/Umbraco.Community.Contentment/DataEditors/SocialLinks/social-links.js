/* Copyright © 2022 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

angular.module("umbraco").controller("Umbraco.Community.Contentment.DataEditors.SocialLinks.Controller", [
    "$scope",
    "localizationService",
    "overlayService",
    "Umbraco.Community.Contentment.Services.DevMode",
    function ($scope, localizationService, overlayService, devModeService) {

        //console.log("social-links.model", $scope.model);

        var defaultConfig = {
            confirmRemoval: 0,
            defaultIcon: "icon-add",
            defaultBackgroundColor: "#f6f4f4",
            defaultIconColor: "#9e9e9e",
            enableDevMode: 0,
            maxItems: 0,
            overlayView: "",
            networks: [],
        };
        var config = Object.assign({}, defaultConfig, $scope.model.config);

        var vm = this;

        function init() {

            $scope.model.value = $scope.model.value || [];

            if (Number.isInteger(config.maxItems) === false) {
                config.maxItems = Number.parseInt(config.maxItems) || defaultConfig.maxItems;
            }

            config.confirmRemoval = Object.toBoolean(config.confirmRemoval);

            vm.allowAdd = config.maxItems === 0 || $scope.model.value.length < config.maxItems;
            vm.focusIdx = -1;

            vm.sortableOptions = {
                axis: "y",
                containment: "parent",
                cursor: "move",
                opacity: 0.7,
                scroll: true,
                tolerance: "pointer",
                stop: function (e, ui) {
                    setDirty();
                }
            };

            config.lookup = _.indexBy(config.networks, "network");

            vm.add = add;
            vm.edit = edit;
            vm.getIcon = getIcon;
            vm.getStyle = getStyle;
            vm.pick = pick;
            vm.remove = remove;

            if (Object.toBoolean(config.enableDevMode) === true && $scope.umbProperty) {
                $scope.umbProperty.setPropertyActions([{
                    labelKey: "contentment_editRawValue",
                    icon: "brackets",
                    method: edit
                }, {
                    labelKey: "clipboard_labelForRemoveAllEntries",
                    icon: "trash",
                    method: function () {
                        $scope.model.value = [];
                    }
                }]);
            }
        };

        function add() {

            vm.focusIdx = -1;

            if (!$scope.model.value) {
                $scope.model.value = [];
            }

            $scope.model.value.push({
                network: "",
                name: "",
                url: "",
            });

            pick($scope.model.value.length - 1);

            if (config.maxItems !== 0 && $scope.model.value.length >= config.maxItems) {
                vm.allowAdd = false;
            }

            setDirty();
        };

        function edit() {
            devModeService.editValue($scope.model, function () {
                // NOTE: Any future validation can be done here.
                if (!$scope.model.value) {
                    $scope.model.value = [];
                }
            });
        };

        function getIcon(network) {

            var item = config.lookup[network];

            if (item && item.icon) {
                return item.icon;
            }

            return config.defaultIcon;
        }

        function getStyle(network) {
            var item = config.lookup[network];

            if (item && item.backgroundColor && item.iconColor) {
                return { backgroundColor: item.backgroundColor, color: item.iconColor, borderRadius: "5px", paddingTop: 0 };
            }

            return { backgroundColor: config.defaultBackgroundColor, color: config.defaultIconColor, borderRadius: "5px", borderStyle: "dashed", paddingTop: 0 };
        }

        function pick($index) {

            var item = $scope.model.value[$index];

            overlayService.open({
                title: "Select a social network...",
                config: { items: config.networks },
                position: "center",
                size: "small",
                view: config.overlayView,
                value: item,
                hideSubmitButton: true,
                submit: function (model) {

                    item.network = model.network;

                    if (item.name === '') {
                        item.name = model.name;
                    }

                    if (item.url === '') {
                        item.url = model.url;
                    }

                    vm.focusIdx = $index;

                    setDirty();

                    overlayService.close();
                },
                close: function () {
                    if (item.network === "") {
                        removeItem($index);
                    }
                    overlayService.close();
                }
            });
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

            $scope.model.value.splice($index, 1);

            if (config.maxItems === 0 || $scope.model.value.length < config.maxItems) {
                vm.allowAdd = true;
            }

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
