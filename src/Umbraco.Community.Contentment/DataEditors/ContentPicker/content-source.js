/* Copyright © 2020 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

angular.module("umbraco").controller("Umbraco.Community.Contentment.DataEditors.ContentSource.Controller", [
    "$scope",
    "$controller",
    "$q",
    "editorService",
    "entityResource",
    "localizationService",
    function ($scope, $controller, $q, editorService, entityResource, localizationService) {

        //console.log("content-source.model", $scope.model);

        var config = Object.assign({}, $scope.model.config);

        var vm = this;

        function init() {

            vm.type = "select";

            vm.dynamicRoot = null;
            vm.node = null;

            if (typeof $scope.model.value === 'string' && $scope.model.value.startsWith("umb://document/")) {
                vm.type = "picker";
                vm.node = {};
                vm.loading = true;
                entityResource.getById($scope.model.value, "Document").then(item => {
                    setPickerModel(item);
                    vm.loading = false;
                })
            } else if (typeof $scope.model.value === 'object' && $scope.model.value.hasOwnProperty("originAlias")) {
                vm.type = "dynamicRoot";
                setDynamicRootModel();
            }
            else {
                vm.type = "xpath";
            }

            vm.buttonGroup = {
                defaultButton: { labelKey: "contentPicker_defineRootNode", handler: openPicker },
                subButtons: [
                    { labelKey: "contentPicker_defineDynamicRoot", handler: openOrigin },
                    { labelKey: "contentPicker_defineXPathOrigin", handler: openXPath },
                ]
            };

            vm.sortableOptions = {
                axis: "y",
                containment: "parent",
                cursor: "move",
                disabled: vm.allowSort === false,
                opacity: 0.7,
                scroll: true,
                tolerance: "pointer",
                update: function (e, ui) {
                    setDynamicRootModel();
                }
            };

            vm.clearValue = clearValue;
            vm.openOrigin = openOrigin;
            vm.openQueryStep = openQueryStep;
            vm.removeQueryStep = removeQueryStep;
            vm.showXPathHelp = false;
            vm.toggleXPathHelp = toggleXPathHelp;
        };

        function clearValue() {
            vm.type = "select";
            $scope.model.value = null;
            vm.node = null;
            vm.dynamicRoot = null;
        };

        function getIconForOriginAlias(originAlias) {
            switch (originAlias) {
                case "Root":
                    return "icon-home";
                case "Parent":
                    return "icon-page-up";
                case "Current":
                    return "icon-document";
                case "Site":
                    return "icon-home";
                case "ByKey":
                    return "icon-wand";
            }
        }

        function getIconForQueryStep(queryStep) {
            switch (queryStep.alias) {
                case "NearestAncestorOrSelf":
                    return "icon-chevron-up";
                case "FurthestAncestorOrSelf":
                    return "icon-chevron-up";
                case "NearestDescendantOrSelf":
                    return "icon-chevron-down";
                case "FurthestDescendantOrSelf":
                    return "icon-chevron-down";
            }
            return "icon-lab";
        }

        function openPicker() {
            editorService.treePicker({
                idType: "udi",
                section: "content",
                treeAlias: "content",
                multiPicker: false,
                submit: function (model) {
                    vm.type = "picker";
                    var item = model.selection[0];
                    setPickerModel(item);
                    $scope.model.value = item.udi;
                    editorService.close();
                },
                close: function () {
                    editorService.close();
                }
            });
        };

        function openOrigin() {
            editorService.open({
                view: "views/common/infiniteeditors/pickdynamicrootorigin/pickdynamicrootorigin.html",
                contentType: "content",
                size: "small",
                value: {},
                multiPicker: false,
                submit: function (model) {
                    vm.type = "dynamicRoot";
                    $scope.model.value = Object.assign({}, $scope.model.value, model.value);
                    setDynamicRootModel();
                    editorService.close();
                },
                close: function () {
                    editorService.close();
                }
            });
        };

        function openQueryStep() {
            editorService.open({
                view: "views/common/infiniteeditors/pickdynamicrootquerystep/pickdynamicrootquerystep.html",
                contentType: "content",
                size: "small",
                multiPicker: false,
                submit: function (model) {
                    if (!$scope.model.value.querySteps) {
                        $scope.model.value.querySteps = [];
                    }
                    $scope.model.value.querySteps.push(model.value);
                    setDynamicRootModel();
                    editorService.close();
                },
                close: function () {
                    editorService.close();
                }
            });
        };

        function openXPath() {
            vm.type = "xpath";
        };

        function removeQueryStep($index, step) {
            if ($index !== -1 && $scope.model.value.querySteps) {
                $scope.model.value.querySteps.splice($index, 1);
                setDynamicRootModel()
            }
        };

        function setDynamicRootModel() {
            vm.dynamicRoot = {
                origin: { icon: null, name: null, description: null },
                steps: [],
            };

            const getDataForOrigin = (data) => {
                const key = `dynamicRoot_origin${data.originAlias}Title`;
                localizationService.localize(key).then(name => {
                    vm.dynamicRoot.origin.name = name;
                });

                if (data.originKey) {
                    entityResource.getById(data.originKey, "document").then(entity => {
                        vm.dynamicRoot.origin.description = entity.name;
                    });
                }

                vm.dynamicRoot.origin.icon = getIconForOriginAlias(data.originAlias);
            };

            const getDataForQueryStep = (queryStep) => {
                const icon = getIconForQueryStep(queryStep);
                const keys = [`dynamicRoot_queryStep${queryStep.alias}Title`, "dynamicRoot_queryStepTypes"];
                localizationService.localizeMany(keys).then(values => {
                    const name = values[0];
                    const description = queryStep.anyOfDocTypeKeys && queryStep.anyOfDocTypeKeys.length > 0
                        ? (values[1] || "That matches types: ") + queryStep.anyOfDocTypeKeys.join(", ")
                        : null;
                    vm.dynamicRoot.steps.push({ icon, name, description });
                });
            };

            const promises = [];

            promises.push(getDataForOrigin($scope.model.value));

            if ($scope.model.value.querySteps) {
                $scope.model.value.querySteps.forEach(x => promises.push(getDataForQueryStep(x)));
            }

            $q.all(promises);
        };

        function setPickerModel(item) {
            vm.node = item;
            entityResource.getUrl(item.id, "Document").then(data => {
                vm.node.path = data;
            });
        };

        function toggleXPathHelp() {
            vm.showXPathHelp = !vm.showXPathHelp;
        };

        init();
    }
]);
