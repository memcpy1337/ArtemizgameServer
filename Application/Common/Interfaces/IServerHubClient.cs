using Contracts.Common.Models;
using Contracts.Common.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Interfaces;

public interface IServerHubClient
{
    Task ServerMatchStart();
    Task ServerMatchCancel(MatchCancelEnum reason);
    Task ServerMatchEnd(List<MatchPlayerResult> matchResults);
}
