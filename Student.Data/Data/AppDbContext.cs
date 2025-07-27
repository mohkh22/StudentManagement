
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Studentmanage.Entities.Models;

namespace Studentmanage.Data.Data
{
    public class AppDbContext: IdentityDbContext<IdentityUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
           
        }

        public DbSet<Studentmanage.Entities.Models.Student> Students { get; set; }
        public DbSet<AppUser> AppUsers { get; set; }

    }
}
