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
                        title: "Edit JSON value...",
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
angular.module("umbraco.directives").directive("leeWasHere", [
    function () {
        console.log(`%c
                     888888888888
                   88            88
                 88                88
               88                    88
              88    88888    88888    88
             88    8  o  8  8  o  8    88
----8888---------------- 8  8 --------------8888-------
|                        8  8                         |
|                       8    8                        |
|   _____         _      8888 _                 _     |
|  |     |___ ___| |_ ___ ___| |_ _____ ___ ___| |_   |
|  |   --| . |   |  _| -_|   |  _|     | -_|   |  _|  |
|  |_____|___|_|_|_| |___|_|_|_| |_|_|_|___|_|_|_|    |
|                                                     |
-------------------------------------------------------`, "color: #3445b1; font-family: monospace; font-size: small;");
        return {
            restrict: "E",
            template: "<img src=\"data:image/webp;base64,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\" alt=\"Lee was here\">"
        };
    }
]);
