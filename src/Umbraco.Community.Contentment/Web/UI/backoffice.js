/* Copyright © 2020 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

angular.module("umbraco").controller("Umbraco.Community.Contentment.Tree.Controller", [
    "$scope",
    "$window",
    "navigationService",
    function ($scope, $window, navigationService) {

        // console.log("tree.model", $scope.model);

        var vm = this;

        function init() {

            const alias = "contentment";

            var config = Umbraco.Sys.ServerVariables[alias];

            vm.title = config.name;
            vm.version = "v" + config.version;

            navigationService.syncTree({ tree: alias, path: "-1" });

            $scope.$emit("$changeTitle", vm.title);

            vm.links = [
                {
                    icon: "icon-fa fa-book",
                    name: "Documentation",
                    url: "https://github.com/leekelleher/umbraco-contentment/tree/master/docs",
                    description: "How to use each of the property editors."
                },
                {
                    icon: "icon-fa fa-comments-o",
                    name: "Support forum",
                    url: "https://our.umbraco.com/packages/backoffice-extensions/contentment/contentment-feedback/",
                    description: "Ask for help, the community is your friend."
                },
                {
                    icon: "icon-fa fa-code-fork",
                    name: "Source code",
                    url: "https://github.com/leekelleher/umbraco-contentment",
                    description: "See the code, all free and open-source."
                },
                {
                    icon: "icon-fa fa-bug",
                    name: "Issue tracker",
                    url: "https://github.com/leekelleher/umbraco-contentment/issues/new/choose",
                    description: "Found a bug? Suggest a feature? Let me know."
                },
                {
                    icon: "icon-fa fa-id-card-o",
                    name: "License",
                    url: "https://opensource.org/licenses/MPL-2.0",
                    description: "Licensed under the Mozilla Public License."
                }
            ];

            vm.shareUrl = encodeURIComponent("https://github.com/leekelleher/umbraco-contentment");
            vm.shareTitle = encodeURIComponent("Check out Contentment, innovative editor components for Umbraco CMS!");

            vm.subscribe = function ($event) {
                $window.open("https://tinyletter.com/umbraco-contentment", "newsletterWindow", "scrollbars=yes,width=840,height=640");
                return true;
            };

            vm.vote = function (x) {
                vm.nggyu = x == false;
            };
        };

        init();
    }
]);
