using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Common.Models.EdgeGap;
using Application.Common.Wrappers;
using Domain.Entities;
using MediatR;
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
    public EdgeGapDeployWebhookCommandHandler(IDeployService deployService)
    {
        _deployService = deployService;
    }

    public async Task<IResponse<Unit>> Handle(EdgeGapDeployWebhookCommand request, CancellationToken cancellationToken)
    {
        var data = request.WebHookData;

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