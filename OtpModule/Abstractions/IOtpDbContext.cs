using Microsoft.EntityFrameworkCore;
using OtpModule.Core;

namespace OtpModule.Abstractions;

public interface IOtpDbContext
{
    DbSet<OtpEntry> OtpEntries { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
