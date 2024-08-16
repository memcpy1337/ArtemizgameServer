using Application.Common.DTOs;
using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Common.Models.EdgeGap;
using Application.Models.EdgeGap;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services;

public class DeployService : IDeployService
{
    private readonly ICloudServiceProvider<EdgeGapDeploymentResult> _cloudServiceProvider;
    private readonly IDeploymentPublisher _deploymentPublisher;
    private readonly IServerRepository _serverRepository;
    private readonly ITokenGenerationService _tokenGeneration;
    private readonly ICloudServiceProvider<DebugDeploymentResult> _debugCloudProvider;

    private const string TOKEN_ERR_MSG = "Secure token failed to generate";

    public DeployService(ICloudServiceProvider<EdgeGapDeploymentResult> cloudServiceProvider,  ICloudServiceProvider<DebugDeploymentResult> debugCloudProvider,
        IDeploymentPublisher deploymentPublisher, 
        IServerRepository serverRepository,
        ITokenGenerationService tokenGenerationService)
    {
        _cloudServiceProvider = cloudServiceProvider;
        _deploymentPublisher = deploymentPublisher;
        _serverRepository = serverRepository;
        _tokenGeneration = tokenGenerationService;
        _debugCloudProvider = debugCloudProvider;
    }

    public async Task Deploy(MatchNewDTO match)
    {
        string serverId = Guid.NewGuid().ToString();

        var serverToken = await _tokenGeneration.GetTokenServer(serverId, match.MatchId);

        if (serverToken == null)
        {
            await _deploymentPublisher.DeploymentFailed(match.MatchId, TOKEN_ERR_MSG);
            return;
        }

#if !DEBUG
        var deploymentResult = await _cloudServiceProvider.RequestNewServer(match, serverId, serverToken);
#else
        var deploymentResult = await _debugCloudProvider.RequestNewServer(match, serverId, serverToken);
#endif

        if (string.IsNullOrEmpty(deploymentResult.ErrorMsg) == false)
        {
            await _deploymentPublisher.DeploymentFailed(match.MatchId, deploymentResult.ErrorMsg);
            return;
        }

        await _serverRepository.Create(new Server()
        {
            ExternalRequestId = deploymentResult.RequestId,
            MatchId = match.MatchId,
            ServerId = serverId,
            DeployStatus = ServerStatus.Initializing,
            IsActive = true
        });

        await _deploymentPublisher.DeploymentSuccess(match.MatchId);

#if DEBUG
        await SetConnectionData(deploymentResult.RequestId, "127.0.0.1", 7777); 
#endif
    }

    public async Task SetConnectionData(string deploymentId, string address, int port)
    {
        await _serverRepository.SetConnectionData(deploymentId, new ConnectionData() { Ip = address, Port = port });

        var data = await _serverRepository.GetByDeployId(deploymentId);

        await _deploymentPublisher.DeploymentConnectionDataUpdate(data.ServerId, data.MatchId, address, port);
    }

    public async Task UpdateStatus(string deploymentId, ServerStatus newStatus)
    {
        await _serverRepository.UpdateStatus(deploymentId, newStatus);
    }

    public async Task RemoveDeploy(string deploymentId)
    {
       await _cloudServiceProvider.DeleteServer(deploymentId);
    }
}
