using OtpModule.Abstractions;
using System.Security.Cryptography;
using System.Text;

namespace OtpModule.Infrastructure;

public class Sha256HashService : IHashService
{
    public string Hash(string input)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(input));
        return Convert.ToBase64String(bytes);
    }
}
