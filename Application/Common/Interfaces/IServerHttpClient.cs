using Application.Common.Models;
using Contracts.Common.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Interfaces;

public interface IServerHttpClient<T> where T : DeploymentResult
{
    Task<T> NewDeployment(GameTypeEnum gameType, string serverId, string serverToken, string appName, string appVersion, List<string> ipClients, string webHook);
    Task<bool> DestroyDeploy(string serverId);
}
