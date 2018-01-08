using System;
using Microsoft.EntityFrameworkCore;
using Firelink.Todos.Entity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;

namespace Firelink.Todos.Persistence
{
    public class TodoDbContext : DbContext
    {
        public static readonly LoggerFactory MyLoggerFactory = new LoggerFactory(new[] {
            new ConsoleLoggerProvider((category, level) => level == LogLevel.Information, true)
        });
        public TodoDbContext(DbContextOptions<TodoDbContext> opts) : base(opts)
        { }

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) => optionsBuilder.UseLoggerFactory(MyLoggerFactory);

        public DbSet<Todo> Todos { get; set; }
    }
}
