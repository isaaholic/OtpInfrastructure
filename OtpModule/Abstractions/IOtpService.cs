namespace OtpModule.Abstractions;

public interface IOtpService
{
    Task SendAsync(string key);
    Task<bool> VerifyAsync(string key, string code);
}
