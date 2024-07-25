using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Models;

public abstract class DeploymentResult
{
    public required string RequestId { get; set; }
    public string? ErrorMsg { get; set; }
}
