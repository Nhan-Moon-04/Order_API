using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Restaurant.Data.Migrations
{
    public partial class InitClean : Migration
    {

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Areas",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    AreaId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    AreaName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Areas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Kitchens",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    KitchenId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    KitchenName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Kitchens", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Dishes",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    DishId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    DishName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<double>(type: "float", nullable: false),
                    KitchenId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dishes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Dishes_Kitchens_KitchenId",
                        column: x => x.KitchenId,
                        principalTable: "Kitchens",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AreaDishPrices",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    AreaId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    DishId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CustomPrice = table.Column<double>(type: "float", nullable: false),
                    EffectiveDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AreaDishPrices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AreaDishPrices_Areas_AreaId",
                        column: x => x.AreaId,
                        principalTable: "Areas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AreaDishPrices_Dishes_DishId",
                        column: x => x.DishId,
                        principalTable: "Dishes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Areas",
                columns: new[] { "Id", "AreaId", "AreaName", "CreatedAt", "Description", "IsActive" },
                values: new object[,]
                {
                    { "AREA001", "A001", "Tầng Trệt - Khu A", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Khu vực tầng trệt, phù hợp cho gia đình", true },
                    { "AREA002", "A002", "Tầng Trệt - Khu B", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Khu vực tầng trệt, gần cửa sổ", true },
                    { "AREA003", "A003", "Tầng 2 - Khu VIP", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Khu vực VIP tầng 2, view đẹp", true },
                    { "AREA004", "A004", "Tầng 2 - Khu Thường", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Khu vực thường tầng 2", true },
                    { "AREA005", "A005", "Sân Thượng", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Khu vực sân thượng, không gian mở", false }
                });

            migrationBuilder.InsertData(
                table: "Kitchens",
                columns: new[] { "Id", "CreatedAt", "Description", "IsActive", "KitchenId", "KitchenName" },
                values: new object[,]
                {
                    { "KITCHEN001", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Chuyên món Âu, Mỹ, Ý", true, "K001", "Bếp Âu" },
                    { "KITCHEN002", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Chuyên món Việt, Trung, Nhật, Hàn", true, "K002", "Bếp Á" },
                    { "KITCHEN003", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Chuyên các món nướng, BBQ", true, "K003", "Bếp Nướng BBQ" },
                    { "KITCHEN004", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Chuyên tráng miệng, đồ ngọt", true, "K004", "Bếp Dessert" },
                    { "KITCHEN005", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Chuyên pha chế đồ uống, cocktail", true, "K005", "Bar & Thức Uống" }
                });

            migrationBuilder.InsertData(
                table: "Dishes",
                columns: new[] { "Id", "CreatedAt", "DishId", "DishName", "IsActive", "KitchenId", "Price" },
                values: new object[,]
                {
                    { "DISH001", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "D001", "Steak Bò Mỹ", true, "KITCHEN001", 350000.0 },
                    { "DISH002", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "D002", "Pasta Carbonara", true, "KITCHEN001", 180000.0 },
                    { "DISH003", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "D003", "Salmon Teriyaki", true, "KITCHEN001", 280000.0 },
                    { "DISH004", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "D004", "Phở Bò Đặc Biệt", true, "KITCHEN002", 75000.0 },
                    { "DISH005", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "D005", "Bún Chả Hà Nội", true, "KITCHEN002", 65000.0 },
                    { "DISH006", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "D006", "Cơm Gà Hải Nam", true, "KITCHEN002", 85000.0 },
                    { "DISH007", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "D007", "Lẩu Thái Chua Cay", true, "KITCHEN002", 250000.0 },
                    { "DISH008", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "D008", "Sườn Nướng BBQ", true, "KITCHEN003", 195000.0 },
                    { "DISH009", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "D009", "Gà Nướng Mật Ong", true, "KITCHEN003", 165000.0 },
                    { "DISH010", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "D010", "Bò Nướng Lá Lốt", true, "KITCHEN003", 145000.0 },
                    { "DISH011", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "D011", "Tiramisu", true, "KITCHEN004", 65000.0 },
                    { "DISH012", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "D012", "Chocolate Lava Cake", true, "KITCHEN004", 75000.0 },
                    { "DISH013", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "D013", "Kem Vanilla Pháp", true, "KITCHEN004", 45000.0 },
                    { "DISH014", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "D014", "Mojito Classic", true, "KITCHEN005", 85000.0 },
                    { "DISH015", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "D015", "Cà Phê Sữa Đá", true, "KITCHEN005", 25000.0 },
                    { "DISH016", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "D016", "Sinh Tố Bơ", true, "KITCHEN005", 35000.0 }
                });

            migrationBuilder.InsertData(
                table: "AreaDishPrices",
                columns: new[] { "Id", "AreaId", "CustomPrice", "DishId", "EffectiveDate", "IsActive" },
                values: new object[,]
                {
                    { "ADP001", "AREA001", 350000.0, "DISH001", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true },
                    { "ADP002", "AREA001", 75000.0, "DISH004", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true },
                    { "ADP003", "AREA003", 402500.0, "DISH001", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true },
                    { "ADP004", "AREA003", 207000.0, "DISH002", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true },
                    { "ADP005", "AREA003", 322000.0, "DISH003", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true },
                    { "ADP006", "AREA003", 287500.0, "DISH007", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true },
                    { "ADP007", "AREA002", 185250.0, "DISH008", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true },
                    { "ADP008", "AREA002", 156750.0, "DISH009", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true },
                    { "ADP009", "AREA002", 137750.0, "DISH010", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true },
                    { "ADP010", "AREA004", 75000.0, "DISH004", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true },
                    { "ADP011", "AREA004", 65000.0, "DISH005", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true },
                    { "ADP012", "AREA004", 85000.0, "DISH006", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true },
                    { "ADP013", "AREA001", 65000.0, "DISH011", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true },
                    { "ADP014", "AREA001", 85000.0, "DISH014", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true },
                    { "ADP015", "AREA001", 25000.0, "DISH015", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true },
                    { "ADP016", "AREA003", 71500.0, "DISH011", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true },
                    { "ADP017", "AREA003", 82500.0, "DISH012", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true },
                    { "ADP018", "AREA003", 93500.0, "DISH014", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AreaDishPrices_AreaId_DishId_EffectiveDate",
                table: "AreaDishPrices",
                columns: new[] { "AreaId", "DishId", "EffectiveDate" });

            migrationBuilder.CreateIndex(
                name: "IX_AreaDishPrices_DishId",
                table: "AreaDishPrices",
                column: "DishId");

            migrationBuilder.CreateIndex(
                name: "IX_Areas_AreaId",
                table: "Areas",
                column: "AreaId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Dishes_DishId",
                table: "Dishes",
                column: "DishId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Dishes_KitchenId",
                table: "Dishes",
                column: "KitchenId");

            migrationBuilder.CreateIndex(
                name: "IX_Kitchens_KitchenId",
                table: "Kitchens",
                column: "KitchenId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AreaDishPrices");

            migrationBuilder.DropTable(
                name: "Areas");

            migrationBuilder.DropTable(
                name: "Dishes");

            migrationBuilder.DropTable(
                name: "Kitchens");
        }
    }
}
