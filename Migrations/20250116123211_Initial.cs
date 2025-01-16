using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MUS.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Features",
                columns: table => new
                {
                    PrimaryKey = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Features", x => x.PrimaryKey);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Inventories",
                columns: table => new
                {
                    PrimaryKey = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Inventories", x => x.PrimaryKey);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "TickCounter",
                columns: table => new
                {
                    PrimaryKey = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    TickCount = table.Column<ulong>(type: "bigint unsigned", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TickCounter", x => x.PrimaryKey);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "BeingFeature",
                columns: table => new
                {
                    AssociatedWithBeingsPrimaryKey = table.Column<int>(type: "int", nullable: false),
                    FeaturesPrimaryKey = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BeingFeature", x => new { x.AssociatedWithBeingsPrimaryKey, x.FeaturesPrimaryKey });
                    table.ForeignKey(
                        name: "FK_BeingFeature_Features_FeaturesPrimaryKey",
                        column: x => x.FeaturesPrimaryKey,
                        principalTable: "Features",
                        principalColumn: "PrimaryKey",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Beings",
                columns: table => new
                {
                    PrimaryKey = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    CreatedByUserPrimaryKey = table.Column<int>(type: "int", nullable: false),
                    FreeInventoryPrimaryKey = table.Column<int>(type: "int", nullable: false),
                    InRoomPrimaryKey = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    RoomInsidePrimaryKey = table.Column<int>(type: "int", nullable: true),
                    TradeInventoryPrimaryKey = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Beings", x => x.PrimaryKey);
                    table.ForeignKey(
                        name: "FK_Beings_Inventories_FreeInventoryPrimaryKey",
                        column: x => x.FreeInventoryPrimaryKey,
                        principalTable: "Inventories",
                        principalColumn: "PrimaryKey",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Beings_Inventories_TradeInventoryPrimaryKey",
                        column: x => x.TradeInventoryPrimaryKey,
                        principalTable: "Inventories",
                        principalColumn: "PrimaryKey",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Items",
                columns: table => new
                {
                    PrimaryKey = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DeploymentPrototypePrimaryKey = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Items", x => x.PrimaryKey);
                    table.ForeignKey(
                        name: "FK_Items_Beings_DeploymentPrototypePrimaryKey",
                        column: x => x.DeploymentPrototypePrimaryKey,
                        principalTable: "Beings",
                        principalColumn: "PrimaryKey");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    PrimaryKey = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    HashedPassword = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsAdmin = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    SelectedBeingPrimaryKey = table.Column<int>(type: "int", nullable: true),
                    Username = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.PrimaryKey);
                    table.ForeignKey(
                        name: "FK_Users_Beings_SelectedBeingPrimaryKey",
                        column: x => x.SelectedBeingPrimaryKey,
                        principalTable: "Beings",
                        principalColumn: "PrimaryKey");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "CraftPlans",
                columns: table => new
                {
                    PrimaryKey = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ProductPrimaryKey = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CraftPlans", x => x.PrimaryKey);
                    table.ForeignKey(
                        name: "FK_CraftPlans_Items_ProductPrimaryKey",
                        column: x => x.ProductPrimaryKey,
                        principalTable: "Items",
                        principalColumn: "PrimaryKey",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ItemHatchers",
                columns: table => new
                {
                    PrimaryKey = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ItemPrimaryKey = table.Column<int>(type: "int", nullable: false),
                    MaximumQuantity = table.Column<int>(type: "int", nullable: false),
                    MinimumQuantity = table.Column<int>(type: "int", nullable: false),
                    IntervalInTicks = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemHatchers", x => x.PrimaryKey);
                    table.ForeignKey(
                        name: "FK_ItemHatchers_Items_ItemPrimaryKey",
                        column: x => x.ItemPrimaryKey,
                        principalTable: "Items",
                        principalColumn: "PrimaryKey",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ItemsStacks",
                columns: table => new
                {
                    ItemPrimaryKey = table.Column<int>(type: "int", nullable: false),
                    InventoryPrimaryKey = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemsStacks", x => new { x.ItemPrimaryKey, x.InventoryPrimaryKey });
                    table.ForeignKey(
                        name: "FK_ItemsStacks_Inventories_InventoryPrimaryKey",
                        column: x => x.InventoryPrimaryKey,
                        principalTable: "Inventories",
                        principalColumn: "PrimaryKey",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ItemsStacks_Items_ItemPrimaryKey",
                        column: x => x.ItemPrimaryKey,
                        principalTable: "Items",
                        principalColumn: "PrimaryKey",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Offers",
                columns: table => new
                {
                    PrimaryKey = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    CreatedByBeingPrimaryKey = table.Column<int>(type: "int", nullable: false),
                    ItemToSellPrimaryKey = table.Column<int>(type: "int", nullable: false),
                    QuantityToSell = table.Column<int>(type: "int", nullable: false),
                    ItemToBuyPrimaryKey = table.Column<int>(type: "int", nullable: false),
                    QuantityToBuy = table.Column<int>(type: "int", nullable: false),
                    Repeat = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Offers", x => x.PrimaryKey);
                    table.ForeignKey(
                        name: "FK_Offers_Beings_CreatedByBeingPrimaryKey",
                        column: x => x.CreatedByBeingPrimaryKey,
                        principalTable: "Beings",
                        principalColumn: "PrimaryKey",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Offers_Items_ItemToBuyPrimaryKey",
                        column: x => x.ItemToBuyPrimaryKey,
                        principalTable: "Items",
                        principalColumn: "PrimaryKey",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Offers_Items_ItemToSellPrimaryKey",
                        column: x => x.ItemToSellPrimaryKey,
                        principalTable: "Items",
                        principalColumn: "PrimaryKey",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "RoomPools",
                columns: table => new
                {
                    PrimaryKey = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Description = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FeeItemPrimaryKey = table.Column<int>(type: "int", nullable: true),
                    Name = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoomPools", x => x.PrimaryKey);
                    table.ForeignKey(
                        name: "FK_RoomPools_Items_FeeItemPrimaryKey",
                        column: x => x.FeeItemPrimaryKey,
                        principalTable: "Items",
                        principalColumn: "PrimaryKey");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "CraftComponent",
                columns: table => new
                {
                    PrimaryKey = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    ItemPrimaryKey = table.Column<int>(type: "int", nullable: false),
                    CraftPlanPrimaryKey = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CraftComponent", x => x.PrimaryKey);
                    table.ForeignKey(
                        name: "FK_CraftComponent_CraftPlans_CraftPlanPrimaryKey",
                        column: x => x.CraftPlanPrimaryKey,
                        principalTable: "CraftPlans",
                        principalColumn: "PrimaryKey");
                    table.ForeignKey(
                        name: "FK_CraftComponent_Items_ItemPrimaryKey",
                        column: x => x.ItemPrimaryKey,
                        principalTable: "Items",
                        principalColumn: "PrimaryKey",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "InventoryItemHatcher",
                columns: table => new
                {
                    InventoriesPrimaryKey = table.Column<int>(type: "int", nullable: false),
                    ItemHatchersPrimaryKey = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryItemHatcher", x => new { x.InventoriesPrimaryKey, x.ItemHatchersPrimaryKey });
                    table.ForeignKey(
                        name: "FK_InventoryItemHatcher_Inventories_InventoriesPrimaryKey",
                        column: x => x.InventoriesPrimaryKey,
                        principalTable: "Inventories",
                        principalColumn: "PrimaryKey",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InventoryItemHatcher_ItemHatchers_ItemHatchersPrimaryKey",
                        column: x => x.ItemHatchersPrimaryKey,
                        principalTable: "ItemHatchers",
                        principalColumn: "PrimaryKey",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Rooms",
                columns: table => new
                {
                    PrimaryKey = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ConnectionLimit = table.Column<int>(type: "int", nullable: false),
                    CuriosityPrimaryKey = table.Column<int>(type: "int", nullable: true),
                    Description = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    GlobalAccess = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Name = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    InventoryPrimaryKey = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rooms", x => x.PrimaryKey);
                    table.ForeignKey(
                        name: "FK_Rooms_Inventories_InventoryPrimaryKey",
                        column: x => x.InventoryPrimaryKey,
                        principalTable: "Inventories",
                        principalColumn: "PrimaryKey",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Rooms_RoomPools_CuriosityPrimaryKey",
                        column: x => x.CuriosityPrimaryKey,
                        principalTable: "RoomPools",
                        principalColumn: "PrimaryKey");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "FeatureRoom",
                columns: table => new
                {
                    RequiredFeaturesPrimaryKey = table.Column<int>(type: "int", nullable: false),
                    RequiredInRoomsPrimaryKey = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FeatureRoom", x => new { x.RequiredFeaturesPrimaryKey, x.RequiredInRoomsPrimaryKey });
                    table.ForeignKey(
                        name: "FK_FeatureRoom_Features_RequiredFeaturesPrimaryKey",
                        column: x => x.RequiredFeaturesPrimaryKey,
                        principalTable: "Features",
                        principalColumn: "PrimaryKey",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FeatureRoom_Rooms_RequiredInRoomsPrimaryKey",
                        column: x => x.RequiredInRoomsPrimaryKey,
                        principalTable: "Rooms",
                        principalColumn: "PrimaryKey",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "GameSettings",
                columns: table => new
                {
                    PrimaryKey = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    DefaultBeingPrimaryKey = table.Column<int>(type: "int", nullable: false),
                    DefaultSpawnRoomPrimaryKey = table.Column<int>(type: "int", nullable: false),
                    GameName = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    GameDescription = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TickIntervalSeconds = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameSettings", x => x.PrimaryKey);
                    table.ForeignKey(
                        name: "FK_GameSettings_Beings_DefaultBeingPrimaryKey",
                        column: x => x.DefaultBeingPrimaryKey,
                        principalTable: "Beings",
                        principalColumn: "PrimaryKey",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GameSettings_Rooms_DefaultSpawnRoomPrimaryKey",
                        column: x => x.DefaultSpawnRoomPrimaryKey,
                        principalTable: "Rooms",
                        principalColumn: "PrimaryKey",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "RoomRoom",
                columns: table => new
                {
                    ConnectedFromRoomsPrimaryKey = table.Column<int>(type: "int", nullable: false),
                    ConnectedToRoomsPrimaryKey = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoomRoom", x => new { x.ConnectedFromRoomsPrimaryKey, x.ConnectedToRoomsPrimaryKey });
                    table.ForeignKey(
                        name: "FK_RoomRoom_Rooms_ConnectedFromRoomsPrimaryKey",
                        column: x => x.ConnectedFromRoomsPrimaryKey,
                        principalTable: "Rooms",
                        principalColumn: "PrimaryKey",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RoomRoom_Rooms_ConnectedToRoomsPrimaryKey",
                        column: x => x.ConnectedToRoomsPrimaryKey,
                        principalTable: "Rooms",
                        principalColumn: "PrimaryKey",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "RoomRoomPool",
                columns: table => new
                {
                    PrototypesPrimaryKey = table.Column<int>(type: "int", nullable: false),
                    RoomPoolsPrimaryKey = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoomRoomPool", x => new { x.PrototypesPrimaryKey, x.RoomPoolsPrimaryKey });
                    table.ForeignKey(
                        name: "FK_RoomRoomPool_RoomPools_RoomPoolsPrimaryKey",
                        column: x => x.RoomPoolsPrimaryKey,
                        principalTable: "RoomPools",
                        principalColumn: "PrimaryKey",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RoomRoomPool_Rooms_PrototypesPrimaryKey",
                        column: x => x.PrototypesPrimaryKey,
                        principalTable: "Rooms",
                        principalColumn: "PrimaryKey",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_BeingFeature_FeaturesPrimaryKey",
                table: "BeingFeature",
                column: "FeaturesPrimaryKey");

            migrationBuilder.CreateIndex(
                name: "IX_Beings_CreatedByUserPrimaryKey",
                table: "Beings",
                column: "CreatedByUserPrimaryKey");

            migrationBuilder.CreateIndex(
                name: "IX_Beings_FreeInventoryPrimaryKey",
                table: "Beings",
                column: "FreeInventoryPrimaryKey");

            migrationBuilder.CreateIndex(
                name: "IX_Beings_InRoomPrimaryKey",
                table: "Beings",
                column: "InRoomPrimaryKey");

            migrationBuilder.CreateIndex(
                name: "IX_Beings_Name",
                table: "Beings",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Beings_RoomInsidePrimaryKey",
                table: "Beings",
                column: "RoomInsidePrimaryKey",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Beings_TradeInventoryPrimaryKey",
                table: "Beings",
                column: "TradeInventoryPrimaryKey");

            migrationBuilder.CreateIndex(
                name: "IX_CraftComponent_CraftPlanPrimaryKey",
                table: "CraftComponent",
                column: "CraftPlanPrimaryKey");

            migrationBuilder.CreateIndex(
                name: "IX_CraftComponent_ItemPrimaryKey",
                table: "CraftComponent",
                column: "ItemPrimaryKey");

            migrationBuilder.CreateIndex(
                name: "IX_CraftPlans_ProductPrimaryKey",
                table: "CraftPlans",
                column: "ProductPrimaryKey",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FeatureRoom_RequiredInRoomsPrimaryKey",
                table: "FeatureRoom",
                column: "RequiredInRoomsPrimaryKey");

            migrationBuilder.CreateIndex(
                name: "IX_Features_Name",
                table: "Features",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_GameSettings_DefaultBeingPrimaryKey",
                table: "GameSettings",
                column: "DefaultBeingPrimaryKey");

            migrationBuilder.CreateIndex(
                name: "IX_GameSettings_DefaultSpawnRoomPrimaryKey",
                table: "GameSettings",
                column: "DefaultSpawnRoomPrimaryKey");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryItemHatcher_ItemHatchersPrimaryKey",
                table: "InventoryItemHatcher",
                column: "ItemHatchersPrimaryKey");

            migrationBuilder.CreateIndex(
                name: "IX_ItemHatchers_ItemPrimaryKey",
                table: "ItemHatchers",
                column: "ItemPrimaryKey");

            migrationBuilder.CreateIndex(
                name: "IX_Items_DeploymentPrototypePrimaryKey",
                table: "Items",
                column: "DeploymentPrototypePrimaryKey");

            migrationBuilder.CreateIndex(
                name: "IX_Items_Name",
                table: "Items",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_ItemsStacks_InventoryPrimaryKey",
                table: "ItemsStacks",
                column: "InventoryPrimaryKey");

            migrationBuilder.CreateIndex(
                name: "IX_Offers_CreatedByBeingPrimaryKey",
                table: "Offers",
                column: "CreatedByBeingPrimaryKey");

            migrationBuilder.CreateIndex(
                name: "IX_Offers_ItemToBuyPrimaryKey",
                table: "Offers",
                column: "ItemToBuyPrimaryKey");

            migrationBuilder.CreateIndex(
                name: "IX_Offers_ItemToSellPrimaryKey",
                table: "Offers",
                column: "ItemToSellPrimaryKey");

            migrationBuilder.CreateIndex(
                name: "IX_RoomPools_FeeItemPrimaryKey",
                table: "RoomPools",
                column: "FeeItemPrimaryKey");

            migrationBuilder.CreateIndex(
                name: "IX_RoomPools_Name",
                table: "RoomPools",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_RoomRoom_ConnectedToRoomsPrimaryKey",
                table: "RoomRoom",
                column: "ConnectedToRoomsPrimaryKey");

            migrationBuilder.CreateIndex(
                name: "IX_RoomRoomPool_RoomPoolsPrimaryKey",
                table: "RoomRoomPool",
                column: "RoomPoolsPrimaryKey");

            migrationBuilder.CreateIndex(
                name: "IX_Rooms_CuriosityPrimaryKey",
                table: "Rooms",
                column: "CuriosityPrimaryKey");

            migrationBuilder.CreateIndex(
                name: "IX_Rooms_InventoryPrimaryKey",
                table: "Rooms",
                column: "InventoryPrimaryKey");

            migrationBuilder.CreateIndex(
                name: "IX_Rooms_Name",
                table: "Rooms",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Users_SelectedBeingPrimaryKey",
                table: "Users",
                column: "SelectedBeingPrimaryKey");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username",
                table: "Users",
                column: "Username",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_BeingFeature_Beings_AssociatedWithBeingsPrimaryKey",
                table: "BeingFeature",
                column: "AssociatedWithBeingsPrimaryKey",
                principalTable: "Beings",
                principalColumn: "PrimaryKey",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Beings_Rooms_InRoomPrimaryKey",
                table: "Beings",
                column: "InRoomPrimaryKey",
                principalTable: "Rooms",
                principalColumn: "PrimaryKey",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Beings_Rooms_RoomInsidePrimaryKey",
                table: "Beings",
                column: "RoomInsidePrimaryKey",
                principalTable: "Rooms",
                principalColumn: "PrimaryKey");

            migrationBuilder.AddForeignKey(
                name: "FK_Beings_Users_CreatedByUserPrimaryKey",
                table: "Beings",
                column: "CreatedByUserPrimaryKey",
                principalTable: "Users",
                principalColumn: "PrimaryKey",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Items_Beings_DeploymentPrototypePrimaryKey",
                table: "Items");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Beings_SelectedBeingPrimaryKey",
                table: "Users");

            migrationBuilder.DropTable(
                name: "BeingFeature");

            migrationBuilder.DropTable(
                name: "CraftComponent");

            migrationBuilder.DropTable(
                name: "FeatureRoom");

            migrationBuilder.DropTable(
                name: "GameSettings");

            migrationBuilder.DropTable(
                name: "InventoryItemHatcher");

            migrationBuilder.DropTable(
                name: "ItemsStacks");

            migrationBuilder.DropTable(
                name: "Offers");

            migrationBuilder.DropTable(
                name: "RoomRoom");

            migrationBuilder.DropTable(
                name: "RoomRoomPool");

            migrationBuilder.DropTable(
                name: "TickCounter");

            migrationBuilder.DropTable(
                name: "CraftPlans");

            migrationBuilder.DropTable(
                name: "Features");

            migrationBuilder.DropTable(
                name: "ItemHatchers");

            migrationBuilder.DropTable(
                name: "Beings");

            migrationBuilder.DropTable(
                name: "Rooms");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Inventories");

            migrationBuilder.DropTable(
                name: "RoomPools");

            migrationBuilder.DropTable(
                name: "Items");
        }
    }
}
