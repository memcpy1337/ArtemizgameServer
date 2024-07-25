using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models.EdgeGap;

public class EdgeGapDeploymentRequestModel
{
    public required string AppName { get; set; }
    public required string VersionName { get; set; }
    public required List<string> IpList { get; set; }
    public required string WebhookUrl { get; set; }
    public required List<EnvVar> EnvVars { get; set; }
}

public class EnvVar
{
    public required string Key { get; set; }
    public required string Value { get; set; }
}