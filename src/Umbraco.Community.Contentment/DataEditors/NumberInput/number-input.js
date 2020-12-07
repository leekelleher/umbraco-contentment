/* Copyright © 2020 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

angular.module("umbraco").controller("Umbraco.Community.Contentment.DataEditors.NumberInput.Controller", [
    "$scope",
    function ($scope) {

        console.log("number-input.model", $scope.model);

        var defaultConfig = {
            placeholderText: null,
            defaultValue: null,
            maximum: null,
            minimum: null,
            sizeClass: "umb-property-editor-tiny",
            step: null,
            umbracoDataValueType: "INT",
        };
        var config = Object.assign({}, defaultConfig, $scope.model.config);

        var vm = this;

        function init() {

            $scope.model.value = $scope.model.value || config.defaultValue;

            vm.maximum = config.maximum;
            vm.minimum = config.minimum;

            vm.pattern = config.umbracoDataValueType === "DECIMAL"
                ? "[\-0-9]+([,\.][0-9]+)?"
                : "[\-0-9]*";

            vm.placeholderText = config.placeholderText;
            vm.sizeClass = config.sizeClass;
            vm.step = config.step;

            vm.uniqueId = $scope.model.hasOwnProperty("dataTypeKey")
                ? [$scope.model.alias, $scope.model.dataTypeKey.substring(0, 8)].join("-")
                : $scope.model.alias;
        };

        init();
    }
]);
