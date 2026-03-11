namespace Common.Extensions;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class IntExtensions
{
    public static string ToStringHandleUnknown(this int value, string?[]? unknownValues = null, string unknownString = "?") =>
        ((int?)value).ToStringHandleUnknown(unknownValues, unknownString);

    public static string ToStringHandleUnknown(this int? value, string?[]? unknownValues = null, string unknownString = "?")
    {
        unknownValues ??= ["-1", null];

        return value?.ToString() is string stringValue && !unknownValues.Contains(stringValue)
             ? stringValue
             : unknownString;
    }
}
