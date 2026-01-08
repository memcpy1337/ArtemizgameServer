using Application.Commands.Deploying;
using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Common.Models.EdgeGap;
using Application.Common.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Commands.ContiniousDelivery;

public record GameNewVersionCommand(GameUpdateRequestModel WebHookData) : IRequestWrapper<Unit>;

internal sealed class GameNewVersionCommandHandler : IHandlerWrapper<GameNewVersionCommand, Unit>
{
    private readonly IContiniousMonitoringService _continiousMonitoringService;
    public GameNewVersionCommandHandler(IContiniousMonitoringService continiousMonitoringService)
    {
        _continiousMonitoringService = continiousMonitoringService;
    }

    public async Task<IResponse<Unit>> Handle(GameNewVersionCommand request, CancellationToken cancellationToken)
    {
        if (request.WebHookData.Token != Environment.GetEnvironmentVariable("ContiniousMonitoringToken"))
        {
            return Response.Fail<Unit>("Incorrect token");
        }

        await _continiousMonitoringService.DeployVersion(request.WebHookData.TagVersion, request.WebHookData.RegistryUrl, request.WebHookData.ImageName);

        return Response.Success(Unit.Value);
    }
}