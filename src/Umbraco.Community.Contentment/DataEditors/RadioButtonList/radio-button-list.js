/* Copyright © 2019 Lee Kelleher, Umbrella Inc and other contributors.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

angular.module("umbraco").controller("Umbraco.Community.Contentment.DataEditors.RadioButtonList.Controller", [
    "$scope",
    function ($scope) {

        // console.log("radiobuttonlist.model", $scope.model);

        var defaultConfig = { items: [], orientation: "vertical", defaultValue: "" };
        var config = angular.extend({}, defaultConfig, $scope.model.config);

        var vm = this;

        function init() {
            $scope.model.value = $scope.model.value || config.defaultValue;

            if (_.isArray($scope.model.value)) {
                $scope.model.value = _.first($scope.model.value);
            }

            vm.items = config.items;
            vm.layout = config.orientation === "horizontal" ? "inline" : "";
        };

        init();
    }
]);
