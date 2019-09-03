/* Copyright © 2019 Lee Kelleher, Umbrella Inc and other contributors.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

angular.module("umbraco").controller("Umbraco.Community.Contentment.Overlays.SocialPicker.Controller", [
    "$scope",
    "editorService",
    function ($scope, editorService) {

        // console.log("social-picker-overlay.model", $scope.model);

        var defaultConfig = {
            title: "Select social network...",
        };
        var config = angular.extend({}, defaultConfig, $scope.model.config);

        var vm = this;

        function init() {

            vm.title = config.title;

            // NOTE: I've deliberately not added: adn, dropbox, foursquare, google, microsoft, openid
            vm.items = [
                { name: "Bitbucket", network: "bitbucket", url: "https://bitbucket.org/" },
                { name: "Facebook", network: "facebook", url: "https://facebook.com/" },
                { name: "Flickr", network: "flickr", url: "https://www.flickr.com/" },
                { name: "GitHub", network: "github", url: "https://github.com/" },
                { name: "Instagram", network: "instagram", url: "https://instagram.com/" },
                { name: "LinkedIn", network: "linkedin", url: "https://linkedin.com/in/" },
                { name: "Одноклассники", network: "odnoklassniki", url: "https://ok.ru/" },
                { name: "Pinterest", network: "pinterest", url: "https://pinterest.com/" },
                { name: "Reddit", network: "reddit", url: "https://www.reddit.com/user/" },
                { name: "SoundCloud", network: "soundcloud", url: "https://soundcloud.com/" },
                { name: "Tumblr", network: "tumblr", url: "https://{username}.tumblr.com/" },
                { name: "Twitter", network: "twitter", url: "https://twitter.com/" },
                { name: "Vimeo", network: "vimeo", url: "https://vimeo.com/" },
                { name: "VK", network: "vk", url: "https://vk.com/" },
            ];

            vm.select = select;
            vm.close = close;
        };

        function select(item) {
            if ($scope.model.submit) {
                $scope.model.submit(item);
            }
        };

        function close() {
            if ($scope.model.close) {
                $scope.model.close();
            }
        };

        init();
    }
]);
