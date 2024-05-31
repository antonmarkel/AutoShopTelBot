using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramBot.Data
{
    public class BotDataContext : DbContext
    {
        public DbSet<PurchaseModel> Purchases { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

        }
        public BotDataContext() : base()
        {
            //Database.EnsureDeleted();
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

            string connectionString_localServer = "Server=localhost;Port=3307;Database=testdb;user=root;Password=1111;";

            optionsBuilder.UseMySql(connectionString_localServer, ServerVersion.AutoDetect(connectionString_localServer));
        }
    }
}
