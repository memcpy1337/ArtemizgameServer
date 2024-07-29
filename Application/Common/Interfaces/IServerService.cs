﻿using Contracts.Common.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Interfaces;

public interface IServerService
{
    Task Up(string serverId);
    Task Down(string serverId);
    Task PlayerConnected(string serverId, string userId);
    Task PlayerDisconnected(string serverId, string userId);
}
