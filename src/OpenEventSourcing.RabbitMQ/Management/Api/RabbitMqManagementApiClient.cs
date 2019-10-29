using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using OpenEventSourcing.RabbitMQ.Extensions;
using OpenEventSourcing.RabbitMQ.Management.Api.Responses;

namespace OpenEventSourcing.RabbitMQ.Management.Api
{
    public class RabbitMqManagementApiClient : IRabbitMqManagementApiClient
    {
        private readonly HttpClient _client;
        private readonly IOptions<RabbitMqOptions> _options;

        public RabbitMqManagementApiClient(HttpClient client, IOptions<RabbitMqOptions> options)
        {
            _client = client;
            _options = options;

            _client.BaseAddress = _options.Value.ManagementApi?.Endpoint;
            _client.DefaultRequestHeaders.Add("Accept", "application/json");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", $"{_options.Value.ManagementApi?.User}:{_options.Value.ManagementApi?.Password}".Base64Encode());
        }

        public async Task<IEnumerable<RabbitMqBinding>> RetrieveSubscriptionsAsync(string queue)
        {
            var vhost = new Uri(_options.Value.ConnectionString).AbsolutePath.TrimStart('/');

            if (string.IsNullOrEmpty(vhost))
                vhost = Uri.EscapeDataString("/");

            var exchange = Uri.EscapeDataString(_options.Value.Exchange);
            var queueName = Uri.EscapeDataString(queue);

            var response = await _client.GetAsync($"/api/queues/{vhost}/{queueName}/bindings");
            
            response.EnsureSuccessStatusCode();

            var results = await response.Content.ReadAsAsync<IEnumerable<RabbitMqSubscriptionResponse>>();

            return results.Where(r => r.source == _options.Value.Exchange && r.destination_type == "queue")
                          .Select(r => new RabbitMqBinding
                          {
                              Exchange = r.source,
                              Queue = r.destination,
                              RoutingKey = r.routing_key,
                          });
        }
    }
}
