/* Copyright © 2013-present Umbraco.
 * This Source Code has been derived from Umbraco CMS.
 * https://github.com/umbraco/Umbraco-CMS/blob/release-8.3.0/src/Umbraco.Web.UI.Client/src/views/documenttypes/copy.controller.js
 * Modified under the permissions of the MIT License.
 * Modifications are licensed under the Mozilla Public License.
 * Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

angular.module("umbraco").controller("Umbraco.Community.Contentment.Trees.DataTypeCopy.Controller", [
    "$scope",
    "$http",
    "appState",
    "navigationService",
    "treeService",
    "umbRequestHelper",
    function ($scope, $http, appState, navigationService, treeService, umbRequestHelper) {

        // console.log("data-type-copy", $scope.currentNode);

        var vm = this;

        function init() {

            vm.busy = false;
            vm.success = false;
            vm.error = false;

            vm.source = _.clone($scope.currentNode);

            vm.dialogTreeApi = {};
            vm.initDataTypeTree = initDataTypeTree;

            vm.copy = copy;
            vm.close = close;
        };

        function initDataTypeTree() {
            vm.dialogTreeApi.callbacks.treeNodeSelect(function (args) {

                args.event.preventDefault();
                args.event.stopPropagation();

                if (vm.target) {
                    //un-select if there's a current one selected
                    vm.target.selected = false;
                }

                vm.target = args.node;
                vm.target.selected = true;

            });
        };

        function copy() {

            vm.busy = true;
            vm.error = false;
            vm.success = false;

            umbRequestHelper.resourcePromise(
                $http.post("backoffice/Contentment/DataTypeCopyApi/Copy", { parentId: vm.target.id, id: vm.source.id }, { responseType: "text" }),
                "Failed to copy data type")
                .then(function (path) {

                    vm.busy = false;
                    vm.error = false;
                    vm.success = true;

                    //get the currently edited node (if any)
                    var activeNode = appState.getTreeState("selectedNode");

                    //we need to do a double sync here: first sync to the copied content - but don't activate the node,
                    //then sync to the currenlty edited content (note: this might not be the content that was copied!!)
                    navigationService.syncTree({
                        tree: "dataTypes",
                        path: path,
                        forceReload: true,
                        activate: false
                    }).then(function (args) {
                        if (activeNode) {
                            var activeNodePath = treeService.getPath(activeNode).join();
                            //sync to this node now - depending on what was copied this might already be synced but might not be
                            navigationService.syncTree({
                                tree: "dataTypes",
                                path: activeNodePath,
                                forceReload: false,
                                activate: true
                            });
                        }
                    });

                }, function (error) {
                    vm.busy = false;
                    vm.error = error;
                    vm.success = false;
                });

        };

        function close() {
            navigationService.hideDialog();
        };

        init();
    }
]);
