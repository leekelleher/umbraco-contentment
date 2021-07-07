/* Copyright © 2020 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

angular.module("umbraco").controller("Umbraco.Community.Contentment.Tree.Controller", [
    "$scope",
    "$window",
    "navigationService",
    function ($scope, $window, navigationService) {

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

            vm.subscribe = function ($event) {
                $window.open("https://tinyletter.com/umbraco-contentment", "newsletterWindow", "scrollbars=yes,width=840,height=640");
                return true;
            };

            vm.vote = function (x) {
                if (x === false && !vm.nggyu) {

                    vm.nggyu = true;

                    // Kudos to Mathieu 'p01' Henri for this snippet:
                    // Music SoftSynth https://gist.github.com/p01/1285255
                    var softSynth = function (f) { return eval("for(var t=0,S='RIFF_oO_WAVEfmt " + atob("EAAAAAEAAQBAHwAAQB8AAAEACAA") + "data';++t<3e5;)S+=String.fromCharCode(" + f + ")"); };
                    var formula = "(t<<3)*[8/9,1,9/8,6/5,4/3,3/2,0][[0xd2d2c8,0xce4088,0xca32c8,0x8e4009][t>>14&3]>>(0x3dbe4688>>((t>>10&15)>9?18:t>>10&15)*3&7)*3&7]&255";
                    vm.audio = new Audio("data:audio/wav;base64," + btoa(softSynth(formula)));
                    vm.audio.play();

                } else {

                    vm.nggyu = false;

                    if (vm.audio) {
                        vm.audio.pause();
                    }

                }
            };

            vm.codeSnippetLanguage = "JSON";
            vm.telemetryEnabled = config.telemetry === true;
            vm.disableTelemetryCode = { "Umbraco": { "Contentment": { "DisableTelemetry": true } } };
            vm.disableTreeCode = { "Umbraco": { "Contentment": { "DisableTree": true } } };
        };

        init();
    }
]);
