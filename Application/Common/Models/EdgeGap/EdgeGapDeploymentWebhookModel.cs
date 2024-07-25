using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Application.Common.Models.EdgeGap;

public class EdgeGapDeploymentWebhookModel
{
    [JsonPropertyName("request_id")]
    public string? RequestId { get; set; }
    [JsonPropertyName("fqdn")]
    public string? Address { get; set; }
    [JsonPropertyName("app_name")]
    public string? AppName { get; set; }
    [JsonPropertyName("app_version")]
    public string? AppVersion { get; set; }
    [JsonPropertyName("current_status")]
    public string? CurrentStatus { get; set; }
    [JsonPropertyName("current_status_label")]
    public string? CurrentStatusLabel { get; set; }
    [JsonPropertyName("running")]
    public bool Running { get; set; }
    [JsonPropertyName("start_time")]
    public string? StartTime { get; set; }
    [JsonPropertyName("removal_time")]
    public string? RemovalTime { get; set; }
    [JsonPropertyName("elapsed_time")]
    public int ElapsedTime { get; set; }
    [JsonPropertyName("error")]
    public bool Error { get; set; }
    [JsonPropertyName("public_ip")]
    public string? PublicIp { get; set; }
    [JsonPropertyName("ports")]
    public Ports? Ports { get; set; }
    [JsonPropertyName("last_status")]
    public string? LastStatus { get; set; }
    [JsonPropertyName("last_status_label")]
    public string? LastStatusLabel { get; set; }
}

public class Ports
{
    [JsonPropertyName("Game Port")]
    public GamePort? GamePort { get; set; }
}

public class GamePort
{
    [JsonPropertyName("external")]
    public int External { get; set; }
    [JsonPropertyName("protocol")]
    public string? Protocol { get; set; }
    [JsonPropertyName("name")]
    public string? Name { get; set; }
    [JsonPropertyName("tls_upgrade")]
    public bool TlsUpgrade { get; set; }
    [JsonPropertyName("link")]
    public string? Link { get; set; }
}
