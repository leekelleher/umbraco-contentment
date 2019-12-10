/* Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

angular.module("umbraco").controller("Umbraco.Community.Contentment.DataEditors.DropdownList.Controller", [
    "$scope",
    function ($scope) {

        //console.log("dropdown-list.model", $scope.model);

        var defaultConfig = {
            items: [],
            allowEmpty: 1,
            defaultValue: "",
            htmlAttributes: []
        };
        var config = angular.extend({}, defaultConfig, $scope.model.config);

        var vm = this;

        function init() {
            $scope.model.value = $scope.model.value || config.defaultValue;

            if (_.isArray($scope.model.value)) {
                $scope.model.value = _.first($scope.model.value);
            }

            vm.items = angular.copy(config.items);

            vm.allowEmpty = Object.toBoolean(config.allowEmpty);

            vm.uniqueId = [$scope.model.alias, $scope.model.dataTypeKey.substring(0, 8)].join("-");
            vm.htmlAttributes = config.htmlAttributes;
        };

        init();
    }
]);
