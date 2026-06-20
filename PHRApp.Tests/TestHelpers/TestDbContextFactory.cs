using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using PHRApp.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PHRApp.Tests.TestHelpers
{
    public sealed class TestDbContextFactory : IDisposable
    {
        private readonly SqliteConnection _connection;
        public AppDbContext Context { get; }

        public TestDbContextFactory()
        {
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();

            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlite(_connection)
                .Options;

            Context = new AppDbContext(options);
            Context.Database.EnsureCreated();
        }

        public void Dispose()
        {
            Context.Dispose();
            _connection.Dispose();
        }
    }
}
