using Application.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models.EdgeGap;

public class EdgeGapDeploymentResult : DeploymentResult
{
    public string RequestDns { get; set; }
    public string RequestVersion { get; set; }
}
