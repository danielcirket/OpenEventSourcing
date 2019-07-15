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

        public async Task<TQueryResult> DispatchAsync<TQuery, TQueryResult>(TQuery query)
            where TQuery : class, IQuery<TQueryResult>
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));

            _logger.LogInformation($"Dispatching query '{typeof(TQuery).FriendlyName()}' returning '{typeof(TQueryResult).FriendlyName()}'.");

            var type = query.GetType();

            var handler = _serviceProvider.GetService(typeof(IQueryHandler<,>).MakeGenericType(type, typeof(TQueryResult)));

            if (handler == null)
                throw new InvalidOperationException($"No query handler for type '{typeof(TQuery).FriendlyName()}' has been registered.");

            var result = await (Task<TQueryResult>)handler.GetType().GetMethod("RetrieveAsync").Invoke(handler, new[] { query });

            await _queryStore.SaveAsync<TQuery, TQueryResult>(query);

            return result;
        }
    }
}
