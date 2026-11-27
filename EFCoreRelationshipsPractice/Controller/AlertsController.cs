
using EFCorePractice.Models;
using EFCoreRelationshipsPractice.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EFCoreRelationshipsPractice.Data;

namespace EFCorePractice.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AlertsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AlertsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("batch")]
        public async Task<IActionResult> InsertAlerts(
            List<Alert> alerts)
        {
            if (alerts == null || alerts.Count == 0)
            {
                return BadRequest("No alerts provided.");
            }

            foreach (var alert in alerts)
            {
                alert.Id = 0;

                if (alert.CreatedAt == default)
                {
                    alert.CreatedAt = DateTime.UtcNow;
                }
            }

            _context.Alerts.AddRange(alerts);

            await _context.SaveChangesAsync();

            return Ok(new
            {
                Message = "Alerts inserted successfully",
                TotalInserted = alerts.Count
            });
        }

        [HttpGet("by-time")]
        public async Task<IActionResult> GetAlertsByTime(
          DateTime from,
            DateTime to)
        {
            var alerts = await _context.Alerts
                .AsNoTracking()
                .Where(a =>
                    a.CreatedAt >= from &&
                    a.CreatedAt < to)
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();

            return Ok(alerts);
        }

        [HttpGet("critical-count")]
        public async Task<IActionResult> GetCriticalAlertCount()
        {
            var count = await _context.Alerts
                .CountAsync(a => a.Severity == "Critical");

            return Ok(new
            {
                CriticalAlerts = count
            });
        }

    }
}