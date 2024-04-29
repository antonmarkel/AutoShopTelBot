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
        public DbSet<Purchase> Purchases { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

        }
        public BotDataContext() : base()
        {
            Database.EnsureDeleted();
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

            string connectionString_dedicatedServer = "Server=vultr-prod-71909da1-e560-42b9-87a4-e129adcf43cd-vultr-prod-a2dd.vultrdb.com;Port=16751;Database=telpurchases;user=vultradmin;Password=AVNS_UknBjA2afETCB9rSRx2;";

            optionsBuilder.UseMySql(connectionString_dedicatedServer, ServerVersion.AutoDetect(connectionString_dedicatedServer));
        }
    }
}
