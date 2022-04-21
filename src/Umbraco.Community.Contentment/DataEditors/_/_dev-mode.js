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
                            $timeout(() => _editor.focus());
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

angular.module("umbraco.directives").component("leeWasHere", {
    template: "<img ng-src=\"data:image/png;base64,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\" alt=\"Lee was here\">"
});
