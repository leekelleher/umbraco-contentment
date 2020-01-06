/* Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

angular.module("umbraco").component("contentmentPropertyActions", {
    bindings: {
        actions: "<"
    },
    controller: function () {
        this.$onInit = function () {
            if (this.umbProperty && this.actions) {
                this.umbProperty.setPropertyActions(this.actions);
            }
        };
    },
    require: {
        umbProperty: "?^umbProperty"
    }
});
