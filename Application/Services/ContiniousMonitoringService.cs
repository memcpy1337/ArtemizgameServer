using Application.Common.Interfaces;
using Application.Models.EdgeGap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services;

public class ContiniousMonitoringService : IContiniousMonitoringService
{
    private readonly ICloudServiceProvider<EdgeGapDeploymentResult> _cloudServiceProvider;
    private readonly IServerRepository _serverRepo;
    public ContiniousMonitoringService(ICloudServiceProvider<EdgeGapDeploymentResult> cloudServiceProvider, IServerRepository serverRepository)
    {
        _cloudServiceProvider = cloudServiceProvider;
        _serverRepo = serverRepository;
    }

    public async Task DeployVersion(string versionTag, string registry, string imageName)
    {
        var versions = await _serverRepo.GetServerVersions();

        if (versions.Count >= 2)
        {
            var previousVersionTag = versions
                .OrderByDescending(v => v.Version)
                .Skip(1)                              
                .First()
                .Version;

            await _cloudServiceProvider.DeleteVersion(previousVersionTag);
        }

        await _cloudServiceProvider.CreateNewVersion(versionTag, registry, imageName);

        await _serverRepo.NewVersion(new Domain.Entities.GameVersion() { ReleaseDate = DateTime.UtcNow, Version = versionTag });
    }
}
