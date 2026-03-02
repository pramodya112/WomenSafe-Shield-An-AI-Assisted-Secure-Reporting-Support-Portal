using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WomenEmpower.Infrastructure.Data;
using WomenEmpower.Core.Entities;

namespace WomenEmpower.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class IncidentsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public IncidentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> PostIncident([FromBody] IncidentModel report)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            _context.Incidents.Add(report);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Incident saved to database!" });
        }

        [HttpGet]
        public async Task<IActionResult> GetIncidents()
        {
            var incidents = await _context.Incidents.ToListAsync();
            return Ok(incidents);
        }
    }
}