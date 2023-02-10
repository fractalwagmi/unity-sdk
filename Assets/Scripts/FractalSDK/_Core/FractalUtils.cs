using System;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using UnityEngine;

public static class FractalUtils
{
    /// <summary>
    /// Logging feature for fractal output, all messages are tagged with Fractal flag.
    /// </summary>
    /// <param name="log">Message to log into the Unity console.</param>
    public static void Log(string message)
    {
        Debug.Log("<color=#F2059F>[FRACTAL]</color> " + message);
    }

    /// <summary>
    /// Generates a Query string for API requests
    /// </summary>
    /// <param name="nvc">Collection of querries to serialize into string.</param>
    public static string ToQueryString(NameValueCollection nvc)
    {
        string[] querryArray = (
            from key in nvc.AllKeys
            from value in nvc.GetValues(key)
            select string.Format(
            "{0}={1}",
            HttpUtility.UrlEncode(key),
            HttpUtility.UrlEncode(value))
            ).ToArray();
        return "?" + string.Join("&", querryArray);
    }

    /// <summary>
    /// Evaluates the enumerator to a string value
    /// </summary>
    /// <param name="type">Type of enumerable to transform to string.</param>
    public static string ToEnumString<T>(T type)
    {
        var enumType = typeof(T);
        var name = Enum.GetName(enumType, type);
        var enumMemberAttribute = ((EnumMemberAttribute[])enumType.GetField(name).GetCustomAttributes(typeof(EnumMemberAttribute), true)).Single();
        return enumMemberAttribute.Value;
    }
}
