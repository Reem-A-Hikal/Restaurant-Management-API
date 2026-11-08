
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Rest.Application.Validation;
using Rest.Infrastructure.Data;

namespace Rest.API.BackgroundServices
{
    /// <summary>
    /// Removes uploaded images that no product or user references anymore.
    /// </summary>
    public sealed class OrphanImageCleanupService : BackgroundService
    {
        private static readonly string[] Folders = ["products", "users"];

        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IWebHostEnvironment _env;
        private readonly ImageCleanupOptions _options;
        private readonly ILogger<OrphanImageCleanupService> _logger;

        public OrphanImageCleanupService(
            IServiceScopeFactory scopeFactory,
            IWebHostEnvironment env,
            IOptions<ImageCleanupOptions> options,
            ILogger<OrphanImageCleanupService> logger)
        {
            _scopeFactory = scopeFactory;
            _env = env;
            _options = options.Value;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (!_options.Enabled) return;

            // Let the app finish starting before the first pass.
            await Task.Delay(TimeSpan.FromMinutes(2), stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await RunOnceAsync(stoppingToken);
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                {
                    return;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Orphan image cleanup failed.");
                }

                await Task.Delay(TimeSpan.FromHours(Math.Max(1, _options.IntervalHours)), stoppingToken);
            }
        }

        private async Task RunOnceAsync(CancellationToken ct)
        {
            var webRoot = _env.WebRootPath ?? Path.Combine(_env.ContentRootPath, "wwwroot");

            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<RestDbContext>();

            // IgnoreQueryFilters: archived products and deleted users still "own" their files.
            var productUrls = await db.Products.IgnoreQueryFilters()
                .Where(p => p.ImageUrl != null)
                .Select(p => p.ImageUrl!)
                .ToListAsync(ct);

            var userUrls = await db.Users.IgnoreQueryFilters()
                .Where(u => u.ProfileImageUrl != null)
                .Select(u => u.ProfileImageUrl!)
                .ToListAsync(ct);

            var referenced = new HashSet<string>(
                productUrls.Concat(userUrls), StringComparer.OrdinalIgnoreCase);

            var cutoff = DateTime.UtcNow.AddHours(-Math.Max(1, _options.MinAgeHours));
            var orphans = new List<string>();
            var totalFiles = 0;

            foreach (var folder in Folders)
            {
                var dir = Path.Combine(webRoot, "images", folder);
                if (!Directory.Exists(dir)) continue;

                foreach (var file in Directory.EnumerateFiles(dir))
                {
                    totalFiles++;
                    var url = $"/images/{folder}/{Path.GetFileName(file)}";

                    if (!ImageUrlRules.IsValid(url)) continue;              // only files our uploader created
                    if (File.GetLastWriteTimeUtc(file) > cutoff) continue;  // too recent
                    if (referenced.Contains(url)) continue;                 // still in use

                    orphans.Add(file);
                }
            }

            // Sanity guard: files exist but the DB references none of them
            // (wrong database / failed seed) must never trigger a mass delete.
            if (referenced.Count == 0 && totalFiles > 0)
            {
                _logger.LogWarning(
                    "Orphan cleanup skipped: {Files} files on disk but no image references in the database.",
                    totalFiles);
                return;
            }

            if (!_options.DeleteFiles)
            {
                _logger.LogInformation(
                    "Orphan cleanup (dry run): {Count} of {Total} files would be deleted.",
                    orphans.Count, totalFiles);
                return;
            }

            var deleted = 0;
            foreach (var file in orphans)
            {
                try { File.Delete(file); deleted++; }
                catch (IOException ex) { _logger.LogWarning(ex, "Could not delete {File}", file); }
            }

            _logger.LogInformation("Orphan cleanup: deleted {Deleted} of {Total} files.", deleted, totalFiles);
        }
    }
}
