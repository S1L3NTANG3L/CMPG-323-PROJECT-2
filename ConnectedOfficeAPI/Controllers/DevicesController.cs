using ConnectedOfficeAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Policy;

namespace ConnectedOfficeAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class DevicesController : ControllerBase
    {
        private readonly ConnectedOfficeContexts _context;

        public DevicesController(ConnectedOfficeContexts context)
        {
            _context = context;
        }

        // GET: api/Devices
        [Authorize(Roles = "admin,user,superuser")]
        [HttpGet("GetAllDevices")]
        public async Task<ActionResult<IEnumerable<Device>>> GetDevices()
        {
            if (_context.Devices == null)
            {
                return NotFound();
            }
            return await _context.Devices.ToListAsync();
        }

        // GET: api/Devices/5
        [Authorize(Roles = "admin,user,superuser")]
        [HttpGet("GetCertainDevices/{DeviceId}")]
        public async Task<ActionResult<Device>> GetDevice(Guid DeviceId)
        {
            if (_context.Devices == null)
            {
                return NotFound();
            }
            var device = await _context.Devices.FindAsync(DeviceId);

            if (device == null)
            {
                return NotFound();
            }

            return device;
        }

        // GET: api/Devices/ZoneId
        [Authorize(Roles = "admin,user,superuser")]
        [HttpGet("GetAllDevicesInCertainZone/{ZoneId}")]
        public async Task<ActionResult<IEnumerable<Device>>> GetZone(Guid ZoneId)
{
            if (_context.Devices == null)
            {
                return NotFound();
            }
            var devices = await _context.Devices.Where(e => e.ZoneId == ZoneId).ToListAsync();
            if( devices == null)
            {
                return NotFound();
            }
            return devices;
        }        

        // PUT: api/Devices/5
        [Authorize(Roles = "admin,superuser")]
        [HttpPut("UpdateDevice/{DeviceId}")]
        public async Task<IActionResult> PutDevice(Guid DeviceId, Device device)
        {
            if (DeviceId != device.DeviceId)
            {
                return BadRequest();
            }

            _context.Entry(device).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DeviceExists(DeviceId))
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

        // POST: api/Devices
        [Authorize(Roles = "admin,superuser")]
        [HttpPost("CreateDevice")]
        public async Task<ActionResult<Device>> PostDevice(Device device)
        {
            if (_context.Devices == null)
            {
                return Problem("Entity set 'ConnectedOfficeContexts.Devices'  is null.");
            }
            _context.Devices.Add(device);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (DeviceExists(device.DeviceId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetDevice", new { id = device.DeviceId }, device);
        }

        // DELETE: api/Devices/5
        [Authorize(Roles = "admin,superuser")]
        [HttpDelete("RemoveDevice/{DeviceId}")]
        public async Task<IActionResult> DeleteDevice(Guid DeviceId)
        {
            if (_context.Devices == null)
            {
                return NotFound();
            }
            var device = await _context.Devices.FindAsync(DeviceId);
            if (device == null)
            {
                return NotFound();
            }

            _context.Devices.Remove(device);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool DeviceExists(Guid id)
        {
            return (_context.Devices?.Any(e => e.DeviceId == id)).GetValueOrDefault();
        }
    }
}
