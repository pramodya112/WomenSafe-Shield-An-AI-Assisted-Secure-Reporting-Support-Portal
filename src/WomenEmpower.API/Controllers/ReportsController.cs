using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        private readonly IWebHostEnvironment _env;

        public ReportsController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // ─────────────────────────────────────────────────────────────
        // POST api/Reports/submit
        // ─────────────────────────────────────────────────────────────
        [HttpPost("submit")]
        public async Task<IActionResult> SubmitReport([FromBody] ReportSubmissionDTO dto)
        {
            try
            {
                var sriLankaTime = TimeZoneInfo.ConvertTime(
                    DateTimeOffset.UtcNow,
                    TimeZoneInfo.FindSystemTimeZoneById("Asia/Colombo")
                );

                var report = new Report
                {
                    Title = dto.Title ?? "Untitled Report",
                    Description = dto.Description ?? "No description provided",
                    Location = dto.Location ?? "Unknown",
                    IncidentType = dto.IncidentType ?? "General",
                    Category = "General",
                    AnonymousTrackingCode = "WE-" + Guid.NewGuid().ToString()[..8].ToUpper(),
                    Status = ReportStatus.Pending,
                    CreatedAt = sriLankaTime,
                    ContactInfo = NullIfBlank(dto.ContactInfo),
                    VictimName = NullIfBlank(dto.VictimName),
                    VictimAge = dto.VictimAge,
                    ReporterType = dto.ReporterType ?? "victim",
                    Relationship = NullIfBlank(dto.Relationship),
                    IncidentDate = NullIfBlank(dto.IncidentDate),
                    IncidentTime = NullIfBlank(dto.IncidentTime),
                    Severity = string.IsNullOrWhiteSpace(dto.Severity) ? "moderate" : dto.Severity.Trim().ToLower(),
                    AdditionalNotes = NullIfBlank(dto.AdditionalNotes),
                };

                _context.Reports.Add(report);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    message = "Report submitted successfully!",
                    reportId = report.Id,
                    trackingCode = report.AnonymousTrackingCode
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Database Error: {ex.InnerException?.Message ?? ex.Message}");
            }
        }

        // ─────────────────────────────────────────────────────────────
        // POST api/Reports/{reportId}/evidence
        // ─────────────────────────────────────────────────────────────
        [HttpPost("{reportId:guid}/evidence")]
        [RequestSizeLimit(52_428_800)]
        public async Task<IActionResult> UploadEvidence(Guid reportId, [FromForm] IFormFileCollection files)
        {
            if (files == null || files.Count == 0)
                return BadRequest("No files were provided.");

            var report = await _context.Reports
                .Include(r => r.Evidence)
                .FirstOrDefaultAsync(r => r.Id == reportId);

            if (report == null)
                return NotFound("Report not found.");

            var uploadsRoot = Path.Combine(
                _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"),
                "uploads", "evidence", reportId.ToString()
            );
            Directory.CreateDirectory(uploadsRoot);

            var allowed = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                ".jpg", ".jpeg", ".png", ".gif", ".webp",
                ".mp4", ".mov", ".avi", ".mkv",
                ".pdf", ".doc", ".docx"
            };

            var savedFiles = new List<object>();

            foreach (var file in files)
            {
                if (file.Length == 0) continue;
                var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
                if (!allowed.Contains(ext))
                    return BadRequest($"File type '{ext}' is not permitted.");

                var storedName = $"{Guid.NewGuid()}{ext}";
                var fullPath = Path.Combine(uploadsRoot, storedName);

                await using (var stream = new FileStream(fullPath, FileMode.Create))
                    await file.CopyToAsync(stream);

                var evidence = new Evidence
                {
                    ReportId = reportId,
                    FileName = file.FileName,
                    StoredName = storedName,
                    FilePath = $"/uploads/evidence/{reportId}/{storedName}",
                    FileSize = file.Length,
                    ContentType = file.ContentType,
                    UploadedAt = DateTimeOffset.UtcNow,
                };

                _context.Evidences.Add(evidence);
                savedFiles.Add(new { evidence.FileName, evidence.FilePath, SizeKb = Math.Round(file.Length / 1024.0, 1) });
            }

            await _context.SaveChangesAsync();
            return Ok(new { message = $"{savedFiles.Count} file(s) uploaded successfully.", files = savedFiles });
        }

        // ─────────────────────────────────────────────────────────────
        // GET api/Reports/track/{code}
        // Public — used by the Track My Report page
        // ─────────────────────────────────────────────────────────────
        [HttpGet("track/{code}")]
        public async Task<IActionResult> TrackReport(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
                return BadRequest("Tracking code is required.");

            var report = await _context.Reports
                .Include(r => r.Evidence)
                .FirstOrDefaultAsync(r => r.AnonymousTrackingCode == code.Trim().ToUpper());

            if (report == null)
                return NotFound(new { message = $"No report found with code {code}." });

            return Ok(new
            {
                report.Id,
                report.Title,
                report.IncidentType,
                report.Status,
                StatusDisplay = report.Status.ToString(),
                report.CreatedAt,
                report.UpdatedAt,
                report.AdminNotes,
                report.Severity,
                report.IncidentDate,
                report.IncidentTime,
                report.Location,
                EvidenceCount = report.Evidence?.Count ?? 0,
            });
        }

        // ─────────────────────────────────────────────────────────────
        // GET api/Reports/all-reports  [Authorized]
        // ─────────────────────────────────────────────────────────────
        [HttpGet("all-reports")]
        [Authorize]
        public async Task<IActionResult> GetAllReports([FromQuery] string? status)
        {
            var query = _context.Reports
                .Include(r => r.Evidence)
                .AsQueryable();

            if (!string.IsNullOrEmpty(status))
                query = query.Where(r => r.Status.ToString() == status);

            var reports = await query
                .OrderByDescending(r => r.CreatedAt)
                .Select(r => new
                {
                    r.Id,
                    r.AnonymousTrackingCode,
                    r.Title,
                    r.Description,
                    r.IncidentType,
                    r.Location,
                    Status = r.Status.ToString(),
                    r.Severity,
                    r.CreatedAt,
                    r.UpdatedAt,
                    r.AdminNotes,
                    r.ContactInfo,
                    r.VictimName,
                    r.VictimAge,
                    r.ReporterType,
                    r.Relationship,
                    r.IncidentDate,
                    r.IncidentTime,
                    r.AdditionalNotes,
                    EvidenceCount = r.Evidence.Count
                })
                .ToListAsync();

            return Ok(reports);
        }

        // ─────────────────────────────────────────────────────────────
        // PUT api/Reports/{id}/status  [Authorized]
        // ─────────────────────────────────────────────────────────────
        [HttpPut("{id:guid}/status")]
        [Authorize]
        public async Task<IActionResult> UpdateReportStatus(Guid id, [FromBody] string newStatus)
        {
            var report = await _context.Reports.FindAsync(id);
            if (report == null) return NotFound("Report not found.");

            if (Enum.TryParse<ReportStatus>(newStatus, true, out var status))
            {
                report.Status = status;
                report.UpdatedAt = DateTimeOffset.UtcNow;
                await _context.SaveChangesAsync();
                return Ok(new { message = "Status updated successfully." });
            }

            return BadRequest("Invalid status. Use: Pending, UnderReview, AssignmentToGo, or Closed.");
        }

        // ─────────────────────────────────────────────────────────────
        // PUT api/Reports/{id}/notes  [Authorized]
        // ─────────────────────────────────────────────────────────────
        [HttpPut("{id:guid}/notes")]
        [Authorize]
        public async Task<IActionResult> UpdateAdminNotes(Guid id, [FromBody] string notes)
        {
            var report = await _context.Reports.FindAsync(id);
            if (report == null) return NotFound("Report not found.");

            report.AdminNotes = notes?.Trim();
            report.UpdatedAt = DateTimeOffset.UtcNow;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Admin notes saved successfully." });
        }

        // ─────────────────────────────────────────────────────────────
        // DELETE api/Reports/{id}  [Authorized]
        // ─────────────────────────────────────────────────────────────
        [HttpDelete("{id:guid}")]
        [Authorize]
        public async Task<IActionResult> DeleteReport(Guid id)
        {
            var report = await _context.Reports
                .Include(r => r.Evidence)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (report == null) return NotFound();

            foreach (var ev in report.Evidence)
            {
                var fullPath = Path.Combine(
                    _env.WebRootPath ?? "wwwroot",
                    ev.FilePath.TrimStart('/')
                );
                if (System.IO.File.Exists(fullPath))
                    System.IO.File.Delete(fullPath);
            }

            _context.Reports.Remove(report);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Report and all evidence deleted successfully." });
        }

        private static string? NullIfBlank(string? s)
            => string.IsNullOrWhiteSpace(s) ? null : s.Trim();
    }
}