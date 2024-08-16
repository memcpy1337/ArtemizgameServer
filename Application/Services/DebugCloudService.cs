using Application.Common.DTOs;
using Application.Common.Interfaces;
using Application.Common.Models.EdgeGap;
using Application.Models.EdgeGap;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services;

public class DebugCloudService : ICloudServiceProvider<DebugDeploymentResult>
{
    public IServerHttpClient<DebugDeploymentResult> HttpClient { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public Task DeleteServer(string matchId)
    {
        return Task.CompletedTask;
    }

    public async Task<DebugDeploymentResult> RequestNewServer(MatchNewDTO match, string serverId, string serverToken)
    {
        Random rnd = new Random();
        var data = new DebugDeploymentResult() { RequestId = $"{rnd.Next(100000, 999999)}", ErrorMsg = null };

        await File.WriteAllTextAsync("/app/Key/key.txt", serverToken);
        await File.WriteAllTextAsync("/app/Regime/regime.txt", (Convert.ToInt32(match.GameType)).ToString());

        return Task.FromResult(data).Result;
    }
}
