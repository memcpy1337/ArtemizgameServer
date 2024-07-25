using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Settings;

public class EdgeGapConfiguration
{
    public string? AppName { get; set; }
    public string? AppVersion { get; set; }
    public string? DeployUrl { get; set; }
    public string? WebHookUrl { get; set; }
}
