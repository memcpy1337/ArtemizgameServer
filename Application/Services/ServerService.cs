using Application.Common.DTOs;
using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Models.EdgeGap;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services;

public class ServerService : IServerService
{
    private readonly ICloudServiceProvider<EdgeGapDeploymentResult> _cloudServiceProvider;
    private readonly IDeploymentPublisher _deploymentPublisher;
    private readonly IServerRepository _serverRepository;
    private readonly ITokenGenerationService _tokenGeneration;

    private static string TOKEN_ERR_MSG = "Secure token failed to generate";

    public ServerService(ICloudServiceProvider<EdgeGapDeploymentResult> cloudServiceProvider, 
        IDeploymentPublisher deploymentPublisher, 
        IServerRepository serverRepository,
        ITokenGenerationService tokenGenerationService)
    {
        _cloudServiceProvider = cloudServiceProvider;
        _deploymentPublisher = deploymentPublisher;
        _serverRepository = serverRepository;
        _tokenGeneration = tokenGenerationService;
    }

    public async Task CreateServer(MatchNewDTO match)
    {
        string serverId = Guid.NewGuid().ToString();

        var serverToken = await _tokenGeneration.GetTokenServer(serverId);

        if (serverToken == null)
        {
            await _deploymentPublisher.DeploymentFailed(match.MatchId, TOKEN_ERR_MSG);
            return;
        }

        var deploymentResult = await _cloudServiceProvider.RequestNewServer(match, serverId, serverToken);

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
            ServerStatus = ServerStatus.Initializing
        });

        await _deploymentPublisher.DeploymentSuccess(match.MatchId);
    }
}
