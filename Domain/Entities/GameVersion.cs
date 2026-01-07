using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities;

public class GameVersion
{
    public int Id { get; set; }
    public string CurrentVersion { get; set; } = string.Empty;
    public string PreviousVersion { get; set; } = string.Empty;
}
