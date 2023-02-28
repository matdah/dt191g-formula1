using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Formula1TeamApp.Data;
using Formula1TeamApp.Models;

namespace Formula1TeamApp.Controllers
{
    [ApiController]
    [Route("api/teams")]
    public class TeamsApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TeamsApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/TeamsApi        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Team>>> GetTeams()
        {
            if (_context.Teams == null)
            {
                return NotFound();
            }

            // Get drivers for team
            var teams = await _context.Teams.Include(t => t.Drivers).ToListAsync();
            
            return teams;
        }

        // GET: api/TeamsApi/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Team>> GetTeam(int id)
        {
            if (_context.Teams == null)
            {
                return NotFound();
            }
            var team = await _context.Teams.FindAsync(id);

            if (team == null)
            {
                return NotFound();
            }

            // Get drivers for team
            team.Drivers = await _context.Drivers.Where(d => d.TeamId == id).ToListAsync();

            return team;
        }

        private bool TeamExists(int id)
        {
            return (_context.Teams?.Any(e => e.TeamId == id)).GetValueOrDefault();
        }
    }
}
