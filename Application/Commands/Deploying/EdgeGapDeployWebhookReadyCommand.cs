using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Common.Models.EdgeGap;
using Application.Common.Wrappers;
using Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Commands.Deploying;


public record EdgeGapDeployWebhookReadyCommand(EdgeGapDeploymentWebhookModel WebHookData) : IRequestWrapper<Unit>;

internal sealed class EdgeGapDeployWebhookCommandHandler : IHandlerWrapper<EdgeGapDeployWebhookReadyCommand, Unit>
{
    private readonly IDeployService _deployService;
    private readonly ILogger<EdgeGapDeploymentWebhookModel> _logger;

    public EdgeGapDeployWebhookCommandHandler(IDeployService deployService, ILogger<EdgeGapDeploymentWebhookModel> logger)
    {
        _deployService = deployService;
        _logger = logger;
    }

    public async Task<IResponse<Unit>> Handle(EdgeGapDeployWebhookReadyCommand request, CancellationToken cancellationToken)
    {
        var data = request.WebHookData;

        _logger.LogInformation($"READY DEPLOY {request.WebHookData.RequestId}");

        await _deployService.SetConnectionData(data.RequestId!, data.Address!, data.Ports!.GamePort!.External);

        await _deployService.UpdateStatus(data.RequestId!, ServerStatus.Ready);

        return Response.Success(Unit.Value);
    }

    private ServerStatus StatusFromStringToEnum(string status)
    {
        status = status.Replace("Status.", "");
        if (Enum.TryParse(typeof(ServerStatus), status, out var result))
        {
            return (ServerStatus)result;
        }
        else
        {
            return ServerStatus.NA;
        }
    }
}