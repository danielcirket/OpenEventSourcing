using System;
using System.Text;

namespace OpenEventSourcing.RabbitMQ.Extensions
{
    internal static class StringExtensions
    {
        public static string Base64Encode(this string source)
        {
            if (source == null)
                throw new ArgumentNullException(source);

            return Convert.ToBase64String(Encoding.UTF8.GetBytes(source));
        }
    }
}
