using System.Linq;
using System.Net.Http;

namespace OnlinerTracker.Core
{
    public static class HttpRequestMessageExtensions
    {
        public static string GetValueFromQueryString(this HttpRequestMessage request, string key)
        {
            var queryStrings = request.GetQueryNameValuePairs();
            if (queryStrings == null) return null;
            var match = queryStrings.FirstOrDefault(keyValue => string.Compare(keyValue.Key, key, true) == 0);
            return string.IsNullOrEmpty(match.Value) ? null : match.Value;
        }
    }
}
