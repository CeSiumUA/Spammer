using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace TLSpammer.WEB.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public DbSet<CheckedChat> SelectedChats { get; set; }
        public DbSet<TimeOption> Times { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<TimeOption>().HasData(new TimeOption()
            {
                Id = 1,
                Time = DateTime.Now
            });
            base.OnModelCreating(builder);
        }
    }
}
