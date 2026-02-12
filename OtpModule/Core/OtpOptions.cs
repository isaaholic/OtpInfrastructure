namespace OtpModule.Core;

public class OtpOptions
{
    public int ExpireMinutes { get; set; } = 5;
    public int MaxAttempts { get; set; } = 5;
    public int ResendSeconds { get; set; } = 60;
}
