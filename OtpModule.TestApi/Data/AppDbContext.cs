using Microsoft.EntityFrameworkCore;
using OtpModule.Abstractions;
using OtpModule.Core;

namespace OtpModule.TestApi.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options), IOtpDbContext
{
    public DbSet<OtpEntry> OtpEntries => Set<OtpEntry>();
}
