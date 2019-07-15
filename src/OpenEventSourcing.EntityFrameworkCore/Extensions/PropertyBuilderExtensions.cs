using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpenEventSourcing.EntityFrameworkCore.ChangeTracking;
using OpenEventSourcing.EntityFrameworkCore.ValueConversion;

namespace OpenEventSourcing.EntityFrameworkCore.Extensions
{
    public static class PropertyBuilderExtensions
    {
        public static PropertyBuilder<T> HasJsonValueConversion<T>(this PropertyBuilder<T> builder)
            where T : class
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            builder.HasConversion(new JsonValueConverter<T>())
                   .Metadata
                   .SetValueComparer(new JsonValueComparer<T>());

            return builder;
        }
    }
}
