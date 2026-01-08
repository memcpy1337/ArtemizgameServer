using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Models;

public class GameUpdateRequestModel
{
    public required string Token { get; set; }
    public required string RegistryUrl { get; set; }
    public required string ImageName { get; set; }
    public required string TagVersion { get; set; }
}
