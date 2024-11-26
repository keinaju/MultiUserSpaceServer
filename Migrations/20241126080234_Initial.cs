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
                name: "Items",
                columns: table => new
                {
                    PrimaryKey = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Items", x => x.PrimaryKey);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "RoomPools",
                columns: table => new
                {
                    PrimaryKey = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoomPools", x => x.PrimaryKey);
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
                name: "ItemGenerators",
                columns: table => new
                {
                    PrimaryKey = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ItemPrimaryKey = table.Column<int>(type: "int", nullable: false),
                    MinQuantity = table.Column<int>(type: "int", nullable: false),
                    MaxQuantity = table.Column<int>(type: "int", nullable: false),
                    IntervalInTicks = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemGenerators", x => x.PrimaryKey);
                    table.ForeignKey(
                        name: "FK_ItemGenerators_Items_ItemPrimaryKey",
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
                name: "Obscurities",
                columns: table => new
                {
                    PrimaryKey = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Description = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    RoomPoolPrimaryKey = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Obscurities", x => x.PrimaryKey);
                    table.ForeignKey(
                        name: "FK_Obscurities_RoomPools_RoomPoolPrimaryKey",
                        column: x => x.RoomPoolPrimaryKey,
                        principalTable: "RoomPools",
                        principalColumn: "PrimaryKey",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "InventoryItemGenerator",
                columns: table => new
                {
                    InventoriesPrimaryKey = table.Column<int>(type: "int", nullable: false),
                    ItemGeneratorsPrimaryKey = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryItemGenerator", x => new { x.InventoriesPrimaryKey, x.ItemGeneratorsPrimaryKey });
                    table.ForeignKey(
                        name: "FK_InventoryItemGenerator_Inventories_InventoriesPrimaryKey",
                        column: x => x.InventoriesPrimaryKey,
                        principalTable: "Inventories",
                        principalColumn: "PrimaryKey",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InventoryItemGenerator_ItemGenerators_ItemGeneratorsPrimaryK~",
                        column: x => x.ItemGeneratorsPrimaryKey,
                        principalTable: "ItemGenerators",
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
                    Name = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    InventoryPrimaryKey = table.Column<int>(type: "int", nullable: false),
                    ObscurityPrimaryKey = table.Column<int>(type: "int", nullable: true)
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
                        name: "FK_Rooms_Obscurities_ObscurityPrimaryKey",
                        column: x => x.ObscurityPrimaryKey,
                        principalTable: "Obscurities",
                        principalColumn: "PrimaryKey");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "GameSettings",
                columns: table => new
                {
                    PrimaryKey = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    DefaultSpawnRoomPrimaryKey = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameSettings", x => x.PrimaryKey);
                    table.ForeignKey(
                        name: "FK_GameSettings_Rooms_DefaultSpawnRoomPrimaryKey",
                        column: x => x.DefaultSpawnRoomPrimaryKey,
                        principalTable: "Rooms",
                        principalColumn: "PrimaryKey",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "RoomInPool",
                columns: table => new
                {
                    PrimaryKey = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    RoomPrimaryKey = table.Column<int>(type: "int", nullable: false),
                    RoomPoolPrimaryKey = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoomInPool", x => x.PrimaryKey);
                    table.ForeignKey(
                        name: "FK_RoomInPool_RoomPools_RoomPoolPrimaryKey",
                        column: x => x.RoomPoolPrimaryKey,
                        principalTable: "RoomPools",
                        principalColumn: "PrimaryKey");
                    table.ForeignKey(
                        name: "FK_RoomInPool_Rooms_RoomPrimaryKey",
                        column: x => x.RoomPrimaryKey,
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
                name: "Beings",
                columns: table => new
                {
                    PrimaryKey = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    CreatedByUserPrimaryKey = table.Column<int>(type: "int", nullable: false),
                    InventoryPrimaryKey = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    RoomPrimaryKey = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Beings", x => x.PrimaryKey);
                    table.ForeignKey(
                        name: "FK_Beings_Inventories_InventoryPrimaryKey",
                        column: x => x.InventoryPrimaryKey,
                        principalTable: "Inventories",
                        principalColumn: "PrimaryKey",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Beings_Rooms_RoomPrimaryKey",
                        column: x => x.RoomPrimaryKey,
                        principalTable: "Rooms",
                        principalColumn: "PrimaryKey",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    PrimaryKey = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Username = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    HashedPassword = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PickedBeingPrimaryKey = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.PrimaryKey);
                    table.ForeignKey(
                        name: "FK_Users_Beings_PickedBeingPrimaryKey",
                        column: x => x.PickedBeingPrimaryKey,
                        principalTable: "Beings",
                        principalColumn: "PrimaryKey");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Beings_CreatedByUserPrimaryKey",
                table: "Beings",
                column: "CreatedByUserPrimaryKey");

            migrationBuilder.CreateIndex(
                name: "IX_Beings_InventoryPrimaryKey",
                table: "Beings",
                column: "InventoryPrimaryKey");

            migrationBuilder.CreateIndex(
                name: "IX_Beings_RoomPrimaryKey",
                table: "Beings",
                column: "RoomPrimaryKey");

            migrationBuilder.CreateIndex(
                name: "IX_GameSettings_DefaultSpawnRoomPrimaryKey",
                table: "GameSettings",
                column: "DefaultSpawnRoomPrimaryKey");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryItemGenerator_ItemGeneratorsPrimaryKey",
                table: "InventoryItemGenerator",
                column: "ItemGeneratorsPrimaryKey");

            migrationBuilder.CreateIndex(
                name: "IX_ItemGenerators_ItemPrimaryKey",
                table: "ItemGenerators",
                column: "ItemPrimaryKey");

            migrationBuilder.CreateIndex(
                name: "IX_ItemsStacks_InventoryPrimaryKey",
                table: "ItemsStacks",
                column: "InventoryPrimaryKey");

            migrationBuilder.CreateIndex(
                name: "IX_Obscurities_RoomPoolPrimaryKey",
                table: "Obscurities",
                column: "RoomPoolPrimaryKey");

            migrationBuilder.CreateIndex(
                name: "IX_RoomInPool_RoomPoolPrimaryKey",
                table: "RoomInPool",
                column: "RoomPoolPrimaryKey");

            migrationBuilder.CreateIndex(
                name: "IX_RoomInPool_RoomPrimaryKey",
                table: "RoomInPool",
                column: "RoomPrimaryKey");

            migrationBuilder.CreateIndex(
                name: "IX_RoomRoom_ConnectedToRoomsPrimaryKey",
                table: "RoomRoom",
                column: "ConnectedToRoomsPrimaryKey");

            migrationBuilder.CreateIndex(
                name: "IX_Rooms_InventoryPrimaryKey",
                table: "Rooms",
                column: "InventoryPrimaryKey");

            migrationBuilder.CreateIndex(
                name: "IX_Rooms_ObscurityPrimaryKey",
                table: "Rooms",
                column: "ObscurityPrimaryKey");

            migrationBuilder.CreateIndex(
                name: "IX_Users_PickedBeingPrimaryKey",
                table: "Users",
                column: "PickedBeingPrimaryKey");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username",
                table: "Users",
                column: "Username",
                unique: true);

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
                name: "FK_Beings_Inventories_InventoryPrimaryKey",
                table: "Beings");

            migrationBuilder.DropForeignKey(
                name: "FK_Rooms_Inventories_InventoryPrimaryKey",
                table: "Rooms");

            migrationBuilder.DropForeignKey(
                name: "FK_Beings_Rooms_RoomPrimaryKey",
                table: "Beings");

            migrationBuilder.DropForeignKey(
                name: "FK_Beings_Users_CreatedByUserPrimaryKey",
                table: "Beings");

            migrationBuilder.DropTable(
                name: "GameSettings");

            migrationBuilder.DropTable(
                name: "InventoryItemGenerator");

            migrationBuilder.DropTable(
                name: "ItemsStacks");

            migrationBuilder.DropTable(
                name: "RoomInPool");

            migrationBuilder.DropTable(
                name: "RoomRoom");

            migrationBuilder.DropTable(
                name: "TickCounter");

            migrationBuilder.DropTable(
                name: "ItemGenerators");

            migrationBuilder.DropTable(
                name: "Items");

            migrationBuilder.DropTable(
                name: "Inventories");

            migrationBuilder.DropTable(
                name: "Rooms");

            migrationBuilder.DropTable(
                name: "Obscurities");

            migrationBuilder.DropTable(
                name: "RoomPools");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Beings");
        }
    }
}
