/* Copyright © 2022 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

angular.module("umbraco").controller("Umbraco.Community.Contentment.DataEditors.ConfigurationEditorInline.Controller", [
    "$scope",
    "formHelper",
    function ($scope, formHelper) {

        // console.log("config-editor-inline.model", $scope.model);

        var defaultConfig = {
            editor: {}
        };
        var config = Object.assign({}, defaultConfig, $scope.model.config);

        var vm = this;

        function init() {

            vm.editor = Object.assign({}, config.editor);

            $scope.model.value = $scope.model.value || { key: vm.editor.key, value: vm.editor.defaultValues || {} };

            if ($scope.model.value.key !== vm.editor.key) {
                $scope.model.value.key = vm.editor.key;
            }

            if (vm.editor.fields && vm.editor.fields.length > 0) {
                vm.editor.fields.forEach(x => {
                    x.alias = x.key;
                    if ($scope.model.value.value.hasOwnProperty(x.key)) {
                        x.value = $scope.model.value.value[x.key];
                    }
                });
            }

            $scope.$on("formSubmitting", function (ev, args) {
                if (vm.editor.fields && vm.editor.fields.length > 0) {
                    vm.editor.fields.forEach(x => {
                        $scope.model.value.value[x.key] = x.value;
                    });
                }
            });
        };

        init();
    }
]);
