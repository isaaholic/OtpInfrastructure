using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using OtpModule.Abstractions;
using OtpModule.Core;

namespace OtpModule.Infrastructure;

public class OtpService : IOtpService
{
    private readonly IOtpDbContext _db;
    private readonly IOtpGenerator _generator;
    private readonly IHashService _hash;
    private readonly IOtpSender _sender;
    private readonly OtpOptions _options;

    public OtpService(
        IOtpDbContext db,
        IOtpGenerator generator,
        IHashService hash,
        IOtpSender sender,
        IOptions<OtpOptions> options)
    {
        _db = db;
        _generator = generator;
        _hash = hash;
        _sender = sender;
        _options = options.Value;
    }

    public async Task SendAsync(string key)
    {
        var last = await _db.OtpEntries
            .Where(x => x.Key == key)
            .OrderByDescending(x => x.CreatedAt)
            .FirstOrDefaultAsync();

        if (last is not null && last.CreatedAt.AddSeconds(_options.ResendSeconds) > DateTime.UtcNow)
            throw new Exception("Wait before resend");

        var code = _generator.Generate();

        var entity = new OtpEntry
        {
            Key = key,
            CodeHash = _hash.Hash(code),
            ExpireAt = DateTime.UtcNow.AddMinutes(_options.ExpireMinutes),
            AttemptCount = 0,
            IsUsed = false,
            CreatedAt = DateTime.UtcNow
        };

        _db.OtpEntries.Add(entity);
        await _db.SaveChangesAsync();

        await _sender.SendAsync(key, code);
    }

    public async Task<bool> VerifyAsync(string key, string code)
    {
        var otp = await _db.OtpEntries
            .Where(x => x.Key == key && !x.IsUsed)
            .OrderByDescending(x => x.CreatedAt)
            .FirstOrDefaultAsync();

        if (otp == null) return false;
        if (otp.ExpireAt < DateTime.UtcNow) return false;
        if (otp.AttemptCount >= _options.MaxAttempts) return false;

        otp.AttemptCount++;

        if (otp.CodeHash != _hash.Hash(code))
        {
            await _db.SaveChangesAsync();
            return false;
        }

        otp.IsUsed = true;
        await _db.SaveChangesAsync();

        return true;
    }
}
