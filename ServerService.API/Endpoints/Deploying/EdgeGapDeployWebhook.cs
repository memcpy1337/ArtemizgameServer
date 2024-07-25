using Application.Commands.Deploying;
using Application.Common.Models.EdgeGap;
using Ardalis.ApiEndpoints;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using ServerService.API.Routes;

namespace API.Endpoints.Deploying;

[Route(DeployingRoute.EdgeGapWebhook)]
public class EdgeGapDeployWebhook : EndpointBaseAsync
    .WithRequest<EdgeGapDeploymentWebhookModel>
    .WithoutResult
{
    private readonly IMediator _mediator;

    public EdgeGapDeployWebhook(IMediator mediator) => _mediator = mediator;

    [HttpPost, Produces("application/json"), Consumes("application/json")]
    public override async Task<ActionResult> HandleAsync(EdgeGapDeploymentWebhookModel request, CancellationToken cancellationToken = new())
    {
        await _mediator.Send(new EdgeGapDeployWebhookCommand(request), cancellationToken);
        return NoContent();
    }
}