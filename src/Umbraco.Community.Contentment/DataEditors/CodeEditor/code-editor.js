/* Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

angular.module("umbraco").controller("Umbraco.Community.Contentment.DataEditors.CodeEditor.Controller", [
    "$scope",
    function ($scope) {

        // console.log("code-editor.model", $scope.model);

        var defaultConfig = {
            showGutter: 1,
            useWrapMode: 1,
            showInvisibles: 0,
            showIndentGuides: 0,
            useSoftTabs: 1,
            showPrintMargin: 0,
            disableSearch: 0,
            theme: "chrome",
            mode: "javascript",
            firstLineNumber: 1,
            fontSize: "small",
            enableSnippets: 0,
            enableBasicAutocompletion: 0,
            enableLiveAutocompletion: 0,
            readonly: 0,
            minLines: undefined,
            maxLines: undefined,
        };
        var config = Object.assign({}, defaultConfig, $scope.model.config);

        var vm = this;

        function init() {

            vm.cssClasses = !$scope.model.labelOnTop ? ["umb-property-editor--limit-width"] : [];

            vm.readonly = Object.toBoolean(config.readonly);

            vm.options = {
                autoFocus: false,
                showGutter: Object.toBoolean(config.showGutter),
                useWrapMode: Object.toBoolean(config.useWrapMode),
                showInvisibles: Object.toBoolean(config.showInvisibles),
                showIndentGuides: Object.toBoolean(config.showIndentGuides),
                useSoftTabs: Object.toBoolean(config.useSoftTabs),
                showPrintMargin: Object.toBoolean(config.showPrintMargin),
                disableSearch: Object.toBoolean(config.disableSearch),
                theme: config.theme,
                mode: config.mode,
                firstLineNumber: config.firstLineNumber,
                advanced: {
                    fontSize: config.fontSize,
                    enableSnippets: Object.toBoolean(config.enableSnippets),
                    enableBasicAutocompletion: Object.toBoolean(config.enableBasicAutocompletion),
                    enableLiveAutocompletion: Object.toBoolean(config.enableLiveAutocompletion),
                    minLines: config.minLines,
                    maxLines: config.maxLines,
                    wrap: Object.toBoolean(config.useWrapMode)
                },
                //rendererOptions: {},
                //onLoad: function (aceEditor) {
                //    console.log("ace.onload", aceEditor);
                //}
            };

        };

        init();
    }
]);
