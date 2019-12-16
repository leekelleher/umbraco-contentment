/* Copyright © 2019 Marc Stöcker.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

angular.module("umbraco").controller("Umbraco.Community.Contentment.DataEditors.ImageButtonList.Controller", [
    "$scope",
    "mediaResource",
    function($scope, mediaResource) {

        // console.log("imagebuttonlist.model", $scope.model);

        var defaultConfig = { items: [], showDescriptions: 1, svgSpriteMedia: null, defaultValue: "" };
        var config = angular.extend({}, defaultConfig, $scope.model.config);
        
        var vm = this;

        function init() {
            $scope.model.value = $scope.model.value || config.defaultValue;

            if (_.isArray($scope.model.value)) {
                $scope.model.value = _.first($scope.model.value);
            }

            vm.items = angular.copy(config.items);

            vm.uniqueId = [$scope.model.alias, $scope.model.dataTypeKey.substring(0, 8)].join("-");
            vm.showDescriptions = Object.toBoolean(config.showDescriptions);
            vm.svgSpriteMedia = config.svgSpriteMedia;

            //console.log("svgSpriteMedia", vm.svgSpriteMedia);
            mediaResource.getById(vm.svgSpriteMedia)
                .then(function(media) {
                    vm.svgSpriteUrl = media.mediaLink;
                    //console.log("fetched sprite media", media);
                });
        };

        init();
    }
]);
