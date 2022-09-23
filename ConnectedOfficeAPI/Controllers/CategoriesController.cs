using ConnectedOfficeAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ConnectedOfficeAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ConnectedOfficeContexts _context;

        public CategoriesController(ConnectedOfficeContexts context)
        {
            _context = context;
        }

        // GET: api/Categories
        [Authorize(Roles = "admin,user,superuser")]
        [HttpGet("GetAllCategories")]
        public async Task<ActionResult<IEnumerable<Category>>> GetAllCategories()
        {
            if (_context.Categories == null)
            {
                return NotFound();
            }

            return await _context.Categories.ToListAsync();
        }

        // GET: api/Categories/5
        [Authorize(Roles = "admin,user,superuser")]
        [HttpGet("GetCertainCategory/{CategoryId}")]
        public async Task<ActionResult<Category>> GetSpecificCategory(Guid CategoryId)
        {
            if (_context.Categories == null)
            {
                return NotFound();
            }

            var category = await _context.Categories.FindAsync(CategoryId);

            if (category == null)
            {
                return NotFound();
            }

            return category;
        }

        [Authorize(Roles = "admin,user,superuser")]
        [HttpGet("GetAllDevicesInCertainCategory/{CategoryId}")]
        public async Task<ActionResult<IEnumerable<Device>>> GetDevicesInCategory(Guid CategoryId)
        {
            if (_context.Devices == null)
            {
                return NotFound();
            }
            var devices = await _context.Devices.Where(e => e.CategoryId == CategoryId).Select(e => e).ToListAsync();
            if (devices == null)
            {
                return NotFound();
            }
            return devices;
        }

        [Authorize(Roles = "admin,user,superuser")]
        [HttpGet("GetZoneCountsInCertainCategory/{CategoryId}")]
        public async Task<ActionResult<Device>> GetZoneCountInCategory(Guid CategoryId)
        {
            if (_context.Devices == null)
            {
                return NotFound();
            }
            var devices = await _context.Devices.Where(e => e.CategoryId == CategoryId).Select(e => e).ToListAsync();
            var CategoryName = await _context.Categories.Where(e => e.CategoryId == CategoryId).Select(e => e.CategoryName).ToListAsync();
            List<string> zones = new List<string>();  
            for(int i = 0; i < devices.ToList().Count; i++)
            {
                if (!(zones.Contains(devices[i].ZoneId.ToString())))
                {
                    zones.Add(devices[i].ZoneId.ToString());
                }
            }
            if (devices == null)
            {
                return NotFound();
            }
            return Content("Zone count for category " + CategoryName[0].ToString() + "-" + CategoryId.ToString() + " is: " + zones.Count().ToString());
        }

        // PUT: api/Categories/5
        [Authorize(Roles = "admin,superuser")]
        [HttpPut("UpdateCategory/{CategoryId}")]
        public async Task<IActionResult> PutCategory(Guid CategoryId, Category category)
        {
            if (CategoryId != category.CategoryId)
            {
                return BadRequest();
            }

            _context.Entry(category).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CategoryExists(CategoryId))
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

        // POST: api/Categories
        [Authorize(Roles = "admin,superuser")]
        [HttpPost("CreateCategory")]
        public async Task<ActionResult<Category>> PostCategory(Category category)
        {
            if (_context.Categories == null)
            {
                return Problem("Entity set 'ConnectedOfficeContexts.Categories'  is null.");
            }
            _context.Categories.Add(category);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (CategoryExists(category.CategoryId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetCategory", new { id = category.CategoryId }, category);
        }

        // DELETE: api/Categories/5
        [Authorize(Roles = "admin,superuser")]
        [HttpDelete("RemoveCategory/{CategoryId}")]
        public async Task<IActionResult> DeleteCategory(Guid CategoryId)
        {
            if (_context.Categories == null)
            {
                return NotFound();
            }
            var category = await _context.Categories.FindAsync(CategoryId);
            if (category == null)
            {
                return NotFound();
            }

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CategoryExists(Guid id)
        {
            return (_context.Categories?.Any(e => e.CategoryId == id)).GetValueOrDefault();
        }
    }
}
