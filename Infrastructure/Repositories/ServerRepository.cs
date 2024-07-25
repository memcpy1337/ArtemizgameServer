using Application.Common.Interfaces;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Repositories;

public class ServerRepository : IServerRepository
{
    private readonly IApplicationDbContext _context;
    public ServerRepository(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Create(Server server)
    {
        await _context.Servers.AddAsync(server);

        await _context.SaveChangesAsync(CancellationToken.None);
    }
}
