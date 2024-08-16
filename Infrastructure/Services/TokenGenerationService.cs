

using Application.Common.Interfaces;
using Contracts.Events.ServerEvents;
using MassTransit;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Infrastructure.Services;

public class TokenGenerationService : ITokenGenerationService
{
    private readonly IRequestClient<ServerTokenRequest> _client;
    private readonly ILogger<TokenGenerationService> _logger;

    public TokenGenerationService(IRequestClient<ServerTokenRequest> client, ILogger<TokenGenerationService> logger)
    {
        _client = client;
        _logger = logger;
    }

    public async Task<string?> GetTokenServer (string serverId, string matchId)
    {
        try
        {
            var response = await _client.GetResponse<ServerTokenResponse>(new ServerTokenRequest { MatchId = matchId, ServerId = serverId });
            return response.Message.Token;
        }
        catch (RequestTimeoutException ex)
        {
            _logger.LogError($"Timeout request get token for server {serverId}");
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Exception request get token for server {serverId} error: {ex.Message}");
            return null;
        }
    }
}
