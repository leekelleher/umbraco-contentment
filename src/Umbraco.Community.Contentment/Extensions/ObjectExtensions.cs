/* Copyright © 2013-present Umbraco.
 * This Source Code has been derived from Umbraco CMS.
 * https://github.com/umbraco/Umbraco-CMS/blob/release-16.3.4/src/Umbraco.Core/Extensions/ObjectExtensions.cs
 * Copied under the permissions of the MIT License.
 * Copyright © 2025 Lee Kelleher.
 */

#if NET10_0_OR_GREATER
using System.ComponentModel;

namespace Umbraco.Extensions;

internal static class ObjectExtensions
{
    public static IDictionary<string, TVal> ToDictionary<TVal>(this object o, params string[] ignoreProperties)
    {
        if (o != null)
        {
            var props = TypeDescriptor.GetProperties(o);
            var d = new Dictionary<string, TVal>();
            foreach (var prop in props.Cast<PropertyDescriptor>().Where(x => ignoreProperties.Contains(x.Name) == false))
            {
                var val = prop.GetValue(o);
                if (val != null)
                {
                    d.Add(prop.Name, (TVal)val);
                }
            }

            return d;
        }

        return new Dictionary<string, TVal>();
    }
}
#endif
