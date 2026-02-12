namespace OtpModule.Core;

public class OtpEntry
{
    public int Id { get; set; }
    public string Key { get; set; } = string.Empty;
    public string CodeHash { get; set; } = string.Empty;
    public DateTime ExpireAt { get; set; }
    public int AttemptCount { get; set; }
    public bool IsUsed { get; set; }
    public DateTime CreatedAt { get; set; }
}
