using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Threading;
using Domain.Entities;

public interface IApplicationDbContext
{
    public DbSet<Server> Servers { get; set; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}