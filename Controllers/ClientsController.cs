using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Azure;
using PillarUtils.Data;
using PillarUtils.Migrations;
using PillarUtils.Models;

namespace PillarUtils.Controllers
{
    public class ClientsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ClientsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Clients
        public async Task<IActionResult> Index()
        {
            return View(await _context.Client.ToListAsync());
        }

        public async Task<IActionResult> RenewalList()
        {
            var clientsWithOverdueItems = await _context.Client
                .Where(c => c.ArchiveItems.Any(ai => ai.RenewalDate <= DateTime.Now))
                .Include(c => c.ArchiveItems.Where(ai => ai.RenewalDate <= DateTime.Now))
                .AsNoTracking()
                .ToListAsync();
            var clientsWithOldNotifications = await _context.Client
                .Where(c => c.ArchiveItems.Any(ai => ai.NotificationDate <= DateTime.Now.AddMonths(-1)))
                .Include(c => c.ArchiveItems.Where(ai => ai.NotificationDate <= DateTime.Now.AddMonths(-1)))
                .AsNoTracking()
                .ToListAsync();

            RenewalListModel renewalModel = new();
            renewalModel.clientsWithOverdueItems = clientsWithOverdueItems;
            renewalModel.clientsWithOldNotifications = clientsWithOldNotifications;

            return View(renewalModel);
        }

        // GET: Clients/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var client = await _context.Client
                .Include(c => c.ArchiveItems)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);

            if (client == null)
            {
                return NotFound();
            }

            return View(client);
        }

        public async Task<IActionResult> Notify(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var client = await _context.Client
                .Include(c => c.ArchiveItems)
                .Include(c => c.Contacts)
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id);

            if (client == null)
            {
                return NotFound();
            }

            return View(client);
        }

        private void RenewItem(ArchiveItem itemToRenew)
        {
            if (itemToRenew != null)
            {
                if (itemToRenew.RenewalDate != null && itemToRenew.RenewalDate > DateTime.Now)
                {
                    itemToRenew.RenewalDate = itemToRenew.RenewalDate?.AddYears(3);
                }
                else
                {
                    itemToRenew.RenewalDate = DateTime.Now.AddYears(3);
                }
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RenewAll(int? clientId)
        {
            if (clientId == null)
            {
                return NotFound();
            }

            var archiveItems = await _context.ArchiveItem
                .Where(a => a.ClientId == clientId)
                .Where(a => a.isDeleted == false)
                .ToListAsync();

            foreach (var item in archiveItems)
            {
                RenewItem(item);
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Notify), new { id = clientId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RenewAllOverdue(int? clientId)
        {
            if (clientId == null)
            {
                return NotFound();
            }

            var overdueArchiveItems = await _context.ArchiveItem
                .Where(a => a.ClientId == clientId)
                .Where(a => a.isDeleted == false)
                .Where(ai => ai.RenewalDate <= DateTime.Now)
                .ToListAsync();

            foreach (var item in overdueArchiveItems)
            {
                RenewItem(item);
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Notify), new { id = clientId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> NotifyAllOverdue(int? clientId)
        {
            if (clientId == null)
            {
                return NotFound();
            }

            var overdueArchiveItems = await _context.ArchiveItem
                .Where(a => a.ClientId == clientId)
                .Where(a => a.isDeleted == false)
                .Where(ai => ai.RenewalDate <= DateTime.Now)
                .ToListAsync();

            var newNotifyDate = DateTime.Now;

            foreach (var item in overdueArchiveItems)
            {
                item.NotificationDate = newNotifyDate;
                item.NotificationSent = true;
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Notify), new { id = clientId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkAllForDeletion(int? clientId)
        {
            if (clientId == null)
            {
                return NotFound();
            }

            var archiveItems = await _context.ArchiveItem
                .Where(a => a.ClientId == clientId)
                .Where(a => a.isDeleted == false)
                .ToListAsync();


            foreach (var item in archiveItems)
            {
                item.ReadyToDelete = true;
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Notify), new { id = clientId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkAllOverdueForDeletion(int? clientId)
        {
            if (clientId == null)
            {
                return NotFound();
            }

            var overdueArchiveItems = await _context.ArchiveItem
                .Where(a => a.ClientId == clientId)
                .Where(a => a.isDeleted == false)
                .Where(ai => ai.RenewalDate <= DateTime.Now)
                .ToListAsync();

            foreach (var item in overdueArchiveItems)
            {
                item.ReadyToDelete = true;
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Notify), new { id = clientId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DuplicateLastContactAlphabetically(int? clientId)
        {
            if (clientId == null)
            {
                return NotFound();
            }

            var lastContact = _context.Contact
                .Where(c => c.ClientId == clientId)
                .OrderByDescending(c => c.LastName)
                .Take(1);

            // create a duplicate
            if (lastContact != null && lastContact.Any())
            {
                var contactToDuplicate = lastContact.First();
                Contact newContact = new()
                {
                    FirstName = contactToDuplicate.FirstName,
                    LastName = contactToDuplicate.LastName,
                    AvazaUserId = contactToDuplicate.AvazaUserId,
                    JobTitle = contactToDuplicate.JobTitle,
                    Email = contactToDuplicate.Email,
                    MobilePhone = contactToDuplicate.MobilePhone,
                    WorkPhone = contactToDuplicate.WorkPhone,
                    BillingAddress = contactToDuplicate.BillingAddress,
                    Comments = contactToDuplicate.Comments,
                    ClientId = clientId.Value
                };

                _context.Contact.Add(newContact);
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Notify), new { id = clientId });
        }

        // GET: Clients/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Clients/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,ClientCode")] Client client)
        {
            if (ModelState.IsValid)
            {
                _context.Add(client);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(client);
        }

        // GET: Clients/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var client = await _context.Client.FindAsync(id);
            if (client == null)
            {
                return NotFound();
            }
            return View(client);
        }

        // POST: Clients/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,ClientCode")] Client client)
        {
            if (id != client.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(client);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClientExists(client.Id))
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
            return View(client);
        }



        // GET: Clients/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var client = await _context.Client
                .FirstOrDefaultAsync(m => m.Id == id);

            if (client == null)
            {
                return NotFound();
            }

            return View(client);
        }

        // POST: Clients/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var client = await _context.Client.FindAsync(id);
            if (client != null)
            {
                _context.Client.Remove(client);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ClientExists(int id)
        {
            return _context.Client.Any(e => e.Id == id);
        }
        public async Task<IActionResult> QuickEditArchiveItem(int clientId, int archiveItemId, bool readyToDelete, bool isDeleted, bool notificationSent, bool fileChecked, bool renewUponSaving)
        {
            var client = await _context.Client
                .Include(c => c.ArchiveItems)
                .FirstOrDefaultAsync(c => c.Id == clientId);

            if (client == null)
            {
                return NotFound();
            }

            var archiveItem = client.ArchiveItems.FirstOrDefault(ai => ai.Id == archiveItemId);
            if (archiveItem == null)
            {
                return NotFound();
            }

            if (archiveItem.NotificationSent == false && notificationSent == true)
            {
                archiveItem.NotificationDate = DateTime.Now;
            }

            archiveItem.ReadyToDelete = readyToDelete;
            archiveItem.isDeleted = isDeleted;
            archiveItem.NotificationSent = notificationSent;
            archiveItem.FileChecked = fileChecked;

            if (renewUponSaving == true)
            {
                RenewItem(archiveItem);
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.ArchiveItem.Any(ai => ai.Id == archiveItemId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToAction(nameof(Notify), new { id = clientId });
        }
    }
}
