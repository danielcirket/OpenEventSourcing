using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpenEventSourcing.EntityFrameworkCore.Entities;

namespace OpenEventSourcing.EntityFrameworkCore.EntityConfiguration
{
    internal class CommandEntityTypeConfiguration : IEntityTypeConfiguration<Command>
    {
        public void Configure(EntityTypeBuilder<Command> builder)
        {
            builder.ToTable(name: nameof(Command), schema: "log");

            builder.HasKey(c => c.SequenceNo);
            builder.HasIndex(c => c.AggregateId);
            builder.HasIndex(c => c.CorrelationId);
            builder.HasIndex(c => c.Name);
        }
    }
}
