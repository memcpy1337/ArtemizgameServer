using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Interfaces;

public interface IContiniousMonitoringService
{
    Task DeployVersion(string versionTag, string registry, string imageName);
}
