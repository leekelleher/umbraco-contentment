/* Copyright © 2019 Lee Kelleher, Umbrella Inc and other contributors.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

angular.module("umbraco").controller("Umbraco.Community.Contentment.DataEditors.Dimensions.Controller", [
    "$scope",
    function ($scope) {

        // console.log("dimensions.model", $scope.model);

        var vm = this;

        function init() {
            $scope.model.value = $scope.model.value || { width: null, height: null };

            vm.labelWidth = "Width"; // TODO: [LK:2019-11-25] Use localized label "general_width"
            vm.labelHeight = "Height"; // TODO: [LK:2019-11-25] Use localized label "general_height"
            vm.labelPixels = "Pixels"; // There doesn't appear to be an existing label for "pixel". Maybe a config option?
        };

        init();
    }
]);
