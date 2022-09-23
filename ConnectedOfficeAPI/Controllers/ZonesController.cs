using ConnectedOfficeAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ConnectedOfficeAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ZonesController : ControllerBase
    {
        private readonly ConnectedOfficeContexts _context;

        public ZonesController(ConnectedOfficeContexts context)
        {
            _context = context;
        }

        // GET: api/Zones
        [Authorize(Roles = "admin,user,superuser")]
        [HttpGet("GetAllZones")]
        public async Task<ActionResult<IEnumerable<Zone>>> GetZones()
        {
            if (_context.Zones == null)
            {
                return NotFound();
            }
            return await _context.Zones.ToListAsync();
        }

        // GET: api/Zones/5
        [Authorize(Roles = "admin,user,superuser")]
        [HttpGet("GetCertainZone/{ZoneId}")]
        public async Task<ActionResult<Zone>> GetZone(Guid ZoneId)
        {
            if (_context.Zones == null)
            {
                return NotFound();
            }
            var zone = await _context.Zones.FindAsync(ZoneId);

            if (zone == null)
            {
                return NotFound();
            }

            return zone;
        }

        // PUT: api/Zones/5
        [Authorize(Roles = "admin,superuser")]
        [HttpPut("UpdateZone/{ZoneId}")]
        public async Task<IActionResult> PutZone(Guid ZoneId, Zone zone)
        {
            if (ZoneId != zone.ZoneId)
            {
                return BadRequest();
            }

            _context.Entry(zone).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ZoneExists(ZoneId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Zones
        [Authorize(Roles = "admin,superuser")]
        [HttpPost("CreateZone")]
        public async Task<ActionResult<Zone>> PostZone(Zone zone)
        {
            if (_context.Zones == null)
            {
                return Problem("Entity set 'ConnectedOfficeContexts.Zones'  is null.");
            }
            _context.Zones.Add(zone);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (ZoneExists(zone.ZoneId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetZone", new { id = zone.ZoneId }, zone);
        }

        // DELETE: api/Zones/5
        [Authorize(Roles = "admin,superuser")]
        [HttpDelete("RemoveZone/{ZoneId}")]
        public async Task<IActionResult> DeleteZone(Guid ZoneId)
        {
            if (_context.Zones == null)
            {
                return NotFound();
            }
            var zone = await _context.Zones.FindAsync(ZoneId);
            if (zone == null)
            {
                return NotFound();
            }

            _context.Zones.Remove(zone);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ZoneExists(Guid id)
        {
            return (_context.Zones?.Any(e => e.ZoneId == id)).GetValueOrDefault();
        }
    }
}
