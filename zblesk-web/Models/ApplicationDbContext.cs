using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace zblesk_web.Models;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    private readonly ILoggerFactory _loggerFactory;

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> opts, ILoggerFactory loggerFactory) : base(opts)
    {
        _loggerFactory = loggerFactory;
    }

    public DbSet<StoredImage> Images { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseLoggerFactory(_loggerFactory);
        base.OnConfiguring(optionsBuilder);
    }
}
