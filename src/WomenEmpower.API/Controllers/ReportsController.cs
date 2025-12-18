using Microsoft.AspNetCore.Mvc;
using WomenEmpower.API.DTOs;
using WomenEmpower.Core.Entities;
using WomenEmpower.Infrastructure.Data;

namespace WomenEmpower.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ReportsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("submit")]
        public async Task<IActionResult> SubmitReport([FromBody] ReportSubmissionDTO dto)
        {
            try
            {
                var report = new Report
                {
                    Title = dto.Title ?? "Untitled Reports",
                    Description = dto.Description ?? "No description provided",
                    Location = dto.Location ?? "Unknown",
                    IncidentType = dto.IncidentType ?? "General",
                    Category =dto.Category= "General",

                    //This generates a shoet code like "WE-A1B2"
                    AnonymousTrackingCode = "WE-" +Guid.NewGuid().ToString().Substring(0, 8).ToUpper(),
                    Status = ReportStatus.Pending,
                    CreatedAt = DateTime.UtcNow

                };

                _context.Reports.Add(report);
                await _context.SaveChangesAsync();

                return Ok(new { 
                    message = " Report submitted successfully!", 
                    reportId = report.Id,
                    trackingCode = report.AnonymousTrackingCode
                });

            }

            catch (Exception ex)
            {
                var realError = ex.InnerException?.Message ?? ex.Message;
                return StatusCode(500, $"Database Error: {realError}");
            }
        }
    }
}

