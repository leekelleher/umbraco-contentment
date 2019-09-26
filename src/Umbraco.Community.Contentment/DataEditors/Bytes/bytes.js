/* This Source Code has been derived from a StackOverflow answer.
 * https://stackoverflow.com/a/18650828/12787
 * Licensed under the permissions of the CC BY-SA 4.0.
 * https://creativecommons.org/licenses/by-sa/4.0/
 * Modifications are licensed under the Mozilla Public License.
 * Copyright © 2019 Lee Kelleher, Umbrella Inc and other contributors.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

angular.module("umbraco.filters").filter("formatBytes", function () {
    return function (bytes, opts) {
        if (bytes === 0 || opts === undefined) return "0 Bytes";

        const k = parseInt(opts.kilo) || 1024;
        const dm = parseInt(opts.decimals) || 0;
        const sizes = ["Bytes", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB"];

        const i = Math.floor(Math.log(bytes) / Math.log(k));

        return parseFloat((bytes / Math.pow(k, i)).toFixed(dm)) + " " + sizes[i];
    }
});
