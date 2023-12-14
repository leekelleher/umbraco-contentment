/* Copyright © 2013-present Umbraco.
 * Parts of this Source Code have been derived from Umbraco CMS.
 * https://github.com/umbraco/Umbraco-CMS/blob/release-13.0.0-rc5/src/Umbraco.Web.UI.Client/src/views/propertyeditors/rte/rte.component.js
 * Modified under the permissions of the MIT License.
 * Modifications are licensed under the Mozilla Public License.
 * Copyright © 2030 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

angular.module("umbraco").controller("Umbraco.Community.Contentment.DataEditors.RichTextEditor.Controller", [
    "$element",
    "$scope",
    "$q",
    "$timeout",
    "assetsService",
    "tinyMceAssets",
    "tinyMceService",
    function ($element, $scope, $q, $timeout, assetsService, tinyMceAssets, tinyMceService) {

        if ($scope.model.hasOwnProperty("contentTypeId")) {
            // NOTE: This will prevents the editor attempting to load whilst in the Content Type Editor's property preview panel.
            return;
        }

        //console.log("rich-text-editor.model", $scope.model);

        var defaultConfig = {};
        var config = Object.assign({}, defaultConfig, $scope.model.config);

        var vm = this;

        function init() {

            vm.model = $scope.model;

            var unsubscribe = [];
            var modelObject;

            vm.readonly = false;
            vm.tinyMceEditor = null;

            vm.loading = true;
            vm.rteLoading = true;
            vm.updateLoading = function () {
                if (!vm.rteLoading) {
                    vm.loading = false;
                }
            }

            vm.$onInit = function () {

                // set the onValueChanged callback, this will tell us if the block list model changed on the server
                // once the data is submitted. If so we need to re-initialize
                vm.model.onValueChanged = onServerValueChanged;

                vm.listWrapperStyles = {};

                if (vm.model.config.maxPropertyWidth) {
                    vm.listWrapperStyles["max-width"] = vm.model.config.maxPropertyWidth;
                }


                // ******************** //
                // RTE PART:
                // ******************** //


                // To id the html textarea we need to use the datetime ticks because we can have multiple rte's per a single property alias
                // because now we have to support having 2x (maybe more at some stage) content editors being displayed at once. This is because
                // we have this mini content editor panel that can be launched with MNTP.
                vm.textAreaHtmlId = vm.model.alias + "_" + String.CreateGuid();

                var editorConfig = vm.model.config ? vm.model.config.editor : null;
                if (!editorConfig || Utilities.isString(editorConfig)) {
                    editorConfig = tinyMceService.defaultPrevalues();
                }

                var width = editorConfig.dimensions ? parseInt(editorConfig.dimensions.width, 10) || null : null;
                var height = editorConfig.dimensions ? parseInt(editorConfig.dimensions.height, 10) || null : null;

                vm.containerWidth = "auto";
                vm.containerHeight = "auto";
                vm.containerOverflow = "inherit";

                var promises = [];

                //queue file loading
                tinyMceAssets.forEach(function (tinyJsAsset) {
                    promises.push(assetsService.loadJs(tinyJsAsset, $scope));
                });

                promises.push(tinyMceService.getTinyMceEditorConfig({
                    htmlId: vm.textAreaHtmlId,
                    stylesheets: editorConfig.stylesheets,
                    toolbar: editorConfig.toolbar,
                    mode: editorConfig.mode
                }));

                //wait for queue to end
                $q.all(promises).then(function (result) {

                    var standardConfig = result[promises.length - 1];

                    if (height !== null) {
                        standardConfig.plugins.splice(standardConfig.plugins.indexOf("autoresize"), 1);
                    }

                    //create a baseline Config to extend upon
                    var baseLineConfigObj = {
                        maxImageSize: editorConfig.maxImageSize,
                        width: width,
                        height: height
                    };

                    baseLineConfigObj.setup = function (editor) {

                        //set the reference
                        vm.tinyMceEditor = editor;

                        vm.tinyMceEditor.on("init", function (e) {
                            $timeout(function () {
                                vm.rteLoading = false;
                                vm.updateLoading();
                            });
                        });
                        vm.tinyMceEditor.on("focus", function () {
                            $element[0].dispatchEvent(new CustomEvent("umb-rte-focus", { composed: true, bubbles: true }));
                        });
                        vm.tinyMceEditor.on("blur", function () {
                            $element[0].dispatchEvent(new CustomEvent("umb-rte-blur", { composed: true, bubbles: true }));
                        });

                        //initialize the standard editor functionality for Umbraco
                        tinyMceService.initializeEditor({
                            //scope: $scope,
                            editor: editor,
                            toolbar: editorConfig.toolbar,
                            model: vm.model,
                            getValue: function () {
                                return vm.model.value;
                            },
                            setValue: function (newVal) {
                                vm.model.value = newVal;
                                $scope.$evalAsync();
                            },
                            culture: null,
                            segment: null,
                            blockEditorApi: {},
                            parentForm: vm.propertyForm,
                            valFormManager: vm.valFormManager,
                            currentFormInput: $scope.rteForm.modelValue
                        });

                    };

                    Utilities.extend(baseLineConfigObj, standardConfig);

                    // Readonly mode
                    baseLineConfigObj.toolbar = vm.readonly ? false : baseLineConfigObj.toolbar;
                    baseLineConfigObj.readonly = vm.readonly ? 1 : baseLineConfigObj.readonly;

                    // We need to wait for DOM to have rendered before we can find the element by ID.
                    $timeout(function () {
                        tinymce.init(baseLineConfigObj);
                    }, 50);

                    //listen for formSubmitting event (the result is callback used to remove the event subscription)
                    unsubscribe.push($scope.$on("formSubmitting", function () {
                        if (vm.tinyMceEditor != null && !vm.rteLoading) {

                            // Remove Angular Classes from markup:
                            var parser = new DOMParser();
                            var doc = parser.parseFromString(vm.model.value, "text/html");

                            // Get all elements in the parsed document
                            var elements = doc.querySelectorAll("*[class]");
                            elements.forEach(element => {
                                var classAttribute = element.getAttribute("class");
                                if (classAttribute) {
                                    // Split the class attribute by spaces and remove "ng-scope" and "ng-isolate-scope"
                                    var classes = classAttribute.split(" ");
                                    var newClasses = classes.filter(function (className) {
                                        return className !== "ng-scope" && className !== "ng-isolate-scope";
                                    });

                                    // Update the class attribute with the remaining classes
                                    if (newClasses.length > 0) {
                                        element.setAttribute("class", newClasses.join(" "));
                                    } else {
                                        // If no remaining classes, remove the class attribute
                                        element.removeAttribute("class");
                                    }
                                }
                            });

                            vm.model.value = doc.body.innerHTML;

                        }
                    }));

                    vm.focusRTE = function () {
                        vm.tinyMceEditor.focus();
                    }

                    // When the element is disposed we need to unsubscribe!
                    // NOTE: this is very important otherwise if this is part of a modal, the listener still exists because the dom
                    // element might still be there even after the modal has been hidden.
                    $scope.$on("$destroy", function () {
                        if (vm.tinyMceEditor != null) {
                            if ($element && $element[0]) {
                                $element[0].dispatchEvent(new CustomEvent("blur", { composed: true, bubbles: true }));
                            }
                            vm.tinyMceEditor.destroy();
                            vm.tinyMceEditor = null;
                        }
                    });

                });

            };

            // Called when we save the value, the server may return an updated data and our value is re-synced
            // we need to deal with that here so that our model values are all in sync so we basically re-initialize.
            function onServerValueChanged(newVal, oldVal) {
                onLoaded();
            }

            function setDirty() {
                if (vm.propertyForm) {
                    vm.propertyForm.$setDirty();
                }
            }

            function onLoaded() {

                vm.updateLoading();

                $scope.$evalAsync();

            }

            $scope.$on("$destroy", function () {
                for (const subscription of unsubscribe) {
                    subscription();
                }
            });

        };

        init();
    }
]);
