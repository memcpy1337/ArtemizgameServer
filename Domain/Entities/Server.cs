using System.Text.RegularExpressions;

namespace Domain.Entities;

public class Server
{
    public int Id { get; set; }
    public required string ServerId { get; set; }
    public required string MatchId { get; set; }
    public required string ExternalRequestId { get; set; }
    public bool IsReady { get; set; } = false;
    public bool IsActive { get; set; } = false;
    public ConnectionData? ConnectionData { get; set; }
    public ServerStatus DeployStatus { get; set; } = ServerStatus.NA;
}

public enum ServerStatus
{
    NA,
    Initializing,
    Seeking,
    Deploying,
    Ready,
    Seeked,
    Terminated,
    Scanning,
    Terminating,
    Error
}