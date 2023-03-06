using Formula1TeamApp.Data;
using Formula1TeamApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Formula1TeamApp.Controllers {
    [ApiController]
    [Route("api/teams")]
    public class TestAPIController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TestAPIController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/TeamsApi
        [HttpGet]
        [Route("test")]
        public IActionResult Test()
        {           
            var drivers = _context.Drivers.FromSql($"SELECT * FROM Drivers").ToList();
            var miniTeam = new List<MiniDriver>();

            foreach (var driver in drivers)
            {
                // Check image
                if (driver.ImageName == null)
                {
                    driver.ImageName = "empty.jpg";
                }

                // Get team name
                if (driver.TeamId != null)
                {
                    var teamName = _context.Teams.FromSql($"SELECT * FROM Teams WHERE TeamId = {driver.TeamId}").FirstOrDefault();
                    
                    if(teamName != null) {
                        driver.TeamName = teamName.Name;
                    } else {
                        driver.TeamName = "Okänd";
                    }
                }

                if(driver.TeamName == null) {
                    driver.TeamName = "Okänd";
                }

                miniTeam.Add(new MiniDriver
                {
                    Id = driver.DriverId,
                    Name = driver.Name,
                    Image = driver.ImageName,
                    Team = driver.TeamName
                });                
            }
            return Ok(miniTeam);


        }
    }
}