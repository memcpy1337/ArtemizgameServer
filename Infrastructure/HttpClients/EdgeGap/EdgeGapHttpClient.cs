using Application.Common.Helpers;
using Application.Common.Interfaces;
using Application.Common.Models.EdgeGap;
using Application.Models.EdgeGap;
using Application.Services;
using Contracts.Common.Models.Enums;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.HttpClients.EdgeGap;

public sealed class EdgeGapHttpClient : IEdgeGapHttpClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<EdgeGapHttpClient> _logger;
    private readonly EdgeGapConfigurationProvider _config;

    public EdgeGapHttpClient(HttpClient httpClient, ILogger<EdgeGapHttpClient> logger, EdgeGapConfigurationProvider configProvider)
    {
        _httpClient = httpClient;
        _logger = logger;
        _config = configProvider;
    }

    public async Task<EdgeGapDeploymentResult> NewDeployment(GameTypeEnum gameType, string serverId, string serverToken, string appName, string appVersion, List<string> ipClients, string webHook)
    {
        var uri = $"v2/deployments";

        var content = new EdgeGapDeploymentRequestModel
        {
            AppName = appName,
            VersionName = appVersion,
            Users = ipClients
                .Where(ip => !string.IsNullOrWhiteSpace(ip))
                .Select(ip => new User
                {
                    UserType = "ip_address",
                    Data = new UserData
                    {
                        IpAddress = ip
                    }
                })
                .ToList(),
            WebhookUrlError = webHook,
            WebhookUrlReady = webHook,
            WebhookUrlTerminated = webHook,
            EnvVars = new List<EnvVar>() { 
                new EnvVar() 
                { 
                    Key = "GameType", 
                    Value = ((int)gameType).ToString(),
                    IsHidden = false
                },
                new EnvVar()
                {
                    Key = "Token",
                    Value = serverToken,
                    IsHidden = false
                }
            }
        };

        var data = EdgeGapJsonHelper<EdgeGapDeploymentRequestModel>.Create(content);

        var httpResponse = await _httpClient.PostAsync(uri, data);

        if (!httpResponse.IsSuccessStatusCode)
        {
            var body = await httpResponse.Content.ReadAsStringAsync();
            _logger.LogError($"Error while deployment. Code: {httpResponse.StatusCode.ToString()}. Text: {body}");
            return new EdgeGapDeploymentResult() { RequestId = string.Empty, ErrorMsg = httpResponse.StatusCode.ToString() };
        }

        var response = await httpResponse.Content.ReadAsStringAsync();

        return EdgeGapJsonHelper<EdgeGapDeploymentResult>.Deserialize(response);
    }

    public async Task DestroyDeploy(string requestId)
    {
        var uri = $"v1/stop/{requestId}";

        var httpResponse = await _httpClient.DeleteAsync(uri);

        if (!httpResponse.IsSuccessStatusCode)
        {
            var body = await httpResponse.Content.ReadAsStringAsync();
            _logger.LogError($"Error HTTP request remove deployment: {httpResponse.ReasonPhrase}. {httpResponse.StatusCode.ToString()}. {uri}. Text: {body}");
        }
    }

    public async Task DeleteVersion(string appName, string versionToDelete)
    {
        var uri = $"v1/app/{appName}/version/{versionToDelete}";
        var httpResponse = await _httpClient.DeleteAsync(uri);
        if (!httpResponse.IsSuccessStatusCode)
        {
            var body = await httpResponse.Content.ReadAsStringAsync();
            _logger.LogError($"Error while delete version. Code: {httpResponse.StatusCode.ToString()}. Text: {body} ");
            throw new Exception($"Error deleting version {versionToDelete} of app {appName}");
        }
    }

    public async Task CreateNewVersion(string appName, string versionTag, string registry, string imageName)
    {
        var content = new EdgeGapNewVersionRequestModel
        {
            Name = versionTag,
            DockerImage = imageName,
            DockerRepository = registry,
            DockerTag = versionTag,
            ForceCache = false,
            MaxDuration = 60,
            VerifyImage = true,
            Ports =
            [
                new Port()
                {
                    PortNumber = 7770,
                    Protocol = "TCP_UDP",
                    TLS = false,
                    WaitPortOpen = true,
                    Name = "GamePort"
                }
            ],
            ReqCpu = 256,
            ReqMem = 512
        };

        var data = EdgeGapJsonHelper<EdgeGapNewVersionRequestModel>.Create(content);
        var uri = $"v1/app/{appName}/version";
        var httpResponse = await _httpClient.PostAsync(uri, data);
        if (!httpResponse.IsSuccessStatusCode)
        {
            var body = await httpResponse.Content.ReadAsStringAsync();
            _logger.LogError($"Error while creating new version. Code: {httpResponse.StatusCode.ToString()}. Text: {body}");
            throw new Exception($"Error creating new version {versionTag} of app {appName}");
        }
    }
}
