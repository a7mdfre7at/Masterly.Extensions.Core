using System.Collections.Generic;
using System.Linq;
using System.Web;
using Ardalis.GuardClauses;

namespace System.Collections.Specialized
{
    public static class NameValueCollectionExtensions
    {
        public static string ToQueryString(this NameValueCollection collection)
        {
            Guard.Against.Null(collection, nameof(collection));

            string[] array = (from key in collection.AllKeys
                              from value in collection.GetValues(key)
                              select $"{HttpUtility.UrlEncode(key)}={HttpUtility.UrlEncode(value)}")
                            .ToArray();

            return string.Join("&", array);
        }

        public static string ToQueryString(this KeyValuePair<string, string>[] collection)
        {
            Guard.Against.Null(collection, nameof(collection));

            return string.Join("&", collection.Select(x => $"{HttpUtility.UrlEncode(x.Key)}={HttpUtility.UrlEncode(x.Value)}").ToArray());
        }
    }
}