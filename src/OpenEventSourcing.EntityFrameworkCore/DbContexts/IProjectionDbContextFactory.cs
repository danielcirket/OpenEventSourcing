namespace OpenEventSourcing.EntityFrameworkCore.DbContexts
{
    public interface IProjectionDbContextFactory
    {
        OpenEventSourcingProjectionDbContext Create();
    }
}
