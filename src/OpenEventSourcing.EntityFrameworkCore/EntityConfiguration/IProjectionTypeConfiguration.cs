using Microsoft.EntityFrameworkCore;

namespace OpenEventSourcing.EntityFrameworkCore.EntityConfiguration
{
    public interface IProjectionTypeConfiguration<T> : IEntityTypeConfiguration<T>
        where T : class
    {
    }
}
