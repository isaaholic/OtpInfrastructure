namespace OtpModule.Abstractions;

public interface IOtpSender
{
    Task SendAsync(string key, string code);
}
