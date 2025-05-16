using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace PillarUtils.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        //public DbSet<Tweedle.Models.Rule> Rule { get; set; } = default!;
        public DbSet<PillarUtils.Models.ArchiveItem> ArchiveItem { get; set; } = default!;
        public DbSet<PillarUtils.Models.Client> Client { get; set; } = default!;
    }
}
