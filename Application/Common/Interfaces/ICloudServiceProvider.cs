using Application.Common.DTOs;
using Application.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Interfaces;

public interface ICloudServiceProvider<T> where T : DeploymentResult
{
    Task<T> RequestNewServer(MatchNewDTO match, string serverId, string serverToken);
    Task DeleteServer(string matchId);
    public IServerHttpClient<T> HttpClient { get; set; }
}
