/* Copyright © 2020 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

angular.module("umbraco").controller("Umbraco.Community.Contentment.DataEditors.Notes.Controller", [
    "$scope",
    "Umbraco.Community.Contentment.Services.DevMode",
    function ($scope, devModeService) {

        //console.log("notes.model", $scope.model);

        var defaultConfig = {
            enableDevMode: 0,
            hidePropertyGroup: 0
        };
        var config = Object.assign({}, defaultConfig, $scope.model.config);

        var vm = this;

        function init() {

            if (Object.toBoolean(config.enableDevMode) === true && $scope.umbProperty) {
                $scope.umbProperty.setPropertyActions([{
                    labelKey: "contentment_editRawValue",
                    icon: "brackets",
                    method: function () {
                        devModeService.editValue($scope.model, setDirty);
                    }
                }]);
            }

            // NOTE: If previewing in the Content Type Editor's property preview panel,
            // we'll ignore the hide property group container.
            vm.hidePropertyGroup = $scope.model.hasOwnProperty("contentTypeId") === false
                && Object.toBoolean(config.hidePropertyGroup);
        };

        function setDirty() {
            if ($scope.propertyForm) {
                $scope.propertyForm.$setDirty();
            }
        };

        init();
    }
]);
