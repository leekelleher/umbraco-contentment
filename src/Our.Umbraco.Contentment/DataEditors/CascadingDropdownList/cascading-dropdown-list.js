/* Copyright © 2019 Lee Kelleher, Umbrella Inc and other contributors.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

angular.module("umbraco").controller("Our.Umbraco.Contentment.DataEditors.CascadingDropdownList.Controller", [
    "$scope",
    "$q",
    "$http",
    function ($scope, $q, $http) {

        // console.log("cascading-dropdown-list.model", $scope.model);

        var defaultConfig = { apis: [], options: [] };
        var config = angular.extend({}, defaultConfig, $scope.model.config);

        var vm = this;

        function init() {

            $scope.model.value = $scope.model.value || [{}];

            vm.dropdowns = [];

            if (config.options.length === 0) {

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

                        _.each(results, function (x, i) {
                            vm.dropdowns[i] = { options: x.data }
                        });

                        vm.loading = false;
                    });
                }

            } else {

                _.each(config.options, function (x, i) {
                    vm.dropdowns.push({ options: x });
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
