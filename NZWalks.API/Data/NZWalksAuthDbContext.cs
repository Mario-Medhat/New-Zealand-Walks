using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace NZWalks.API.Data
{
    public class NZWalksAuthDbContext : IdentityDbContext
    {
        public NZWalksAuthDbContext(DbContextOptions<NZWalksAuthDbContext> options) : base(options)
        {
        }
        //public DbSet<User> Users { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            var reaederId = "12345678-90ab-cdef-1234-567890abcdef";
            var writerId = "abcdef12-3456-7890-abcd-ef1234567890";

            var roles = new List<IdentityRole>
            {
                new IdentityRole
                {
                    Id = reaederId,
                    ConcurrencyStamp = reaederId,
                    Name = "Reader",
                    NormalizedName = "Reader".ToUpper()
                },

                new IdentityRole
                {
                    Id = writerId,
                    ConcurrencyStamp = writerId,
                    Name = "Writer",
                    NormalizedName = "Writer".ToUpper()
                },
            };

            modelBuilder.Entity<IdentityRole>().HasData(roles);
        }
    }
}
