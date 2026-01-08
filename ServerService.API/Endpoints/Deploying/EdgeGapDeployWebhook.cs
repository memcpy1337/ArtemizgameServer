using Application.Commands.Deploying;
using Application.Common.Models.EdgeGap;
using Ardalis.ApiEndpoints;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using ServerService.API.Routes;

namespace API.Endpoints.Deploying;

[Route(DeployingRoute.EdgeGapWebhook)]
public class EdgeGapDeployReadyWebhook : EndpointBaseAsync
    .WithRequest<EdgeGapDeploymentWebhookModel>
    .WithoutResult
{
    private readonly IMediator _mediator;

    public EdgeGapDeployReadyWebhook(IMediator mediator) => _mediator = mediator;

    [HttpPost, Produces("application/json"), Consumes("application/json")]
    public override async Task<ActionResult> HandleAsync([FromBody] EdgeGapDeploymentWebhookModel request, CancellationToken cancellationToken = new())
    {
        await _mediator.Send(new EdgeGapDeployWebhookReadyCommand(request), cancellationToken);
        return NoContent();
    }
}