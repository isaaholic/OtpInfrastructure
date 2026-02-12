using OtpModule.Abstractions;

namespace OtpModule.Infrastructure;

public class DefaultOtpGenerator : IOtpGenerator
{
    public string Generate()
    {
        var r = new Random();
        return r.Next(100000, 999999).ToString();
    }
}
