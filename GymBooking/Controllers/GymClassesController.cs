using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GymBooking.Data;
using GymBooking.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace GymBooking.Controllers
{
    [Authorize]
    public class GymClassesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public GymClassesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: GymClasses
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            return View(await _context.GymClasses.Where(g => g.StartTime > DateTime.Now)
                .Include(c => c.AttendingMembers.Where(a => a.GymClassId == a.GymClass.Id))
                .ToListAsync());
        }

        [AllowAnonymous]
        public async Task<IActionResult> Passed()
        {
            return View(nameof(Index), await _context.GymClasses
                .Include(c => c.AttendingMembers.Where(a => a.GymClassId == a.GymClass.Id))
                .ToListAsync());
        }

        public async Task<IActionResult> BookedPass(string id)
        {
            var x = await _context.ApplicationUserGymClasses.Where(a => a.ApplicationUserId == id && a.GymClass.StartTime > DateTime.Now).Include(g => g.GymClass).ToListAsync();

            return View(nameof(BookedPass), x);
        }

        public async Task<IActionResult> History(string id)
        {
            var x = await _context.ApplicationUserGymClasses.Where(a => a.ApplicationUserId == id && a.GymClass.StartTime <= DateTime.Now).Include(g => g.GymClass).ToListAsync();

            return View(nameof(History), x);
        }

        // GET: GymClasses/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return RedirectToAction(nameof(Index));

            var gymClassWithAttendees = await _context.GymClasses
                .Where(g => g.Id == id)
                .Include(c => c.AttendingMembers)
                .ThenInclude(u => u.User).FirstOrDefaultAsync();

            if (gymClassWithAttendees == null)
                return RedirectToAction(nameof(Index));

            return View(gymClassWithAttendees);
        }

        // GET: GymClasses/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: GymClasses/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("Id,Name,StartTime,Duration,Description")] GymClass gymClass)
        {
            if (ModelState.IsValid)
            {
                _context.Add(gymClass);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(gymClass);
        }

        // GET: GymClasses/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var gymClass = await _context.GymClasses.FindAsync(id);
            if (gymClass == null)
            {
                return NotFound();
            }
            return View(gymClass);
        }

        // POST: GymClasses/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,StartTime,Duration,Description")] GymClass gymClass)
        {
            if (id != gymClass.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(gymClass);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GymClassExists(gymClass.Id))
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
            return View(gymClass);
        }

        // GET: GymClasses/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var gymClass = await _context.GymClasses
                .FirstOrDefaultAsync(m => m.Id == id);
            if (gymClass == null)
            {
                return NotFound();
            }

            return View(gymClass);
        }

        // POST: GymClasses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var gymClass = await _context.GymClasses.FindAsync(id);
            if (gymClass != null)
            {
                _context.GymClasses.Remove(gymClass);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool GymClassExists(int id)
        {
            return _context.GymClasses.Any(e => e.Id == id);
        }

        [Authorize]
        public async Task<IActionResult> BookingToogle(int? id)
        {
            if(id == null) return NotFound();

            var userId = _userManager.GetUserId(User);

            var attending = await _context.ApplicationUserGymClasses.FindAsync(userId, id);

            if (attending == null)
            {
                var booking = new ApplicationUserGymClass
                {
                    ApplicationUserId = userId,
                    GymClassId = (int)id
                };
                _context.ApplicationUserGymClasses.Add(booking);
            }
            else
            {
                _context.Remove(attending);
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
