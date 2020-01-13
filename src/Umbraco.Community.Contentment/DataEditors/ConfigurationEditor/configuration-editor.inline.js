/* Copyright © 2020 Lee Kelleher.
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
        var config = angular.extend({}, defaultConfig, $scope.model.config);

        var vm = this;

        function init() {

            vm.editor = angular.copy(config.editor);

            $scope.model.value = $scope.model.value || { type: vm.editor.type, value: vm.editor.defaultValues || {} };

            if ($scope.model.value.type !== vm.editor.type) {
                // TODO: [LK:2020-01-07] What to do if it's a different type?
                console.log("[LK] type is different", $scope.model.value.type, vm.editor.type);

                $scope.model.value.type = vm.editor.type;
            }

            if (vm.editor.fields && vm.editor.fields.length > 0) {
                _.each(vm.editor.fields, function (x) {
                    x.alias = x.key;

                    if (_.has($scope.model.value.value, x.key)) {
                        x.value = $scope.model.value.value[x.key];
                    }
                });
            }

            $scope.$on("formSubmitting", function (ev, args) {
                if (vm.editor.fields && vm.editor.fields.length > 0) {
                    _.each(vm.editor.fields, function (x) {
                        $scope.model.value.value[x.key] = x.value;
                    });
                }
            });
        };

        init();
    }
]);
