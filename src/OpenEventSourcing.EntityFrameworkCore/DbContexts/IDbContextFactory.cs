namespace OpenEventSourcing.EntityFrameworkCore.DbContexts
{
    public interface IDbContextFactory
    {
        OpenEventSourcingDbContext Create();
    }
}
