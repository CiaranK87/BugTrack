using Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Persistence;

namespace API.Services
{
    public class DemoReseedService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IHostEnvironment _env;
        private readonly DemoSettings _settings;
        private readonly ILogger<DemoReseedService> _logger;

        private static readonly string[] AllowedEnvironments = ["Development", "Demo"];

        public DemoReseedService(
            IServiceScopeFactory scopeFactory,
            IHostEnvironment env,
            IOptions<DemoSettings> settings,
            ILogger<DemoReseedService> logger)
        {
            _scopeFactory = scopeFactory;
            _env = env;
            _settings = settings.Value;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (!AllowedEnvironments.Contains(_env.EnvironmentName))
            {
                _logger.LogInformation(
                    "DemoReseedService: not running in environment '{Environment}'. Allowed environments: {Allowed}",
                    _env.EnvironmentName, string.Join(", ", AllowedEnvironments));
                return;
            }

            if (!_settings.EnableNightlyReseed)
            {
                _logger.LogInformation("DemoReseedService: disabled via configuration (EnableNightlyReseed=false)");
                return;
            }

            while (!stoppingToken.IsCancellationRequested)
            {
                var delay = TimeUntilNextReseed();
                _logger.LogInformation("DemoReseedService: next reseed in {Delay}", delay);

                await Task.Delay(delay, stoppingToken);

                if (stoppingToken.IsCancellationRequested)
                    break;

                try
                {
                    await RunReseedAsync(stoppingToken);
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "DemoReseedService: reseed failed, will retry on next schedule");
                }
            }
        }

        private TimeSpan TimeUntilNextReseed()
        {
            var reseedTime = TimeOnly.Parse(_settings.ReseedTimeUtc);
            var now = DateTime.UtcNow;
            var next = now.Date.Add(reseedTime.ToTimeSpan());
            if (next <= now) next = next.AddDays(1);
            return next - now;
        }

        private async Task RunReseedAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("DemoReseedService: starting nightly reseed");

            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<DataContext>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();

            var demoUser = await context.Users.FirstOrDefaultAsync(
                u => u.UserName == "Demo User" && u.GlobalRole == "Guest", stoppingToken);
            if (demoUser == null)
            {
                _logger.LogWarning("DemoReseedService: no demo user found, skipping reseed");
                return;
            }

            await using var transaction = await context.Database.BeginTransactionAsync(stoppingToken);

            var demoUserComments = await context.Comments
                .Where(c => c.AuthorId == demoUser.Id)
                .ToListAsync(stoppingToken);
            context.Comments.RemoveRange(demoUserComments);

            var demoUserAttachments = await context.CommentAttachments
                .Where(a => a.UploadedById == demoUser.Id)
                .ToListAsync(stoppingToken);
            context.CommentAttachments.RemoveRange(demoUserAttachments);

            var notifications = await context.Notifications
                .Where(n => n.RecipientId == demoUser.Id)
                .ToListAsync(stoppingToken);
            context.Notifications.RemoveRange(notifications);

            // ProjectOwner is a denormalized username string, not a FK — sensitive to renames.
            var projects = await context.Projects
                .Where(p => p.ProjectOwner == demoUser.UserName)
                .ToListAsync(stoppingToken);
            context.Projects.RemoveRange(projects);

            await context.SaveChangesAsync(stoppingToken);
            await transaction.CommitAsync(stoppingToken);

            await DemoSeeder.SeedDemoUserAsync(userManager, _logger, stoppingToken);

            _logger.LogInformation("DemoReseedService: reseed complete");
        }
    }
}
