using Ask_Clone.Models.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ask_Clone.Models
{
    public class AuthenticationContext : IdentityDbContext
    {
        public AuthenticationContext(DbContextOptions options) : base(options)
        { }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }

        public DbSet<ApplicationUser> ApplicationUsers {get;set;}
        public DbSet<Questions> Questions { get; set; }
        public DbSet<Follow> Follow { get; set; }
    }

    public class AuthenticationContextFactory: IDesignTimeDbContextFactory<AuthenticationContext>
    {
        public AuthenticationContext CreateDbContext(string[] args)
        {
            var options = new DbContextOptionsBuilder<AuthenticationContext>();
            options.UseSqlServer("Server = (localdb)\\MSSQLLocalDB; Database = Userdb; Trusted_Connection=True; MultipleActiveResultSets=True;  ");
            return new AuthenticationContext(options.Options);
        }
    }
}
