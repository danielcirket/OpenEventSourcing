using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpenEventSourcing.EntityFrameworkCore.Entities;

namespace OpenEventSourcing.EntityFrameworkCore.EntityConfiguration
{
    internal sealed class QueryEntityTypeConfiguration : IEntityTypeConfiguration<Query>
    {
        public void Configure(EntityTypeBuilder<Query> builder)
        {
            builder.ToTable(name: nameof(Query), schema: "log");

            builder.HasKey(c => c.SequenceNo);
            builder.HasIndex(c => c.Id);
            builder.HasIndex(c => c.CorrelationId);
            builder.HasIndex(c => c.Name);
            builder.HasIndex(c => c.UserId);
        }
    }
}
