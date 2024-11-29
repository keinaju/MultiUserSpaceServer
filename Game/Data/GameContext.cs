using Microsoft.EntityFrameworkCore;
using MUS.Game.Data.Models;

namespace MUS.Game.Data
{
    public class GameContext : DbContext
    {
        public DbSet<Being> Beings { get; set; }
        public DbSet<GameSettings> GameSettings { get; set; }
        public DbSet<ItemGenerator> ItemGenerators { get; set; }
        public DbSet<Inventory> Inventories { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<ItemStack> ItemsStacks { get; set; }
        public DbSet<Offer> Offers { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<RoomPool> RoomPools { get; set; }
        public DbSet<TickCounter> TickCounter { get; set; }
        public DbSet<User> Users { get; set; }

        public GameContext(
            DbContextOptions<GameContext> options
        )
        : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ItemStack>()
                .HasKey(table => new { table.ItemPrimaryKey, table.InventoryPrimaryKey });
        }
    }
}