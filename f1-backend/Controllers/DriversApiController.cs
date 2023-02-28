using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Formula1TeamApp.Data;
using Formula1TeamApp.Models;
using Microsoft.AspNetCore.Cors;

namespace Formula1TeamApp.Controllers
{
    [Route("api/drivers")]
    [ApiController]

    // Enable CORS
    [EnableCors("AllowAllOrigins")]
    public class DriversApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public DriversApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Drivers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Driver>>> GetDrivers()
        {
            if (_context.Drivers == null)
            {
                return NotFound();
            }

            var drivers = await _context.Drivers.ToListAsync();

            // Get team name and file path
            foreach (var driver in drivers)
            {
                driver.TeamName = await _context.Teams.Where(t => t.TeamId == driver.TeamId).Select(t => t.Name).FirstOrDefaultAsync();
                
                // Get http path to wwwroot
                var wwwroot = $"{Request.Scheme}://{Request.Host}{Request.PathBase}";

                // Add absolute path to imageName
                driver.ImageName = wwwroot + "/driverimages/" + driver.ImageName;

                // Add webp-format
                string ImageFilename = driver.ImageName;
                // Remove extension
                driver.ImageWebp = ImageFilename.Substring(0, ImageFilename.LastIndexOf('.')) + ".webp";
            }

            return await _context.Drivers.ToListAsync();
        }

        // GET: api/Drivers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Driver>> GetDriver(int id)
        {
            if (_context.Drivers == null)
            {
                return NotFound();
            }
            var driver = await _context.Drivers.FindAsync(id);

            if (driver == null)
            {
                return NotFound();
            }

            // Get team name
            driver.TeamName = await _context.Teams.Where(t => t.TeamId == driver.TeamId).Select(t => t.Name).FirstOrDefaultAsync();

            return driver;
        }

        // Get drivers for team
        [HttpGet("team/{id}")]
        public async Task<ActionResult<IEnumerable<Driver>>> GetDriversForTeam(int id)
        {
            if (_context.Drivers == null)
            {
                return NotFound();
            }

            var drivers = await _context.Drivers.Where(d => d.TeamId == id).ToListAsync();

            // Get team name
            foreach (var driver in drivers)
            {
                driver.TeamName = await _context.Teams.Where(t => t.TeamId == driver.TeamId).Select(t => t.Name).FirstOrDefaultAsync();
            }

            return drivers;
        }        
    }
}
