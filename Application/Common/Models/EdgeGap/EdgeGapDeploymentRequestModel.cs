using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Application.Models.EdgeGap;

public class EdgeGapDeploymentRequestModel
{
    [JsonPropertyName("application")]
    public required string AppName { get; set; }
    [JsonPropertyName("version")]
    public required string VersionName { get; set; }
    [JsonPropertyName("users")]
    public required List<User> Users { get; set; }
    [JsonPropertyName("webhook_on_ready")]
    public required WebhookData WebhookUrlReady { get; set; }
    [JsonPropertyName("webhook_on_error")]
    public required WebhookData WebhookUrlError { get; set; }
    [JsonPropertyName("webhook_on_terminated")]
    public required WebhookData WebhookUrlTerminated { get; set; }
    [JsonPropertyName("environment_variables")]
    public required List<EnvVar> EnvVars { get; set; }
}

public class WebhookData
{
    [JsonPropertyName("url")]
    public required string Url { get; set; }
}
public class User
{
    [JsonPropertyName("user_type")]
    public required string UserType { get; set; }
    [JsonPropertyName("user_data")]
    public required UserData Data { get; set; }
}

public class UserData
{
    [JsonPropertyName("ip_address")]
    public required string IpAddress { get; set; }
}


public class EnvVar
{
    [JsonPropertyName("key")]
    public required string Key { get; set; }
    [JsonPropertyName("value")]
    public required string Value { get; set; }
    [JsonPropertyName("is_hidden")]
    public bool IsHidden { get; set; }
}