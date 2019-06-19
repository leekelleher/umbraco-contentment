/* Copyright © 2019 Lee Kelleher, Umbrella Inc and other contributors.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

angular.module("umbraco.directives").directive("contentmentDevMode", [
    "editorService",
    function (editorService) {
        return {
            restrict: "E",
            require: "ngModel",
            scope: {
                onUpdate: "&?",
            },
            link: function (scope, element, attrs, ngModelCtrl) {
                scope.open = function () {
                    editorService.open({
                        title: "Edit...",
                        value: angular.toJson(ngModelCtrl.$modelValue, true),
                        ace: {
                            showGutter: true,
                            useWrapMode: true,
                            useSoftTabs: true,
                            theme: "chrome",
                            mode: "javascript",
                            advanced: {
                                fontSize: "14px",
                                wrap: true
                            },
                        },
                        view: "/App_Plugins/Contentment/editors/_json-editor.html",
                        size: "small",
                        submit: function (value) {

                            ngModelCtrl.$setViewValue(angular.fromJson(value));
                            ngModelCtrl.$render();

                            if (scope.onUpdate) {
                                scope.onUpdate();
                            }

                            editorService.close();
                        },
                        close: function () {
                            editorService.close();
                        }
                    });
                };
            },
            templateUrl: "/App_Plugins/Contentment/editors/_dev-mode.html",
        };
    }
]);
