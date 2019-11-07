using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace OpenEventSourcing.RabbitMQ.Extensions
{
    internal static class HttpContentExtensions
    {
        public static async Task<T> ReadAsAsync<T>(this HttpContent content)
        {
            if (content == null)
                throw new ArgumentNullException(nameof(content));

            var value = await content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<T>(value);
        }
    }
}
