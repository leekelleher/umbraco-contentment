/* Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

angular.module("umbraco").controller("Umbraco.Community.Contentment.DataEditors.CascadingDropdownList.Controller", [
    "$scope",
    "$q",
    "$http",
    function ($scope, $q, $http) {

        if ($scope.model.hasOwnProperty("contentTypeId")) {
            // NOTE: This will prevents the editor attempting to load whilst in the Content Type Editor's property preview panel.
            return;
        }

        // console.log("cascading-dropdown-list.model", $scope.model);

        var defaultConfig = {
            apis: [],
            defaultValue: [""]
        };
        var config = Object.assign({}, defaultConfig, $scope.model.config);

        var vm = this;

        function init() {

            $scope.model.value = $scope.model.value || config.defaultValue;

            vm.dropdowns = [];

            if (config.apis.length > 0) {

                vm.loading = true;

                var chain = [];

                for (var i = 0; i < $scope.model.value.length; i++) {

                    var url = config.apis[i];
                    for (var j = 0; j < i; j++) {
                        url = url.replace("{" + j + "}", $scope.model.value[j]);
                    }

                    chain.push($http({ method: "GET", url: url }));
                }

                $q.all(chain).then(function (results) {

                    if (results) {
                        results.forEach(function (x, i) {
                            vm.dropdowns[i] = { options: x.data }
                        });
                    }

                    vm.loading = false;
                });
            }

            vm.change = change;
        };

        function change($index) {

            var next = $index + 1;

            if (config.apis.length > next) {

                vm.loading = true;

                var url = config.apis[next].replace(/{(\d+)}/g, function (match, number) {
                    return typeof $scope.model.value[number] != "undefined" ? $scope.model.value[number] : match;
                });

                $http({ method: "GET", url: url }).then(function (response) {
                    vm.dropdowns[next] = { options: response.data };
                    vm.loading = false;
                });
            }

            setDirty();
        };

        function setDirty() {
            if ($scope.propertyForm) {
                $scope.propertyForm.$setDirty();
            }
        };

        init();
    }
]);
