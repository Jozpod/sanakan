using Microsoft.EntityFrameworkCore;
using Sanakan.ShindenApi.Fake.Models;

namespace Sanakan.ShindenApi.Fake
{
    public class WebScrapedDbContext : DbContext
    {
        public WebScrapedDbContext(DbContextOptions<WebScrapedDbContext> options)
            : base(options)
        {
        }

        public DbSet<Illustration> Illustrations { get; set; } = null!;

        public DbSet<Character> Characters { get; set; } = null!;

        public DbSet<ScrapeStats> ScrapeStats { get; set; } = null!;
    }
}
