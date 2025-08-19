using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Restaurant.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitClean : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Areas",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    AreaId = table.Column<string>(type: "nvarchar(max)", nullable: false),
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
                    KitchenId = table.Column<string>(type: "nvarchar(max)", nullable: false),
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
                    DishId = table.Column<string>(type: "nvarchar(max)", nullable: false),
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
                    { "87cce1fb-d6f9-49a2-b026-15bb0740385c", "A002", "Tầng Trệt - Khu B", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Khu vực tầng trệt, gần cửa sổ", true },
                    { "a57ca4df-4212-4271-ab4e-3c2761987e0b", "A003", "Tầng 2 - Khu VIP", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Khu vực VIP tầng 2, view đẹp", true },
                    { "bf1d3a14-cec7-4217-afef-3070cc964d2b", "A001", "Tầng Trệt - Khu A", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Khu vực tầng trệt, phù hợp cho gia đình", true },
                    { "e3da7474-bab3-48de-b4bd-7887170d7e59", "A004", "Tầng 2 - Khu Thường", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Khu vực thường tầng 2", true },
                    { "eb837d6c-079e-44b9-8df5-da3ac31bc03c", "A005", "Sân Thượng", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Khu vực sân thượng, không gian mở", false }
                });

            migrationBuilder.InsertData(
                table: "Kitchens",
                columns: new[] { "Id", "CreatedAt", "Description", "IsActive", "KitchenId", "KitchenName" },
                values: new object[,]
                {
                    { "2a8d7cbf-f8c0-426f-bebc-da0d67428aad", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Chuyên món Âu, Mỹ, Ý", true, "K001", "Bếp Âu" },
                    { "8820e289-12fc-4434-820e-440479c9b462", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Chuyên pha chế đồ uống, cocktail", true, "K005", "Bar & Thức Uống" },
                    { "dcabb860-de38-4cb8-a190-8ef255deba01", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Chuyên các món nướng, BBQ", true, "K003", "Bếp Nướng BBQ" },
                    { "ed39caf8-325b-4670-a339-9a0b17983250", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Chuyên món Việt, Trung, Nhật, Hàn", true, "K002", "Bếp Á" },
                    { "f6729fb5-bf42-4a1c-9e5e-845f7964f71a", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Chuyên tráng miệng, đồ ngọt", true, "K004", "Bếp Dessert" }
                });

            migrationBuilder.InsertData(
                table: "Dishes",
                columns: new[] { "Id", "CreatedAt", "DishId", "DishName", "IsActive", "KitchenId", "Price" },
                values: new object[,]
                {
                    { "021d17c2-fc6a-459a-af8d-e38eeac64faa", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "D012", "Chocolate Lava Cake", true, "f6729fb5-bf42-4a1c-9e5e-845f7964f71a", 75000.0 },
                    { "1a3c182a-9605-44e1-ae27-df362a15494c", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "D010", "Bò Nướng Lá Lốt", true, "dcabb860-de38-4cb8-a190-8ef255deba01", 145000.0 },
                    { "1f86ddb0-3b7e-441f-9628-3bc7b2a7de9e", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "D016", "Sinh Tố Bơ", true, "8820e289-12fc-4434-820e-440479c9b462", 35000.0 },
                    { "3d1559a8-bb69-4c03-b341-6df3035d5341", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "D014", "Mojito Classic", true, "8820e289-12fc-4434-820e-440479c9b462", 85000.0 },
                    { "5667d134-2d25-4a16-a5d1-157308227a17", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "D002", "Pasta Carbonara", true, "2a8d7cbf-f8c0-426f-bebc-da0d67428aad", 180000.0 },
                    { "5c19bd27-58d1-4b05-9a68-cbf20ababb2c", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "D006", "Cơm Gà Hải Nam", true, "ed39caf8-325b-4670-a339-9a0b17983250", 85000.0 },
                    { "6d2a44ca-6e63-472f-9d80-9cecd5ba8d04", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "D008", "Sườn Nướng BBQ", true, "dcabb860-de38-4cb8-a190-8ef255deba01", 195000.0 },
                    { "7a1e24dc-ca9e-4bf5-aa2d-c0036c4043fa", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "D001", "Steak Bò Mỹ", true, "2a8d7cbf-f8c0-426f-bebc-da0d67428aad", 350000.0 },
                    { "bfee412d-3269-45b4-b044-d57d1da9d9f0", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "D009", "Gà Nướng Mật Ong", true, "dcabb860-de38-4cb8-a190-8ef255deba01", 165000.0 },
                    { "c6dc0e86-f5d0-45fc-b4de-d7299c581a33", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "D011", "Tiramisu", true, "f6729fb5-bf42-4a1c-9e5e-845f7964f71a", 65000.0 },
                    { "cff053a4-a929-40c7-ad66-dd4c3b8a6d65", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "D005", "Bún Chả Hà Nội", true, "ed39caf8-325b-4670-a339-9a0b17983250", 65000.0 },
                    { "d9ccfb52-47f7-47be-b8fe-f5eacac402b1", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "D004", "Phở Bò Đặc Biệt", true, "ed39caf8-325b-4670-a339-9a0b17983250", 75000.0 },
                    { "de57ba84-10b9-4ec3-84a7-7296917dd829", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "D015", "Cà Phê Sữa Đá", true, "8820e289-12fc-4434-820e-440479c9b462", 25000.0 },
                    { "eba3c6e2-a1c1-43ab-9d2a-33c13b335e65", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "D003", "Salmon Teriyaki", true, "2a8d7cbf-f8c0-426f-bebc-da0d67428aad", 280000.0 },
                    { "f40ac9ad-43af-4a54-86fe-f47ff0999b34", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "D007", "Lẩu Thái Chua Cay", true, "ed39caf8-325b-4670-a339-9a0b17983250", 250000.0 },
                    { "f985a4f6-43d3-4545-a967-7aba4556a108", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "D013", "Kem Vanilla Pháp", true, "f6729fb5-bf42-4a1c-9e5e-845f7964f71a", 45000.0 }
                });

            migrationBuilder.InsertData(
                table: "AreaDishPrices",
                columns: new[] { "Id", "AreaId", "CustomPrice", "DishId", "EffectiveDate", "IsActive" },
                values: new object[,]
                {
                    { "31b9a1c7-2e62-4532-b11f-197644b732b7", "a57ca4df-4212-4271-ab4e-3c2761987e0b", 82500.0, "021d17c2-fc6a-459a-af8d-e38eeac64faa", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true },
                    { "36745970-4738-442d-81ac-7c757068102b", "a57ca4df-4212-4271-ab4e-3c2761987e0b", 93500.0, "3d1559a8-bb69-4c03-b341-6df3035d5341", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true },
                    { "4326b2e6-74f3-4fb7-98ce-d93c603bb3ae", "a57ca4df-4212-4271-ab4e-3c2761987e0b", 207000.0, "5667d134-2d25-4a16-a5d1-157308227a17", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true },
                    { "45eafca6-f793-48de-8f0d-a19154392783", "bf1d3a14-cec7-4217-afef-3070cc964d2b", 85000.0, "3d1559a8-bb69-4c03-b341-6df3035d5341", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true },
                    { "4780ada7-3329-4d75-9865-b01ad347348d", "e3da7474-bab3-48de-b4bd-7887170d7e59", 75000.0, "d9ccfb52-47f7-47be-b8fe-f5eacac402b1", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true },
                    { "5ebea2d1-85b2-444f-ad62-1333c379d38d", "87cce1fb-d6f9-49a2-b026-15bb0740385c", 156750.0, "bfee412d-3269-45b4-b044-d57d1da9d9f0", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true },
                    { "5f7ad5cd-1aef-4152-a8d8-223aec2c5f52", "e3da7474-bab3-48de-b4bd-7887170d7e59", 85000.0, "5c19bd27-58d1-4b05-9a68-cbf20ababb2c", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true },
                    { "61945c0d-deff-4057-9e8a-f721a5c0cb7b", "bf1d3a14-cec7-4217-afef-3070cc964d2b", 350000.0, "7a1e24dc-ca9e-4bf5-aa2d-c0036c4043fa", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true },
                    { "6ba71674-fa01-4ee0-abe8-40a9b707a47b", "bf1d3a14-cec7-4217-afef-3070cc964d2b", 25000.0, "de57ba84-10b9-4ec3-84a7-7296917dd829", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true },
                    { "774bac0c-974a-402e-bf86-55112d3f7b32", "bf1d3a14-cec7-4217-afef-3070cc964d2b", 75000.0, "d9ccfb52-47f7-47be-b8fe-f5eacac402b1", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true },
                    { "88f46e3a-5ee1-4afc-b006-dda8a8ba613f", "a57ca4df-4212-4271-ab4e-3c2761987e0b", 322000.0, "eba3c6e2-a1c1-43ab-9d2a-33c13b335e65", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true },
                    { "91dcdba4-88f7-4c1a-b030-fda2240684fd", "e3da7474-bab3-48de-b4bd-7887170d7e59", 65000.0, "cff053a4-a929-40c7-ad66-dd4c3b8a6d65", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true },
                    { "933b82d7-80b3-412a-9095-641178588217", "87cce1fb-d6f9-49a2-b026-15bb0740385c", 185250.0, "6d2a44ca-6e63-472f-9d80-9cecd5ba8d04", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true },
                    { "aa4d00b3-42ab-4af8-9d04-c75a0ee08593", "bf1d3a14-cec7-4217-afef-3070cc964d2b", 65000.0, "c6dc0e86-f5d0-45fc-b4de-d7299c581a33", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true },
                    { "ab2daf7d-8b87-4c85-83be-caed26fdb0cd", "87cce1fb-d6f9-49a2-b026-15bb0740385c", 137750.0, "1a3c182a-9605-44e1-ae27-df362a15494c", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true },
                    { "b9e21b5c-a1b9-40a2-a37c-2d54645772fd", "a57ca4df-4212-4271-ab4e-3c2761987e0b", 402500.0, "7a1e24dc-ca9e-4bf5-aa2d-c0036c4043fa", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true },
                    { "ca3797b3-d380-42a1-a25e-8a265d9e3988", "a57ca4df-4212-4271-ab4e-3c2761987e0b", 71500.0, "c6dc0e86-f5d0-45fc-b4de-d7299c581a33", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true },
                    { "e589b937-ce64-4a17-b37c-c3824ed536fa", "a57ca4df-4212-4271-ab4e-3c2761987e0b", 287500.0, "f40ac9ad-43af-4a54-86fe-f47ff0999b34", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AreaDishPrices_AreaId",
                table: "AreaDishPrices",
                column: "AreaId");

            migrationBuilder.CreateIndex(
                name: "IX_AreaDishPrices_DishId",
                table: "AreaDishPrices",
                column: "DishId");

            migrationBuilder.CreateIndex(
                name: "IX_Dishes_KitchenId",
                table: "Dishes",
                column: "KitchenId");
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
