using Application.Common.Models;
using Application.Common.Models.EdgeGap;
using Application.Common.Wrappers;
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
    public EdgeGapDeployWebhookCommandHandler()
    {

    }

    public async Task<IResponse<Unit>> Handle(EdgeGapDeployWebhookCommand request, CancellationToken cancellationToken)
    {
        var data = request.WebHookData;
       
        return Response.Success(Unit.Value);
    }
}