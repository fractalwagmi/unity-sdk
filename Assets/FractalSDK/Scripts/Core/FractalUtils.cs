using System;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using UnityEngine;

namespace FractalSDK.Core
{
    public static class FractalUtils
    {
        public static Action<string> logCallback;
        public static Action<string> logErrorCallback;
        /// <summary>
        /// Logging feature for fractal output, all messages are tagged with Fractal flag.
        /// </summary>
        /// <param name="message">Message to log into the Unity console.</param>
        public static void Log(string message)
        {
            if (logCallback != null)
            {
                logCallback(message);
            }
            else
            {
                Debug.Log($"<color=#F2059F>[FRACTAL]</color> {message}");
            }
        }

        public static void LogError(string message)
        {
            if (logErrorCallback != null)
            {
                logErrorCallback(message);
            }
            else
            {
                Debug.LogError($"<color=#F2059F>[FRACTAL]</color> {message}");
            }
        }
        
        /// <summary>
        /// Generates a Query string for API requests
        /// </summary>
        /// <param name="nvc">Collection of queries to serialize into string.</param>
        public static string ToQueryString(NameValueCollection nvc)
        {
            string[] queryArray = (
                from key in nvc.AllKeys
                from value in nvc.GetValues(key)
                select string.Format(
                    "{0}={1}",
                    HttpUtility.UrlEncode(key),
                    HttpUtility.UrlEncode(value))
            ).ToArray();
            return "?" + string.Join("&", queryArray);
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
}
