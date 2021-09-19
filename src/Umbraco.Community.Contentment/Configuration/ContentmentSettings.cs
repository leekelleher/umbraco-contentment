/* Copyright © 2021 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

#if NET472 == false
namespace Umbraco.Community.Contentment
{
    public class ContentmentSettings
    {
        public bool DisableTree { get; set; } = false;

        public bool DisableTelemetry { get; set; } = false;
    }
}
#endif
