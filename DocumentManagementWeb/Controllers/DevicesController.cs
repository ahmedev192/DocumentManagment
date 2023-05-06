using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DocumentManagementWeb.Data;
using DocumentManagementWeb.Models;
using DocumentManagementWeb.VM;
using Microsoft.AspNetCore.Hosting;
using DocumentManagementWeb.Utility;
using Microsoft.AspNetCore.Authorization;
using System.Data;

namespace DocumentManagementWeb.Controllers
{
    public class DevicesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DevicesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Devices

        public async Task<IActionResult> Index()
        {
            return _context.Devices != null ?
                        View(await _context.Devices.ToListAsync()) :
                        Problem("Entity set 'ApplicationDbContext.Devices'  is null.");
        }

        // GET: Devices/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Devices == null)
            {
                return NotFound();
            }

            var device = await _context.Devices
                .FirstOrDefaultAsync(m => m.Id == id);
            if (device == null)
            {
                return NotFound();
            }

            return View(device);
        }

        // GET: Devices/Create
        [Authorize(Roles = SD.Role_Admin)]

        public IActionResult Create()
        {
            return View();
        }

        // POST: Devices/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = SD.Role_Admin)]

        public async Task<IActionResult> Create([Bind("Id,IP,Name,CreatedDateTime,UpdatedDateTime")] Device device)
        {
            if (ModelState.IsValid)
            {
                device.CreatedDateTime= DateTime.Now;
                device.UpdatedDateTime= DateTime.Now;
                _context.Add(device);
                await _context.SaveChangesAsync();
                var emails = _context.Users.ToList();
                foreach(var email in emails)
                {
                    Notification n = new Notification()
                    {
                        applicationUser = email,

                        Msg = $" A New Device Was Added with IP = {device.IP} And Name = {device.Name} At {device.CreatedDateTime}"
                    };
                    _context.Notifications.Add(n);
                    _context.SaveChanges();
                }

                return RedirectToAction(nameof(Index));
            }
            return View(device);
        }

        // GET: Devices/Edit/5
        [Authorize(Roles = SD.Role_Admin)]

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Devices == null)
            {
                return NotFound();
            }

            var device = await _context.Devices.FindAsync(id);
            if (device == null)
            {
                return NotFound();
            }
            return View(device);
        }

        // POST: Devices/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = SD.Role_Admin)]

        public async Task<IActionResult> Edit(int id, [Bind("Id,IP,Name,CreatedDateTime,UpdatedDateTime")] Device device)
        {
            if (id != device.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    device.UpdatedDateTime = DateTime.Now;
                    _context.Update(device);
                    await _context.SaveChangesAsync();
                    var emails = _context.Users.ToList();
                    foreach (var email in emails)
                    {
                        Notification n = new Notification()
                        {
                            applicationUser = email,

                            Msg = $" A Device Was Updated Now with IP = {device.IP} And Name = {device.Name} At {device.UpdatedDateTime}"
                        };
                        _context.Notifications.Add(n);
                        _context.SaveChanges();
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DeviceExists(device.Id))
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
            return View(device);
        }

        // GET: Devices/Delete/5
        [Authorize(Roles = SD.Role_Admin)]

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Devices == null)
            {
                return NotFound();
            }

            var device = await _context.Devices
                .FirstOrDefaultAsync(m => m.Id == id);
            if (device == null)
            {
                return NotFound();
            }

            return View(device);
        }

        // POST: Devices/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = SD.Role_Admin)]

        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Devices == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Devices'  is null.");
            }
            var device = await _context.Devices.FindAsync(id);
            if (device != null)
            {
                _context.Devices.Remove(device);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DeviceExists(int id)
        {
            return (_context.Devices?.Any(e => e.Id == id)).GetValueOrDefault();
        }


        #region API CALLS
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {

            var tmp = await _context.Devices.ToListAsync();

            return Json(new
            {
                Data = tmp,
            });
        }
        #endregion



    }
}
