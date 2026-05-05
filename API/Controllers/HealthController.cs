using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Persistence;
using System.Diagnostics;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HealthController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly ILogger<HealthController> _logger;

        public HealthController(DataContext context, ILogger<HealthController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet("health")]
        public IActionResult GetHealth()
        {
            try
            {
                // Check database connection
                _context.Database.CanConnect();
                
                return Ok(new
                {
                    status = "Healthy",
                    timestamp = DateTime.UtcNow,
                    version = "1.0.0",
                    uptime = GetUptime()
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Health check failed");
                return StatusCode(503, new
                {
                    status = "Unhealthy",
                    timestamp = DateTime.UtcNow,
                    error = ex.Message,
                    uptime = GetUptime()
                });
            }
        }

        [HttpGet("health/ready")]
        public IActionResult GetReadiness()
        {
            try
            {
                // Check if all critical services are ready
                _context.Database.CanConnect();
                
                return Ok(new
                {
                    status = "Ready",
                    timestamp = DateTime.UtcNow,
                    checks = new
                    {
                        database = "Healthy",
                        api = "Healthy"
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Readiness check failed");
                return StatusCode(503, new
                {
                    status = "Not Ready",
                    timestamp = DateTime.UtcNow,
                    error = ex.Message,
                    checks = new
                    {
                        database = "Unhealthy",
                        api = "Unhealthy"
                    }
                });
            }
        }

        private string GetUptime()
        {
            var process = Process.GetCurrentProcess();
            var startTime = process.StartTime;
            var uptime = DateTime.UtcNow - startTime;
            return uptime.ToString(@"dd\.hh\:mm\:ss");
        }
    }
}