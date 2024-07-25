using Contracts.Common.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.DTOs;

public record MatchNewDTO
{
    public required string MatchId;
    public required List<string> UsersIp;
    public GameTypeEnum GameType;
}
