using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using MUS.Game.Commands;
using MUS.Game.Data.Models;
using MUS.Game.Utilities;
using static MUS.Game.Commands.CommandResult;

namespace MUS.Game.Data
{
    public class GameContext : DbContext
    {
        public DbSet<Being> Beings { get; set; }
        public DbSet<CraftPlan> CraftPlans { get; set; }
        public DbSet<Feature> Features { get; set; }
        public DbSet<GameSettings> GameSettings { get; set; }
        public DbSet<ItemHatcher> ItemHatchers { get; set; }
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

        public async Task<bool> BeingNameIsReserved(string beingName)
        {
            return await Beings.AnyAsync(
                being => being.Name == beingName
            );
        }

        public async Task<CommandResult> CreateRoomPool(string inputName)
        {
            var validationResult = TextSanitation.ValidateName(inputName);
            if(validationResult.GetStatus() == StatusCode.Fail)
            {
                return validationResult;
            }

            var cleanName = TextSanitation.GetCleanName(inputName);
            if(await RoomPoolNameIsReserved(cleanName))
            {
                return NameIsReserved("room pool", cleanName);
            }

            await RoomPools.AddAsync(
                new RoomPool()
                {
                    Description = null,
                    FeeItem = null,
                    Name = cleanName
                }
            );

            await SaveChangesAsync();

            return new CommandResult(StatusCode.Success)
            .AddMessage(Message.Created("room pool", cleanName));
        }

        public async Task<CommandResult> CreateUser(User user)
        {
            await Users.AddAsync(user);

            var settings = await GetGameSettings();

            user.CreatedBeings.Add(
                new Being()
                {
                    CreatedByUser = user,
                    FreeInventory = new Inventory(),
                    InRoom = settings.DefaultSpawnRoom,
                    Name = await GetUniqueBeingName("BEING #"),
                    TradeInventory = new Inventory()
                }
            );

            await SaveChangesAsync();

            return new CommandResult(StatusCode.Success)
            .AddMessage(Message.Created("user", user.Username));
        }

        public async Task<CommandResult> DeleteFeature(string featureName)
        {
            var feature = await FindFeature(featureName);

            if(feature is not null)
            {
                Features.Remove(feature);

                await SaveChangesAsync();

                return new CommandResult(StatusCode.Success)
                .AddMessage(Message.Deleted("feature", feature.Name));
            }
            else
            {
                return FeatureDoesNotExist(featureName);
            }
        }

        public async Task<CommandResult> DeleteItem(string itemName)
        {
            var item = await FindItem(itemName);

            if(item is not null)
            {
                Items.Remove(item);

                await SaveChangesAsync();

                return new CommandResult(StatusCode.Success)
                .AddMessage(Message.Deleted("item", item.Name));
            }
            else
            {
                return ItemDoesNotExist(itemName);
            }
        }

        public async Task<CommandResult> DeleteRoom(string roomName)
        {
            var room = await FindRoom(roomName);

            if(room is not null)
            {
                Rooms.Remove(room);

                await SaveChangesAsync();

                return new CommandResult(StatusCode.Success)
                .AddMessage(Message.Deleted("room", room.Name));
            }
            else
            {
                return RoomDoesNotExist(roomName);
            }
        }

        public async Task<CommandResult> DeleteRoomPool(string poolName)
        {
            var pool = await FindRoomPool(poolName);

            if(pool is not null)
            {
                // Find all rooms referencing this room pool
                var roomsWithReference = await Rooms.Where(
                    room => room.Curiosity == pool
                ).ToListAsync();
                // Remove all references to this room pool
                foreach(var room in roomsWithReference)
                {
                    room.Curiosity = null;
                }

                RoomPools.Remove(pool);

                await SaveChangesAsync();

                return new CommandResult(StatusCode.Success)
                .AddMessage(Message.Deleted("room pool", pool.Name));
            }
            else
            {
                return RoomPoolDoesNotExist(poolName);
            }
        }

        public async Task<ICollection<Feature>> FindAllFeatures()
        {
            return await Features.ToListAsync();
        }

        public async Task<ICollection<Room>> FindAllGlobalRooms()
        {
            return await Rooms.Where(
                room => room.GlobalAccess == true
            ).ToListAsync();
        }

        public async Task<ICollection<Item>> FindAllItems()
        {
            return await Items.ToListAsync();
        }

        public async Task<ICollection<Offer>> FindAllOffersInRoom(Room room)
        {
            return await Offers.Where(
                offer => offer.CreatedByBeing.InRoom == room
            ).ToListAsync();
        }

        public async Task<ICollection<RoomPool>> FindAllRoomPools()
        {
            return await RoomPools.ToListAsync();
        }

        public async Task<bool> FeatureNameIsReserved(string featureName)
        {
            return await Features.AnyAsync(
                feature => feature.Name == featureName
            );
        }

        public async Task<Being?> FindBeing(string beingName)
        {
            return await Beings.SingleOrDefaultAsync(
                being => being.Name == beingName
            );
        }

        public async Task<Feature?> FindFeature(string featureName)
        {
            return await Features.SingleOrDefaultAsync(
                feature => feature.Name == featureName
            );
        }

        public async Task<Item?> FindItem(string itemName)
        {
            return await Items.SingleOrDefaultAsync(
                item => item.Name == itemName
            );
        }

        public async Task<List<Offer>> FindMatchingOffers(Offer newOffer)
        {
            var offers = await Offers.Where(offerInDb =>
                (offerInDb.ItemToBuy == newOffer.ItemToSell)
                && (offerInDb.ItemToSell == newOffer.ItemToBuy)
                && (offerInDb.QuantityToBuy <= newOffer.QuantityToSell)
                && (offerInDb.QuantityToSell == newOffer.QuantityToBuy)
                && (offerInDb.CreatedByBeing.TradeInventory != newOffer.CreatedByBeing.TradeInventory)
                && (offerInDb.CreatedByBeing.InRoom == newOffer.CreatedByBeing.InRoom)
            ).ToListAsync();

            return offers;
        }
        
        public async Task<Room?> FindRoom(string roomName)
        {
            return await Rooms.SingleOrDefaultAsync(
                room => room.Name == roomName
            );
        }

        public async Task<RoomPool?> FindRoomPool(string poolName)
        {
            return await RoomPools.SingleOrDefaultAsync(
                pool => pool.Name == poolName
            );
        }

        public async Task<User?> FindUser(string username)
        {
            return await Users.SingleOrDefaultAsync(
                user => user.Username == username
            );
        }

        public async Task<GameSettings> GetGameSettings()
        {
            var settings = await GameSettings.FirstOrDefaultAsync();
            return settings!;
        }

        public async Task<ulong> GetTickCount()
        {
            var tickCounter = await TickCounter.FirstOrDefaultAsync();

            if(tickCounter is not null)
            {
                return tickCounter.TickCount;
            }
            else
            {
                return 0;
            }
        }

        public async Task<string> GetUniqueBeingName(string beingName)
        {
            while(await BeingNameIsReserved(beingName))
            {
                beingName += StringUtilities.GetRandomCharacter();
            }

            return beingName;
        }
        
        public async Task<string> GetUniqueRoomName(string roomName)
        {
            while(await RoomNameIsReserved(roomName))
            {
                roomName += StringUtilities.GetRandomCharacter();
            }

            return roomName;
        }

        public async Task<bool> ItemNameIsReserved(string itemName)
        {
            return await Items.AnyAsync(
                item => item.Name == itemName
            );
        }
        
        public async Task<bool> RoomNameIsReserved(string roomName)
        {
            return await Rooms.AnyAsync(
                room => room.Name == roomName
            );
        }

        public async Task<bool> RoomPoolNameIsReserved(string poolName)
        {
            return await RoomPools.AnyAsync(
                pool => pool.Name == poolName
            );
        }

        public async Task SetGameSettings(GameSettings newSettings)
        {
            var settingsInDb = await GetGameSettings();

            if(settingsInDb is null)
            {
                await this.GameSettings.AddAsync(newSettings);
            }
            else
            {
                settingsInDb = newSettings;
            }

            await SaveChangesAsync();
        }

        public async Task<bool> UsernameIsReserved(string username)
        {
            return await Users.AnyAsync(
                user => user.Username == username
            );
        }
    }
}