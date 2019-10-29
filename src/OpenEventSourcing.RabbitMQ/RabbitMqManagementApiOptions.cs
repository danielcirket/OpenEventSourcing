using System;

namespace OpenEventSourcing.RabbitMQ
{
    public class RabbitMqManagementApiOptions
    {
        internal Uri Endpoint { get; set; }
        internal string User { get; set; }
        internal string Password { get; set; }

        public RabbitMqManagementApiOptions WithEndpoint(string endpoint)
        {
            if (string.IsNullOrEmpty(endpoint))
                throw new ArgumentException($"'{nameof(endpoint)}' cannot be null or empty.", nameof(endpoint));
            if (!Uri.TryCreate(endpoint, UriKind.RelativeOrAbsolute, out var uri))
                throw new ArgumentException($"'{nameof(endpoint)}' could not be parsed.", nameof(endpoint));

            Endpoint = uri;

            return this;
        }
        public RabbitMqManagementApiOptions WithCredentials(string user, string password)
        {
            if (string.IsNullOrEmpty(user))
                throw new ArgumentException($"'{nameof(user)}' cannot be null or empty.", nameof(user));
            if (string.IsNullOrEmpty(password))
                throw new ArgumentException($"'{nameof(password)}' cannot be null or empty.", nameof(password));

            User = user;
            Password = password;

            return this;
        }
    }
}
