/* Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

angular.module("umbraco").controller("Umbraco.Community.Contentment.DataEditors.TextInput.Controller", [
    "$scope",
    function ($scope) {

        // console.log("text-input.model", $scope.model);

        var defaultConfig = {
            items: [],
            autocomplete: 0,
            placeholderText: null,
            defaultValue: null,
            prepend: null,
            append: null,
            maxChars: 500
        };
        var config = Object.assign({}, defaultConfig, $scope.model.config);

        var vm = this;

        function init() {

            $scope.model.value = $scope.model.value || config.defaultValue;

            if (Array.isArray($scope.model.value)) {
                $scope.model.value = $scope.model.value.join(", ");
            }

            vm.autoComplete = Object.toBoolean(config.autocomplete) ? "on" : "off";
            vm.placeholderText = config.placeholderText;
            vm.maxChars = parseInt(0 + config.maxChars);
            vm.maxCharsThreshold = vm.maxChars * .8;

            vm.prepend = config.prepend;
            vm.append = config.append;

            vm.uniqueId = $scope.model.hasOwnProperty("dataTypeKey")
                ? [$scope.model.alias, $scope.model.dataTypeKey.substring(0, 8)].join("-")
                : $scope.model.alias;

            if (config.items && config.items.length > 0) {

                vm.dataList = config.items;
                vm.dataListId = "dl-" + vm.uniqueId;
            }
        };

        init();
    }
]);
