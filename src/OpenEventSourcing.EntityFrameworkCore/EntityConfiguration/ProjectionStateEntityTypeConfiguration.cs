using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpenEventSourcing.EntityFrameworkCore.Entities;

namespace OpenEventSourcing.EntityFrameworkCore.EntityConfiguration
{
    internal sealed class ProjectionStateEntityTypeConfiguration : IEntityTypeConfiguration<ProjectionState>
    {
        public void Configure(EntityTypeBuilder<ProjectionState> builder)
        {
            builder.ToTable(name: nameof(ProjectionState));

            builder.HasKey(c => c.Name);
        }
    }
}
