using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PillarUtils.Data;
using PillarUtils.Models;

namespace PillarUtils.Controllers
{
    public class ArchiveItemsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ArchiveItemsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: ArchiveItems
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.ArchiveItem.Include(a => a.Client);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: ArchiveItems/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var archiveItem = await _context.ArchiveItem
                .Include(a => a.Client)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (archiveItem == null)
            {
                return NotFound();
            }

            return View(archiveItem);
        }

        // GET: ArchiveItems/Create
        public IActionResult Create()
        {
            ViewData["ClientId"] = new SelectList(_context.Client, "Id", "Name");
            return View();
        }

        // POST: ArchiveItems/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,FolderName,ImportSourcePath,FileFormat,SourceDate,ClientId,FileChecked,NotificationSent,RenewalDate,ReadyToDelete,isDeleted,Format,Codec,Duration")] ArchiveItem archiveItem)
        {
            if (ModelState.IsValid)
            {
                _context.Add(archiveItem);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ClientId"] = new SelectList(_context.Client, "Id", "Name", archiveItem.ClientId);
            return View(archiveItem);
        }

        // GET: ArchiveItems/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var archiveItem = await _context.ArchiveItem.FindAsync(id);
            if (archiveItem == null)
            {
                return NotFound();
            }
            ViewData["ClientId"] = new SelectList(_context.Client, "Id", "Name", archiveItem.ClientId);
            return View(archiveItem);
        }

        // POST: ArchiveItems/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,FolderName,ImportSourcePath,FileFormat,SourceDate,ClientId,FileChecked,NotificationSent,RenewalDate,ReadyToDelete,isDeleted,Format,Codec,Duration")] ArchiveItem archiveItem)
        {
            if (id != archiveItem.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(archiveItem);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ArchiveItemExists(archiveItem.Id))
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
            ViewData["ClientId"] = new SelectList(_context.Client, "Id", "Name", archiveItem.ClientId);
            return View(archiveItem);
        }

        // GET: ArchiveItems/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var archiveItem = await _context.ArchiveItem
                .Include(a => a.Client)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (archiveItem == null)
            {
                return NotFound();
            }

            return View(archiveItem);
        }

        // POST: ArchiveItems/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var archiveItem = await _context.ArchiveItem.FindAsync(id);
            if (archiveItem != null)
            {
                _context.ArchiveItem.Remove(archiveItem);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ArchiveItemExists(int id)
        {
            return _context.ArchiveItem.Any(e => e.Id == id);
        }
    }
}
