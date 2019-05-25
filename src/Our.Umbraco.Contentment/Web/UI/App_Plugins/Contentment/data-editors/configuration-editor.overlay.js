/* Copyright © 2019 Lee Kelleher, Umbrella Inc and other contributors.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

angular.module("umbraco").controller("Our.Umbraco.Contentment.Overlays.ConfigurationEditor.Controller", [
    "$scope",
    "formHelper",
    function ($scope, formHelper) {

        //console.log("config-editor-overlay.model", $scope.model);

        var defaultConfig = { items: [], overlaySize: "large", enableSearch: 0 };
        var config = angular.extend({}, defaultConfig, $scope.model.config);

        var vm = this;

        function init() {

            vm.selectedItem = $scope.model.value || { type: "", name: "", icon: "", value: {} };

            if (_.isEmpty(vm.selectedItem.type)) {

                vm.title = "Select...";
                vm.mode = "select";
                vm.items = config.items;
                vm.enableSearch = Object.toBoolean(config.enableSearch);

            } else {
                vm.title = "Configure";
                vm.mode = "edit";

                vm.editor = _.find(config.items, function (x) {
                    return x.type === vm.selectedItem.type;
                });

                if (!vm.editor) {
                    // TODO: What to do if we don't find the config? [LK]
                    console.log("Unable to find error:", vm.selectedItem.type)
                }

                if (vm.editor.fields.length > 0) {
                    _.each(vm.editor.fields, function (x) {
                        x.alias = x.key;
                        x.value = vm.selectedItem.value[x.key];
                    });
                }
            }

            vm.close = close;
            vm.save = save;
            vm.select = select;

        };

        function select(item) {

            if (item.fields.length == 0) {

                // If there are no fields, then we can save & close the overlay
                save(item);

            } else {

                vm.title = item.name;
                vm.mode = "edit";
                // TODO: Check if we already know the size of the overlay, then we can check if it's different before we set it. [LK]
                $scope.model.size = config.overlaySize;
                vm.editor = item;

            }
        };

        function close() {
            if ($scope.model.close) {
                $scope.model.close();
            }
        };

        function save(item) {

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
