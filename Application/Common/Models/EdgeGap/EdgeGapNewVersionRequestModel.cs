using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Application.Common.Models.EdgeGap;

public class EdgeGapNewVersionRequestModel
{
    [JsonPropertyName("name")]
    public required string Name { get; set; }
    [JsonPropertyName("docker_repository")]
    public required string DockerRepository { get; set; }
    [JsonPropertyName("docker_image")]
    public required string DockerImage { get; set; }
    [JsonPropertyName("docker_tag")]
    public required string DockerTag { get; set; }
    [JsonPropertyName("req_cpu")]
    public int ReqCpu { get; set; }
    [JsonPropertyName("req_memory")]
    public int ReqMem { get; set; }
    [JsonPropertyName("max_duration")]
    public int MaxDuration { get; set; }
    [JsonPropertyName("force_cache")]
    public bool ForceCache { get; set; }
    [JsonPropertyName("verify_image")]
    public bool VerifyImage { get; set; }
    [JsonPropertyName("ports")]
    public List<Port> Ports { get; set; } = new();
}

public class Port
{
    [JsonPropertyName("port")]
    public required int PortNumber { get; set; }
    [JsonPropertyName("protocol")]
    public required string Protocol { get; set; }
    [JsonPropertyName("to_check")]
    public bool WaitPortOpen { get; set; }
    [JsonPropertyName("tls_upgrade")]
    public bool TLS { get; set; }
    [JsonPropertyName("name")]
    public required string Name { get; set; }
}