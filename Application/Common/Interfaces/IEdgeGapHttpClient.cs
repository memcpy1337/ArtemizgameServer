using Application.Models.EdgeGap;
using Contracts.Common.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Interfaces;

public interface IEdgeGapHttpClient : IServerHttpClient<EdgeGapDeploymentResult>
{

}
