using Microsoft.AspNetCore.Identity;
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
        public DbSet<TextData> TextDatas { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
            this.Database.Migrate();
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            var hasher = new PasswordHasher<IdentityUser>();
            builder.Entity<IdentityUser>().HasData(new IdentityUser
            {
                Id = Guid.NewGuid().ToString(),
                UserName = "admin@admin.com",
                NormalizedUserName = "ADMIN@ADMIN.COM",
                Email = "admin@admin.com",
                NormalizedEmail = "ADMIN@ADMIN.COM",
                EmailConfirmed = true,
                PasswordHash = hasher.HashPassword(null, "12345Aa-"),
                SecurityStamp = string.Empty
            });
            builder.Entity<TimeOption>().HasData(new TimeOption()
            {
                Id = 1,
                Time = DateTime.Now
            });
            builder.Entity<TextData>().HasData(new TextData()
            {
                Id = 1,
                Text = string.Empty
            });
            base.OnModelCreating(builder);
        }
    }
}
