using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Interfaces;

public interface IDeploymentPublisher
{
    Task DeploymentFailed(string matchId, string message);
    Task DeploymentSuccess(string matchId);
    Task DeploymentConnectionDataUpdate(string serverId, string matchId, string address, int port);
}
