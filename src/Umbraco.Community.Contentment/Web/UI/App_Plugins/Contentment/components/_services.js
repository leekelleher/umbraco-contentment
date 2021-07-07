/* Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

angular.module("umbraco.services").factory("Umbraco.Community.Contentment.Services.DevMode", [
    "$timeout",
    "editorService",
    function ($timeout, editorService) {
        return {
            editValue: function (model, callback) {
                editorService.open({
                    title: "Edit raw value",
                    value: Utilities.toJson(model.value, true),
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
                        onLoad: function (_editor) {
                            $timeout(function () {
                                _editor.focus();
                            });
                        },
                    },
                    view: "/App_Plugins/Contentment/editors/_json-editor.html",
                    size: "medium",
                    submit: function (value) {

                        model.value = Utilities.fromJson(value);

                        if (callback) {
                            callback();
                        }

                        editorService.close();
                    },
                    close: function () {
                        editorService.close();
                    }
                });
            }
        }
    }
]);


