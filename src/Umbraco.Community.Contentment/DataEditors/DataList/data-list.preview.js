/* Copyright © 2020 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

angular.module("umbraco").controller("Umbraco.Community.Contentment.DataEditors.DataList.Preview.Controller", [
    "$scope",
    "$http",
    "eventsService",
    "umbRequestHelper",
    function ($scope, $http, eventsService, umbRequestHelper) {

        //console.log("data-list.preview.model", $scope.model);

        var config = Object.assign({}, $scope.model.config);
        var vm = this;

        function init() {

            vm.property = {};
            vm.state = "loading";

            vm.tabs = [{
                label: "Editor preview",
                alias: "listEditor",
                active: false,
            }, {
                label: "Data source items",
                alias: "dataSource",
                active: false,
            }];

            // set the first tab to active
            vm.tabs[0].active = true;
            vm.activeTab = vm.tabs[0].alias;

            vm.changeTab = function (tab) {
                vm.tabs.forEach(function (x) { x.active = false; });
                vm.activeTab = tab.alias;
                tab.active = true;
            };

            var events = [];

            events.push(eventsService.on("contentment.config-editor.model", function (event, args) {
                config[args.alias] = args.value;
                fetch();
            }));

            $scope.$on("$destroy", function () {
                for (var event in events) {
                    eventsService.unsubscribe(events[event]);
                }
            });

        };

        function fetch() {
            if (_.isEmpty(config.dataSource) === false && _.isEmpty(config.listEditor) === false) {

                vm.state = "loading";
                vm.property = null;

                umbRequestHelper.resourcePromise(
                    $http.post("backoffice/Contentment/DataListApi/GetPreview", config),
                    "Failed to retrieve data list preview.")
                    .then(function (result) {

                        vm.property = result;
                        vm.state = result.config.items.length > 0 ? "loaded" : "noItems";

                    });
            }
            else if (_.isEmpty(config.dataSource) === false) {
                vm.state = "dataSourceConfigured";
            }
            else if (_.isEmpty(config.listEditor) === false) {
                vm.state = "listEditorConfigured";
            }
            else {
                vm.state = "init";
            }
        };

        init();
    }
]);
