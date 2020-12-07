using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nummus.Data {
    public class NummusDbContext : IdentityDbContext {
        public NummusDbContext(DbContextOptions<NummusDbContext> options)
            : base(options) {
        }
        
        public NummusDbContext() { }

        public DbSet<Account> Accounts { get; set; }
        public DbSet<AccountStatement> AccountStatements { get; set; }
        public DbSet<BookingLine> BookingLines { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<NummusUser> NummusUsers { get; set; }
    }
}
