using Application.Commands.ContiniousDelivery;
using Application.Commands.Deploying;
using Application.Common.Models;
using Application.Common.Models.EdgeGap;
using Ardalis.ApiEndpoints;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using ServerService.API.Routes;

namespace API.Endpoints.Deploying;

[Route(DeployingRoute.GameUpdateWebhook)]
public class GameUpdateWebhook : EndpointBaseAsync
    .WithRequest<GameUpdateRequestModel>
    .WithoutResult
{
    private readonly IMediator _mediator;

    public GameUpdateWebhook(IMediator mediator) => _mediator = mediator;

    [HttpPost, Produces("application/json"), Consumes("application/json")]
    public override async Task<ActionResult> HandleAsync([FromBody] GameUpdateRequestModel request, CancellationToken cancellationToken = new())
    {
        await _mediator.Send(new GameNewVersionCommand(request), cancellationToken);
        return NoContent();
    }
}