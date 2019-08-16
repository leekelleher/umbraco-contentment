/* Copyright © 2019 Lee Kelleher, Umbrella Inc and other contributors.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

angular.module("umbraco").controller("Umbraco.Community.Contentment.DataEditors.UserPicker.Controller", [
    "$scope",
    "editorService",
    "entityResource",
    "usersResource",
    function ($scope, editorService, entityResource, usersResource) {

        console.log("user-picker.model", $scope.model);

        var defaultConfig = {
            disableSorting: 0,
            maxItems: 0,
        };
        var config = angular.extend({}, defaultConfig, $scope.model.config);

        var vm = this;

        function init() {

            $scope.model.value = $scope.model.value || [];

            vm.sortable = Object.toBoolean(config.disableSorting) === false && (config.maxItems !== 1 && config.maxItems !== "1");

            vm.sortableOptions = {
                axis: "y",
                containment: "parent",
                cursor: "move",
                disabled: vm.sortable === false,
                opacity: 0.7,
                scroll: true,
                tolerance: "pointer",
                stop: function (e, ui) {
                    $scope.model.value = _.map(vm.items, function (x) { return x.id });
                    setDirty();
                }
            };

            vm.allowAdd = true;
            vm.allowRemove = true;

            vm.add = add;
            vm.remove = remove;

            vm.items = [];

            // TODO: [LK:2019-08-16] Querying for each user (using `usersResource.getUser`) is inefficient.
            // Using the `entityResource.getAll("User")` is better, but doesn't return the `avatars`.

            //if ($scope.model.value.length > 0) {
            //    entityResource.getAll("User").then(function (users) {
            //        _.each($scope.model.value, function (id) {
            //            var user = _.find(users, function (x) { return x.id === id });
            //            if (user) {
            //                vm.items.push(user);
            //            }
            //        });
            //    });
            //}

            if ($scope.model.value.length > 0) {
                _.each($scope.model.value, function (v) {
                    usersResource.getUser(v).then(function (user) {
                        vm.items.push(user);
                    });
                });
            }
        };

        function add() {

            var oldSelection = angular.copy(vm.items);
            var userPicker = {
                selection: vm.items,
                submit: function () {

                    $scope.model.value = _.map(vm.items, function (x) { return x.id });

                    setDirty();

                    editorService.close();
                },
                close: function () {
                    vm.items = oldSelection;
                    editorService.close();
                }
            };

            editorService.userPicker(userPicker);

        };

        function remove($index) {

            vm.items.splice($index, 1);
            $scope.model.value.splice($index, 1);

            if ((config.maxItems === 0 || config.maxItems === "0") || $scope.model.value.length < config.maxItems) {
                vm.allowAdd = true;
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
