using Application.Commands.Deploying;
using Application.Common.Models.EdgeGap;
using Ardalis.ApiEndpoints;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using ServerService.API.Routes;

namespace API.Endpoints.Deploying;

[Route(DeployingRoute.EdgeGapWebhook)]
public class EdgeGapDeployWebhook : EndpointBaseAsync
    .WithRequest<string>
    .WithoutResult
{
    private readonly IMediator _mediator;

    public EdgeGapDeployWebhook(IMediator mediator) => _mediator = mediator;

    [HttpPost, Produces("application/json"), Consumes("application/json")]
    public override async Task<ActionResult> HandleAsync([FromBody] string data /*[FromBody] EdgeGapDeploymentWebhookModel request*/, CancellationToken cancellationToken = new())
    {
        Console.WriteLine(data);
        await _mediator.Send(new EdgeGapDeployWebhookCommand(new EdgeGapDeploymentWebhookModel()), cancellationToken);
        return NoContent();
    }
}