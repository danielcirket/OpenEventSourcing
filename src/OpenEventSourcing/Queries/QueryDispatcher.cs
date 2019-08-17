using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using OpenEventSourcing.Extensions;

namespace OpenEventSourcing.Queries
{
    internal sealed class QueryDispatcher : IQueryDispatcher
    {
        private readonly ILogger<QueryDispatcher> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly IQueryStore _queryStore;

        public QueryDispatcher(ILogger<QueryDispatcher> logger,
            IServiceProvider serviceProvider,
            IQueryStore queryStore)
        {
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));
            if (serviceProvider == null)
                throw new ArgumentNullException(nameof(serviceProvider));
            if (queryStore == null)
                throw new ArgumentNullException(nameof(queryStore));

            _logger = logger;
            _serviceProvider = serviceProvider;
            _queryStore = queryStore;
        }

        public async Task<TQueryResult> DispatchAsync<TQueryResult>(IQuery<TQueryResult> query)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));

            var type = query.GetType();

            _logger.LogInformation($"Dispatching query '{type.FriendlyName()}' returning '{typeof(TQueryResult).FriendlyName()}'.");

            var handler = _serviceProvider.GetService(typeof(IQueryHandler<,>).MakeGenericType(type, typeof(TQueryResult)));

            if (handler == null)
                throw new InvalidOperationException($"No query handler for type '{type.FriendlyName()}' has been registered.");

            var result = await (Task<TQueryResult>)handler.GetType().GetMethod("RetrieveAsync").Invoke(handler, new[] { query });

            await _queryStore.SaveAsync<TQueryResult>(query);

            return result;
        }
    }
}
