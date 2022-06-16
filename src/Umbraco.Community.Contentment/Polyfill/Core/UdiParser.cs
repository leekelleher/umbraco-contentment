/* This Source Code has been copied from Lee Kelleher's Umbraco Polyfill library.
 * https://github.com/leekelleher/umbraco-polyfill/blob/main/src/Core/UdiParser.cs
 * Modified under the permissions of the MIT License.
 * Modifications are licensed under the Mozilla Public License.
 * Copyright © 2022 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

#if NET472
namespace Umbraco.Core
{
    internal sealed class UdiParser
    {
        public static Udi Parse(string s) => Udi.Parse(s);

        public static Udi Parse(string s, bool knownTypes) => Udi.Parse(s, knownTypes);

        public static bool TryParse(string s, out Udi udi) => Udi.TryParse(s, out udi);

        public static bool TryParse(string s, out GuidUdi udi) => GuidUdi.TryParse(s, out udi);

        public static bool TryParse(string s, out StringUdi udi) => StringUdi.TryParse(s, out udi);

        public static bool TryParse(string s, bool knownTypes, out Udi udi) => Udi.TryParse(s, knownTypes, out udi);
    }
}
#endif
