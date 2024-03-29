﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Formula1TeamApp.Data;
using Formula1TeamApp.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Authorization;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace Formula1TeamApp.Controllers
{
    [Authorize]
    public class DriversController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;
        private string wwwRootPath;

        public DriversController(ApplicationDbContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
            wwwRootPath = _hostEnvironment.WebRootPath;
        }

        // GET: Drivers
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Drivers.Include(d => d.Team);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Drivers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Drivers == null)
            {
                return NotFound();
            }

            var driver = await _context.Drivers
                .Include(d => d.Team)
                .FirstOrDefaultAsync(m => m.DriverId == id);
            if (driver == null)
            {
                return NotFound();
            }

            return View(driver);
        }

        // Test
        public IActionResult Test()
        {
            var drivers = _context.Drivers
                .Where(c => c.Salary > 199)
                .Select(c => c)
                .ToList();
            
            // Get driver by id
            var driver = _context.Drivers
                .Where(c => c.DriverId == 4)
                .Select(c => c)
                .FirstOrDefault();
            
            // Add to list
            if(driver != null) {
                drivers.Add(driver);
            }           

            return View(drivers);
        }

        // GET: Drivers/Create
        public IActionResult Create()
        {
            ViewData["TeamId"] = new SelectList(_context.Teams, "TeamId", "Name");
            return View();
        }

        // POST: Drivers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("DriverId,Name,Salary,ImageFile,TeamId")] Driver driver)
        {
            if (ModelState.IsValid)
            {
                // Image or not?
                if (driver.ImageFile != null)
                {
                    //Spara bilder till wwwroot / imageupload
                    string fileName = Path.GetFileNameWithoutExtension(driver.ImageFile.FileName);
                    string extension = Path.GetExtension(driver.ImageFile.FileName);

                    // Plockar bort mellanslag i filnamn, och lägger till tidsträng
                    driver.ImageName = fileName = fileName.Replace(" ", String.Empty) + DateTime.Now.ToString("yymmssfff") + extension;

                    string path = Path.Combine(wwwRootPath + "/driverimages/", fileName);

                    // Lagra fil
                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        await driver.ImageFile.CopyToAsync(fileStream);
                    }

                    // Skapa miniatyrer
                    CreateImageFiles(fileName);
                }
                else
                {
                    driver.ImageName = "empty.jpg";
                }

                _context.Add(driver);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["TeamId"] = new SelectList(_context.Teams, "TeamId", "Name", driver.TeamId);
            return View(driver);
        }

        // GET: Drivers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Drivers == null)
            {
                return NotFound();
            }

            var driver = await _context.Drivers.FindAsync(id);
            if (driver == null)
            {
                return NotFound();
            }
            ViewData["TeamId"] = new SelectList(_context.Teams, "TeamId", "Name", driver.TeamId);
            return View(driver);
        }

        // POST: Drivers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("DriverId,Name,Salary,ImageName,TeamId")] Driver driver)
        {
            if (id != driver.DriverId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(driver);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DriverExists(driver.DriverId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["TeamId"] = new SelectList(_context.Teams, "TeamId", "Name", driver.TeamId);
            return View(driver);
        }

        // GET: Drivers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Drivers == null)
            {
                return NotFound();
            }

            var driver = await _context.Drivers
                .Include(d => d.Team)
                .FirstOrDefaultAsync(m => m.DriverId == id);
            if (driver == null)
            {
                return NotFound();
            }

            return View(driver);
        }

        // POST: Drivers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Drivers == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Drivers'  is null.");
            }
            var driver = await _context.Drivers.FindAsync(id);
            if (driver != null)
            {
                _context.Drivers.Remove(driver);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // Bildmanipulering
        private void CreateImageFiles(string filename)
        {     
            // Sökväg till bildkatalog      
            string imagePath = wwwRootPath + "/driverimages/";

            // Skapa miniatyr
            using var image = Image.Load(imagePath + filename);
            image.Mutate(x => x.Resize(image.Width / 2, image.Height / 2));
            image.Save(imagePath + "thumb_" + filename);

            // Konvertera originalet till webp
            using var image_webp = Image.Load(imagePath + filename);
            // Skapa ett vettigt filnamn
            string webp_filename = filename.Substring(0, filename.LastIndexOf(".", StringComparison.Ordinal)) + ".webp";
            image.SaveAsWebp(imagePath + webp_filename);
        }

        private bool DriverExists(int id)
        {
            return (_context.Drivers?.Any(e => e.DriverId == id)).GetValueOrDefault();
        }
    }
}
