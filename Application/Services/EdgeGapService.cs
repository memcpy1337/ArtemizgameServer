using Application.Common.DTOs;
using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Models.EdgeGap;
using Application.Services;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Infrastructure.Services;

public sealed class EdgeGapService : ICloudServiceProvider<EdgeGapDeploymentResult>
{
    private readonly EdgeGapConfigurationProvider _configProvider;
    public IServerHttpClient<EdgeGapDeploymentResult> HttpClient { get; set; }

    public EdgeGapService(IEdgeGapHttpClient httpClient, EdgeGapConfigurationProvider configProvider)
    {
        _configProvider = configProvider;
        HttpClient = httpClient;
    }

    public Task DeleteServer(string matchId)
    {
        throw new System.NotImplementedException();
    }

    public async Task<EdgeGapDeploymentResult> RequestNewServer(MatchNewDTO match, string serverId, string serverToken)
    {
        var deploymentData = await HttpClient.NewDeployment(
            match.GameType,
            serverId,
            serverToken,
            _configProvider.Data!.AppName!, 
            _configProvider.Data!.AppVersion!, 
            match.UsersIp, 
            _configProvider.Data!.WebHookUrl!
        );

        return deploymentData;
    }
}
