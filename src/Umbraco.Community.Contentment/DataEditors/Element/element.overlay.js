/* Copyright © 2019 Lee Kelleher, Umbrella Inc and other contributors.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

angular.module("umbraco").controller("Umbraco.Community.Contentment.Overlays.Element.Controller", [
    "$q",
    "$scope",
    "blueprintConfig",
    "contentResource",
    "versionHelper",
    function ($q, $scope, blueprintConfig, contentResource, versionHelper) {

        // console.log("element-overlay.model", $scope.model, blueprintConfig);

        var defaultConfig = {
            defaultAppAlias: "umbContent",
            elementType: null,
            elementTypes: [],
            editOverlaySize: "large",
        };
        var config = angular.extend({}, defaultConfig, $scope.model.config);

        var vm = this;

        function init() {

            vm.contentId = -2;

            vm.save = save;
            vm.submit = submit;
            vm.close = close;

            vm.infiniteModel = {
                infiniteMode: true,
                allowPublishAndClose: false,
                allowSaveAndClose: true,
                contentNode: {},
                submit: vm.submit,
                close: vm.close
            };

            if (config.elementType && $scope.model.value) {

                vm.mode = "edit";

                edit(config.elementType, $scope.model.value);

            } else {

                vm.mode = "select";
                vm.title = "Select an element type...";
                vm.items = config.elementTypes;
                vm.selectBlueprint = false;
                vm.enableFilter = true;
                vm.orderBy = "name";

                vm.selectedElementType = null;

                vm.select = select;
                vm.create = create;
            }
        };

        function select(elementType) {
            if (elementType.blueprints && elementType.blueprints.length > 0) {
                if (elementType.blueprints.length === 1 && blueprintConfig.skipSelect) {
                    create(elementType, elementType.blueprints[0]);
                }
                else {
                    // TODO: [LK:2019-08-30] Add a cancel/back button, just in case someone made a mistake (picking the wrong element type).
                    vm.title = "Select a content blueprint...";
                    vm.selectBlueprint = true;
                    vm.selectedElementType = elementType;
                    vm.blueprintAllowBlank = blueprintConfig.allowBlank;
                }
            } else {
                create(elementType);
            }
        };

        function create(elementType, blueprint) {

            $scope.model.size = config.editOverlaySize;
            vm.mode = "edit";

            vm.isNew = true;

            vm.getScaffoldMethod = function () {

                var getScaffold = blueprint && blueprint.id > 0
                    ? contentResource.getBlueprintScaffold(vm.contentId, blueprint.id)
                    : contentResource.getScaffold(vm.contentId, elementType.alias);

                return getScaffold.then(function (data) {

                    if (data.apps.length > 1) {
                        data.apps = _.filter(data.apps, function (x) { return x.alias === config.defaultAppAlias });
                    }

                    return data;
                });
            };
        };

        function edit(elementType, element) {

            vm.isNew = false;

            vm.getMethod = function (contentId) {
                return contentResource.getScaffold(vm.contentId, elementType.alias).then(function (data) {

                    if (data.apps.length > 1) {
                        data.apps = _.filter(data.apps, function (x) { return x.alias === config.defaultAppAlias });
                    }

                    data.key = element.key;
                    data.udi = element.udi;
                    data.variants[0].name = element.name;

                    if (element.value) {
                        for (var t = 0; t < data.variants[0].tabs.length; t++) {
                            var tab = data.variants[0].tabs[t];
                            for (var p = 0; p < tab.properties.length; p++) {
                                var property = tab.properties[p];
                                if (element.value.hasOwnProperty(property.alias)) {
                                    property.value = element.value[property.alias];
                                }
                            }
                        }
                    }

                    return data;
                });
            };
        };

        function save(content, create, files, showNotifications) {
            // NOTE: This function serves one purpose, to provide a promise to the <content-editor>'s `performSave` call.
            return $q(function (resolve, reject) {
                resolve(content);

                // TODO: [LK:2019-08-31] Once `allowInfiniteSaveAndClose` is fixed (in Umb v8.2), I can remove this line.
                if (versionHelper.versionCompare(Umbraco.Sys.ServerVariables.application.version, "8.2.0") < 0) {
                    submit({ contentNode: content });
                }
            });
        };

        function submit(model) {
            if ($scope.model.submit && model.contentNode) {

                var guid = String.CreateGuid();

                var item = {
                    elementType: model.contentNode.documentType.key,
                    key: guid,
                    udi: "umb://document/" + guid.replace(/-/g, ""), // NOTE: [LK:2019-09-13] I would have liked the UDI to be `umb://element/{GUID}`
                    name: model.contentNode.variants[0].name,
                    icon: model.contentNode.icon,
                    value: {},
                };

                if (model.contentNode.variants.length > 0) {
                    for (var t = 0; t < model.contentNode.variants[0].tabs.length; t++) {
                        var tab = model.contentNode.variants[0].tabs[t];
                        for (var p = 0; p < tab.properties.length; p++) {
                            var property = tab.properties[p];
                            if (typeof property.value !== "function") {
                                item.value[property.alias] = property.value;
                            }
                        }
                    }
                }

                $scope.model.submit(item);
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
