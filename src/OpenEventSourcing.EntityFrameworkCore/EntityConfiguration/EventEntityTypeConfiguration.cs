using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpenEventSourcing.EntityFrameworkCore.Entities;

namespace OpenEventSourcing.EntityFrameworkCore.EntityConfiguration
{
    internal sealed class EventEntityTypeConfiguration : IEntityTypeConfiguration<Event>
    {
        public void Configure(EntityTypeBuilder<Event> builder)
        {
            builder.ToTable(name: nameof(Event), schema: "log");

            builder.HasKey(c => c.SequenceNo);
            builder.HasIndex(c => c.AggregateId);
            builder.HasIndex(c => c.CorrelationId);
            builder.HasIndex(c => c.CausationId);
            builder.HasIndex(c => c.Name);
        }
    }
}
