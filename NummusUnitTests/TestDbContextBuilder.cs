using System;
using Microsoft.EntityFrameworkCore;
using Nummus.Data;

namespace NummusUnitTests {
    public class TestDbContextBuilder  {
        public static NummusDbContext InMemoryContext() {
            var options = new DbContextOptionsBuilder<NummusDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .EnableSensitiveDataLogging()
                .Options;
            var context = new NummusDbContext(options);
            return context;
        }
    }
}