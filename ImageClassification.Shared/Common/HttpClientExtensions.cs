using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace ImageClassification.Shared.Common
{
    public static class HttpClientExtensions
    {
        public static async Task<(IDisposable Disposable, T Result)> GetAsync<T>(this HttpClient httpClient, Uri uri)
        {
            var response = await httpClient.GetAsync(uri);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var content = await response.Content.ReadAsStringAsync();
                return (response, JsonConvert.DeserializeObject<T>(content));
            }
            return (response, default);
        }
    }
}
