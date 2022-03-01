/* Copyright © 2022 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

angular.module("umbraco").controller("Umbraco.Community.Contentment.DataEditors.EditorNotes.Controller", [
    "$scope",
    function ($scope) {

        //console.log("editor-notes.model", $scope.model);

        var defaultConfig = {
            hidePropertyGroup: 0
        };
        var config = Object.assign({}, defaultConfig, $scope.model.config);

        var vm = this;

        function init() {

            vm.alertType = config.alertType;
            vm.icon = config.icon;
            vm.heading = config.heading;
            vm.message = config.message;

            // NOTE: If previewing in the Content Type Editor's property preview panel,
            // we'll ignore the hide property group container.
            vm.hidePropertyGroup = $scope.model.hasOwnProperty("contentTypeId") === false
                && Object.toBoolean(config.hidePropertyGroup);
        };

        init();
    }
]);
