/* Copyright © 2020 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

angular.module("umbraco").controller("Umbraco.Community.Contentment.Tree.Controller", [
    "$scope",
    "navigationService",
    function ($scope, navigationService) {

        var vm = this;

        function init() {

            const alias = "contentment";

            var config = Umbraco.Sys.ServerVariables.umbracoPlugins[alias];

            vm.title = config.name;
            vm.version = "v" + config.version;

            navigationService.syncTree({ tree: alias, path: "-1" });

            $scope.$emit("$changeTitle", vm.title);

            vm.links = [
                {
                    icon: "icon-fa fa-fw fa-book ",
                    name: "Documentation",
                    description: "How to use each of the property editors.",
                    url: "https://github.com/leekelleher/umbraco-contentment/tree/master/docs"
                },
                {
                    icon: "icon-fa fa-fw fa-comments-o",
                    name: "Support forum",
                    description: "Ask for help, the community is your friend.",
                    url: "https://our.umbraco.com/packages/backoffice-extensions/contentment/contentment-feedback/"
                },
                {
                    icon: "icon-fa fa-fw fa-code-fork",
                    name: "Source code",
                    description: "See the code, all free and open-source.",
                    url: "https://github.com/leekelleher/umbraco-contentment"
                },
                {
                    icon: "icon-fa fa-fw fa-bug",
                    name: "Issue tracker",
                    description: "Found a bug? Suggest a feature? Let me know.",
                    url: "https://github.com/leekelleher/umbraco-contentment/issues/new/choose"
                },
                {
                    icon: "icon-fa fa-fw fa-id-card-o",
                    name: "License",
                    description: "Licensed under the Mozilla Public License.",
                    url: "https://opensource.org/licenses/MPL-2.0"
                }
            ];

            vm.telemetryEnabled = config.telemetry === true;
        };

        init();
    }
]);
