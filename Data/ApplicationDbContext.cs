using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PillarUtils.Models;

namespace PillarUtils.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        //public DbSet<Tweedle.Models.Rule> Rule { get; set; } = default!;
        public DbSet<ArchiveItem> ArchiveItem { get; set; } = default!;
        public DbSet<Client> Client { get; set; } = default!;
        public DbSet<Contact> Contact { get; set; } = default!;
    }
}
