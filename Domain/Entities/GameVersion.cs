using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities;

public class GameVersion
{
    public int Id { get; set; }
    public required string Version { get; set; }
    public required DateTime ReleaseDate { get; set; }
}
