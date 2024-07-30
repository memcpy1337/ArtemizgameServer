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


public record EdgeGapDeployWebhookCommand(EdgeGapDeploymentWebhookModel WebHookData) : IRequestWrapper<Unit>;

internal sealed class EdgeGapDeployWebhookCommandHandler : IHandlerWrapper<EdgeGapDeployWebhookCommand, Unit>
{
    private readonly IDeployService _deployService;
    private readonly ILogger<EdgeGapDeploymentWebhookModel> _logger;

    public EdgeGapDeployWebhookCommandHandler(IDeployService deployService, ILogger<EdgeGapDeploymentWebhookModel> logger)
    {
        _deployService = deployService;
        _logger = logger;
    }

    public async Task<IResponse<Unit>> Handle(EdgeGapDeployWebhookCommand request, CancellationToken cancellationToken)
    {
        var data = request.WebHookData;

        _logger.LogInformation($"NEW STATUS FOR {request.WebHookData.RequestId}. STATUS: {request.WebHookData.CurrentStatusLabel}");

        var newState = StatusFromStringToEnum(data.CurrentStatusLabel!);

        switch(newState)
        {
            case ServerStatus.Ready:
                await _deployService.SetConnectionData(data.RequestId!, data.Address!, data.Ports!.GamePort!.External);
                break;
        }

        await _deployService.UpdateStatus(data.RequestId!, newState);

        return Response.Success(Unit.Value);
    }

    private ServerStatus StatusFromStringToEnum(string status)
    {
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