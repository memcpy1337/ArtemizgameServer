using Application.Common.DTOs;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Interfaces;

public interface IDeployService
{
    Task Deploy(MatchNewDTO match);
    Task SetConnectionData(string deploymentId, string address, int port);
    Task UpdateStatus(string deploymentId, ServerStatus newStatus);
}
