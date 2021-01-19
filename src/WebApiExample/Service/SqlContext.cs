
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json;

namespace WebApiExample
{

    public class KeyValue
    {
        [Key]
        public string Key { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public DateTime UpdateTime { get; set; } = DateTime.MinValue;
    }

    public class SqlContext : DbContext
    {
        public DbSet<KeyValue>? KeyValues { get; set; }

        public SqlContext(DbContextOptions options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            this.SetJsonConverter(modelBuilder);
        }
    }

}