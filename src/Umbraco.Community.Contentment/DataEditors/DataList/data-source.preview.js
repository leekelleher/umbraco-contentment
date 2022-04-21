/* Copyright © 2020 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

angular.module("umbraco").controller("Umbraco.Community.Contentment.DataEditors.DataListSource.Preview.Controller", [
    "$scope",
    "$http",
    "eventsService",
    "umbRequestHelper",
    function ($scope, $http, eventsService, umbRequestHelper) {

        //console.log("data-source.preview.model", $scope.model);

        var config = Object.assign({}, $scope.model.config);
        var vm = this;

        function init() {

            vm.property = {};
            vm.state = "loading";

            vm.tabs = [{
                label: "Data source items",
                alias: "dataSource",
                active: false,
            }, {
                label: "JSON",
                alias: "rawJson",
                active: false,
            }];

            vm.changeTab = tab => {
                vm.tabs.forEach(x => x.active = false);
                vm.activeTab = tab.alias;
                tab.active = true;
            };

            // set the first tab to active
            vm.changeTab(vm.tabs[0]);

            var events = [];

            events.push(eventsService.on("contentment.config-editor.model", (event, args) => {
                config[args.alias] = args.value;
                fetch();
            }));

            $scope.$on("$destroy", () => {
                for (var event in events) {
                    eventsService.unsubscribe(events[event]);
                }
            });

        };

        function fetch() {
            if (_.isEmpty(config.dataSource) === false) {

                vm.state = "loading";
                vm.items = null;

                umbRequestHelper.resourcePromise(
                    $http.post("backoffice/Contentment/DataListApi/GetDataSourceItems", config),
                    "Failed to retrieve data source items.")
                    .then(result => {

                        vm.items = result.config.items;

                        vm.state = vm.items && vm.items.length > 0 ? "loaded" : "noItems";

                        // HACK: Replaces data sources label to include the item count. Could be a nicer way of doing this.
                        vm.tabs[0].label = "Data source items (" + vm.items.length + ")";
                    });
            }
            else {
                vm.state = "init";
            }
        };

        init();
    }
]);
