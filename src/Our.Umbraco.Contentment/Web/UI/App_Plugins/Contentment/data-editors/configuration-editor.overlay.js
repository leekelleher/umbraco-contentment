/* Copyright © 2019 Lee Kelleher, Umbrella Inc and other contributors.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

angular.module("umbraco").controller("Our.Umbraco.Contentment.Overlays.ConfigurationEditor.Controller", [
    "$scope",
    "formHelper",
    function ($scope, formHelper) {

        // console.log("config-editor-overlay.model", $scope.model);

        var defaultConfig = { mode: "select", items: [], editor: null, overlaySize: "large", enableFilter: 0, orderBy: "name" };
        var config = angular.extend({}, defaultConfig, $scope.model.config);

        var vm = this;

        function init() {

            vm.mode = config.mode;

            if (vm.mode === "select") {

                vm.title = "Select...";
                vm.items = config.items;
                vm.enableFilter = Object.toBoolean(config.enableFilter);
                vm.orderBy = config.orderBy;
                vm.select = select;

            } else if (vm.mode === "edit" && config.editor) {

                var item = $scope.model.value || { type: "", name: "", icon: "", value: {} };
                edit(config.editor, item);

            }

            vm.close = close;
        };

        function edit(editor, item) {

            if ($scope.model.size !== config.overlaySize) {
                $scope.model.size = config.overlaySize;
            }

            vm.title = "Configure " + editor.name;
            // TODO: we could add the icon and description?!

            vm.editor = angular.copy(editor);

            if (vm.editor.fields.length > 0) {
                _.each(vm.editor.fields, function (x) {
                    x.alias = x.key;
                    x.value = item.value[x.key];
                });
            }

            vm.save = save;
        };

        function select(editor) {
            // If there are no fields, then we can save & close the overlay
            if (editor.fields.length == 0) {
                save(editor);
            } else {
                vm.mode = "edit";
                edit(editor, { value: {} });
            }
        };

        function close() {
            if ($scope.model.close) {
                $scope.model.close();
            }
        };

        function save(item) {

            // TODO: Check if the Save button is displaying on the 'select' mode.

            // TODO: Not sure if we need to use `formHelper.submitForm` here? e.g. `formHelper.submitForm({ scope: $scope, formCtrl: this.configurationEditorForm })`
            // https://github.com/umbraco/Umbraco-CMS/blob/release-8.0.2/src/Umbraco.Web.UI.Client/src/common/services/formhelper.service.js#L26
            $scope.$broadcast("formSubmitting", { scope: $scope });

            var obj = {
                type: item.type,
                name: item.name,
                icon: item.icon,
                value: {}
            };

            _.each(item.fields, function (x) {
                obj.value[x.key] = x.value;
            });

            if ($scope.model.submit) {
                $scope.model.submit(obj);
            }
        };

        init();
    }
]);
