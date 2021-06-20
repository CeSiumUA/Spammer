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
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
    }
}
