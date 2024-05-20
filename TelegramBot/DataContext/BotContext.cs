using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using TelegramBot.Items;
using TelegramBot.User;

namespace TelegramBot.DataContext
{
    public class BotContext : DbContext
    {
        public DbSet<Category> Categories { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<Picture> Pictures { get; set; }
        public DbSet<UserData> Users { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

        }
        public BotContext() : base()
        {
            Database.EnsureDeleted();
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string connectionString_localhost = "Server=localhost;Database=testdb3;user=root;Password=1111;";
            string connectionString_dedicatedServer = "Server=vultr-prod-71909da1-e560-42b9-87a4-e129adcf43cd-vultr-prod-a2dd.vultrdb.com;Port=16751;Database=telpurchases;user=vultradmin;Password=AVNS_UknBjA2afETCB9rSRx2;";
            optionsBuilder.UseMySql(connectionString_localhost, ServerVersion.AutoDetect(connectionString_dedicatedServer));
        }
    }
}

