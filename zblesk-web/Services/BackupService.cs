using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Threading;
using zblesk_web.Configs;
using zblesk_web.Models;

namespace zblesk_web.Services;

public class BackupService
{
    private readonly ApplicationDbContext _db;
    private readonly ILogger<BackupService> _logger;
    private readonly Config _config;

    public BackupService(
        ILogger<BackupService> logger,
        ApplicationDbContext db,
        Config config)
    {
        _db = db ?? throw new ArgumentNullException(nameof(db));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _config = config ?? throw new ArgumentNullException(nameof(config));
    }

    public async Task CreateBackup(string filename = null)
    {
        if (string.IsNullOrWhiteSpace(filename))
        {
            filename = $"obskurnee.{DateTime.Now.ToString("yyyyMMdd_HHmmss")}.db";
        }
        if (!Directory.Exists(_config.DataFolder))
        {
            _logger.LogInformation("Creating Backup directory at {backupDir}", _config.DataFolder);
            Directory.CreateDirectory(_config.DataFolder);
        }
        var path = Path.Combine(_config.DataFolder, filename);
        _logger.LogInformation("Backing up to {fname}", path);
        await _db.Database.ExecuteSqlRawAsync($"VACUUM INTO '{path}';", new CancellationTokenSource(200).Token);
        _logger.LogInformation("Backup to {fname} finished", path);
    }
}
