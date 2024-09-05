using Application.Common.Helpers;
using Application.Common.Interfaces;
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
        var content = new EdgeGapDeploymentRequestModel
        {
            AppName = appName,
            VersionName = appVersion,
            IpList = ipClients,
            WebhookUrl = webHook,
            EnvVars = new List<EnvVar>() { 
                new EnvVar() 
                { 
                    Key = "GameType", 
                    Value = ((int)gameType).ToString() 
                },
                new EnvVar()
                {
                    Key = "Token",
                    Value = serverToken
                }
            }
        };

        var data = EdgeGapJsonHelper<EdgeGapDeploymentRequestModel>.Create(content);

        var httpResponse = await _httpClient.PostAsync(_config.Data!.DeployUrl, data);

        if (!httpResponse.IsSuccessStatusCode)
        {
            _logger.LogError($"Error while deployment. Code: {httpResponse.StatusCode.ToString()}");
            return new EdgeGapDeploymentResult() { RequestId = string.Empty, ErrorMsg = httpResponse.StatusCode.ToString() };
        }

        var response = await httpResponse.Content.ReadAsStringAsync();

        return EdgeGapJsonHelper<EdgeGapDeploymentResult>.Deserialize(response);
    }

    public async Task DestroyDeploy(string requestId)
    {
        _logger.LogInformation(JsonConvert.SerializeObject(_config.Data));
        var uri = $"{_config.Data!.DeleteUrl}/{requestId}";

        var httpResponse = await _httpClient.DeleteAsync(uri);

        if (!httpResponse.IsSuccessStatusCode)
        {
            _logger.LogError($"Error HTTP request remove deployment: {httpResponse.ReasonPhrase}. {httpResponse.StatusCode.ToString()}. {uri}.");
        }
    }
}
