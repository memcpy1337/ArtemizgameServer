using Application.Common.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Math.EC.Rfc7748;
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

    public async Task<Server> GetByServerId(string serverId)
    {
        return await _context.Servers.Include(s => s.ConnectionData).SingleAsync(s => s.ServerId == serverId);
    }

    public async Task<Server?> GetByDeployId(string deployId)
    {
        return await _context.Servers.FirstOrDefaultAsync(s => s.ExternalRequestId == deployId && s.IsActive);
    }

    public async Task SetConnectionData(string requestId, ConnectionData connectionData)
    {
        await _context.Servers.Where(x => x.ExternalRequestId == requestId && x.IsActive)
            .ExecuteUpdateAsync(b => b.SetProperty(u => u.ConnectionData, connectionData));

        await _context.SaveChangesAsync(CancellationToken.None);
    }

    public async Task UpdateStatus(string requestId, ServerStatus newStatus)
    {
        await _context.Servers.Where(x => x.ExternalRequestId == requestId)
            .ExecuteUpdateAsync(b => b.SetProperty(u => u.DeployStatus, newStatus));

        await _context.SaveChangesAsync(CancellationToken.None);
    }

    public async Task SetServerReady(string serverId)
    {
        await _context.Servers.Where(x => x.ServerId == serverId)
            .ExecuteUpdateAsync(b => b.SetProperty(u => u.IsReady, true));

        await _context.SaveChangesAsync(CancellationToken.None);
    }

    public async Task SetServerInactive(string serverId)
    {
        await _context.Servers.Where(x => x.ServerId == serverId)
            .ExecuteUpdateAsync(b => b.SetProperty(u => u.IsActive, false));

        await _context.SaveChangesAsync(CancellationToken.None);
    }
}
