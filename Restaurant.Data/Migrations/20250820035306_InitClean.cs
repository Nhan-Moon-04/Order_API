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
                    AreaId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    AreaName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Areas", x => x.Id);
                    table.UniqueConstraint("AK_Areas_AreaId", x => x.AreaId);
                });

            migrationBuilder.CreateTable(
                name: "Kitchens",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    KitchenId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    KitchenName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Kitchens", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tables",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TableCode = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TableName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Capacity = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    AreaId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tables", x => x.Id);
                    table.UniqueConstraint("AK_Tables_TableCode", x => x.TableCode);
                    table.ForeignKey(
                        name: "FK_Tables_Areas_AreaId",
                        column: x => x.AreaId,
                        principalTable: "Areas",
                        principalColumn: "AreaId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Dishes",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    DishId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    DishName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BasePrice = table.Column<double>(type: "float", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    KitchenId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dishes", x => x.Id);
                    table.UniqueConstraint("AK_Dishes_DishId", x => x.DishId);
                    table.ForeignKey(
                        name: "FK_Dishes_Kitchens_KitchenId",
                        column: x => x.KitchenId,
                        principalTable: "Kitchens",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    OrderId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    OrderDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsPaid = table.Column<bool>(type: "bit", nullable: false),
                    TableCode = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                    table.UniqueConstraint("AK_Orders_OrderId", x => x.OrderId);
                    table.ForeignKey(
                        name: "FK_Orders_Tables_TableCode",
                        column: x => x.TableCode,
                        principalTable: "Tables",
                        principalColumn: "TableCode",
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

            migrationBuilder.CreateTable(
                name: "OrderDetails",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    OrderDetailId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OrderId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    DishId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    UnitPrice = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderDetails_Dishes_DishId",
                        column: x => x.DishId,
                        principalTable: "Dishes",
                        principalColumn: "DishId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderDetails_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "OrderId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Areas",
                columns: new[] { "Id", "AreaId", "AreaName", "CreatedAt", "Description", "IsActive" },
                values: new object[,]
                {
                    { "1cbade01-c4d5-4480-8495-a4088c6acdb9", "A002", "Tầng Trệt - Khu B", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Khu vực tầng trệt, gần cửa sổ", true },
                    { "658b8200-3165-4f87-9127-4f88c1e910bf", "A003", "Tầng 2 - Khu VIP", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Khu vực VIP tầng 2, view đẹp", true },
                    { "6d904071-1b94-4869-9fc4-990419fe89bf", "A005", "Sân Thượng", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Khu vực sân thượng, không gian mở", false },
                    { "8cd11c10-ad0b-4e03-ad52-45ef1df68233", "A001", "Tầng Trệt - Khu A", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Khu vực tầng trệt, phù hợp cho gia đình", true },
                    { "ff8e4878-df25-47c6-bff2-81e2ef25f6ec", "A004", "Tầng 2 - Khu Thường", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Khu vực thường tầng 2", true }
                });

            migrationBuilder.InsertData(
                table: "Kitchens",
                columns: new[] { "Id", "CreatedAt", "Description", "IsActive", "KitchenId", "KitchenName" },
                values: new object[,]
                {
                    { "24754043-632a-47ed-8249-452c4608fa6b", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Chuyên pha chế đồ uống, cocktail", true, "K005", "Bar & Thức Uống" },
                    { "894ea134-a20a-4bb6-ac52-6bd8f371e12e", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Chuyên tráng miệng, đồ ngọt", true, "K004", "Bếp Dessert" },
                    { "934a1a27-2191-463f-8706-aa8857f8f414", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Chuyên món Việt, Trung, Nhật, Hàn", true, "K002", "Bếp Á" },
                    { "cbee4395-3e11-483e-97c8-358e061874b5", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Chuyên các món nướng, BBQ", true, "K003", "Bếp Nướng BBQ" },
                    { "ffd4d425-2b9a-4da8-afc1-25b0a3ff32ca", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Chuyên món Âu, Mỹ, Ý", true, "K001", "Bếp Âu" }
                });

            migrationBuilder.InsertData(
                table: "Dishes",
                columns: new[] { "Id", "BasePrice", "CreatedAt", "DishId", "DishName", "IsActive", "KitchenId" },
                values: new object[,]
                {
                    { "0bf3e2fa-0059-40bd-aaba-2856a604a758", 280000.0, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "D003", "Salmon Teriyaki", true, "ffd4d425-2b9a-4da8-afc1-25b0a3ff32ca" },
                    { "40dace6f-dabd-444d-9378-c98967a3c183", 350000.0, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "D001", "Steak Bò Mỹ", true, "ffd4d425-2b9a-4da8-afc1-25b0a3ff32ca" },
                    { "4a60fcdf-c9fd-49d3-99de-35ccdf9cba41", 85000.0, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "D014", "Mojito Classic", true, "24754043-632a-47ed-8249-452c4608fa6b" },
                    { "4b593b0f-6dd5-41ac-83a6-739953f43e33", 35000.0, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "D016", "Sinh Tố Bơ", true, "24754043-632a-47ed-8249-452c4608fa6b" },
                    { "699b952b-1629-4f8f-9fc0-beb98db2ddbf", 75000.0, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "D004", "Phở Bò Đặc Biệt", true, "934a1a27-2191-463f-8706-aa8857f8f414" },
                    { "6e3e4f58-30fa-41a4-9aac-ada1f7f607ed", 85000.0, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "D006", "Cơm Gà Hải Nam", true, "934a1a27-2191-463f-8706-aa8857f8f414" },
                    { "741804b4-2ae4-4919-858f-4a99bd68fb96", 145000.0, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "D010", "Bò Nướng Lá Lốt", true, "cbee4395-3e11-483e-97c8-358e061874b5" },
                    { "75e98b88-6198-48aa-9ecd-af3e57d9d2a9", 75000.0, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "D012", "Chocolate Lava Cake", true, "894ea134-a20a-4bb6-ac52-6bd8f371e12e" },
                    { "81630539-2abb-4452-8fd9-37ea67259144", 25000.0, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "D015", "Cà Phê Sữa Đá", true, "24754043-632a-47ed-8249-452c4608fa6b" },
                    { "86c8cd1b-fde6-4408-a03c-ba28b417f1c6", 45000.0, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "D013", "Kem Vanilla Pháp", true, "894ea134-a20a-4bb6-ac52-6bd8f371e12e" },
                    { "8d000849-4121-44b3-9777-3057c1513170", 165000.0, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "D009", "Gà Nướng Mật Ong", true, "cbee4395-3e11-483e-97c8-358e061874b5" },
                    { "8d12a359-fc7c-45a6-b58a-e9d2bc7bff3b", 65000.0, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "D011", "Tiramisu", true, "894ea134-a20a-4bb6-ac52-6bd8f371e12e" },
                    { "ac336386-ae8d-4902-90e2-6d33e09d0a6c", 65000.0, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "D005", "Bún Chả Hà Nội", true, "934a1a27-2191-463f-8706-aa8857f8f414" },
                    { "e67c5639-c603-47f8-815b-35748a532702", 195000.0, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "D008", "Sườn Nướng BBQ", true, "cbee4395-3e11-483e-97c8-358e061874b5" },
                    { "f6700740-1e8b-4169-aace-5f04b6813cc1", 250000.0, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "D007", "Lẩu Thái Chua Cay", true, "934a1a27-2191-463f-8706-aa8857f8f414" },
                    { "fa885c4c-5a2d-4824-8757-405703cdc128", 180000.0, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "D002", "Pasta Carbonara", true, "ffd4d425-2b9a-4da8-afc1-25b0a3ff32ca" }
                });

            migrationBuilder.InsertData(
                table: "Tables",
                columns: new[] { "Id", "AreaId", "Capacity", "IsActive", "TableCode", "TableName" },
                values: new object[,]
                {
                    { "0d1d76e6-c625-4966-beb1-ffbaac5eb6bd", "A003", 6, true, "T006", "Bàn VIP 1" },
                    { "164e54f1-c092-416e-a887-892d7e7f70c9", "A002", 8, true, "T005", "Bàn B2" },
                    { "18ef20f5-7d24-4bfd-98a2-7e5e243f7d18", "A004", 6, true, "T009", "Bàn T2-2" },
                    { "6b22e489-0cee-4650-802e-85655c5deae5", "A001", 4, true, "T001", "Bàn A1" },
                    { "6e77a75d-8655-4075-9332-e9e1c0b8a2d3", "A001", 6, true, "T002", "Bàn A2" },
                    { "7edc5369-8967-40a7-909f-13f51ad9995c", "A001", 2, true, "T003", "Bàn A3" },
                    { "861e63e3-b81d-444d-b7aa-d98ee06b99bb", "A003", 10, true, "T007", "Bàn VIP 2" },
                    { "86850bb5-7e3f-48ef-9fb8-dfc00184f36e", "A004", 4, true, "T008", "Bàn T2-1" },
                    { "d545a871-cf12-4c7c-9433-bbc747aa7ce9", "A002", 4, true, "T004", "Bàn B1" }
                });

            migrationBuilder.InsertData(
                table: "AreaDishPrices",
                columns: new[] { "Id", "AreaId", "CustomPrice", "DishId", "EffectiveDate", "IsActive" },
                values: new object[,]
                {
                    { "0922a426-69f2-4a42-8241-50273afe04c3", "8cd11c10-ad0b-4e03-ad52-45ef1df68233", 350000.0, "40dace6f-dabd-444d-9378-c98967a3c183", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true },
                    { "1cf3d568-a7e0-4717-ac92-8d7768259b76", "8cd11c10-ad0b-4e03-ad52-45ef1df68233", 85000.0, "4a60fcdf-c9fd-49d3-99de-35ccdf9cba41", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true },
                    { "2469db73-9d2d-4a3a-aefd-f60533e95e01", "ff8e4878-df25-47c6-bff2-81e2ef25f6ec", 65000.0, "ac336386-ae8d-4902-90e2-6d33e09d0a6c", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true },
                    { "28b1f3df-561c-4f36-94f1-a28beab291f7", "658b8200-3165-4f87-9127-4f88c1e910bf", 402500.0, "40dace6f-dabd-444d-9378-c98967a3c183", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true },
                    { "2926d8f8-8bbe-4981-b4e8-839fc89ff022", "658b8200-3165-4f87-9127-4f88c1e910bf", 207000.0, "fa885c4c-5a2d-4824-8757-405703cdc128", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true },
                    { "441f3e6a-7ab5-4fcf-9aae-5345478cbd9e", "8cd11c10-ad0b-4e03-ad52-45ef1df68233", 25000.0, "81630539-2abb-4452-8fd9-37ea67259144", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true },
                    { "48d13ef0-0072-4918-a2aa-f36cbcededc8", "1cbade01-c4d5-4480-8495-a4088c6acdb9", 137750.0, "741804b4-2ae4-4919-858f-4a99bd68fb96", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true },
                    { "63f33e16-2c60-486e-b371-8fa6f8834e96", "658b8200-3165-4f87-9127-4f88c1e910bf", 71500.0, "8d12a359-fc7c-45a6-b58a-e9d2bc7bff3b", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true },
                    { "832ef5c0-6c81-4087-8433-9f08c5319f29", "658b8200-3165-4f87-9127-4f88c1e910bf", 322000.0, "0bf3e2fa-0059-40bd-aaba-2856a604a758", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true },
                    { "9a012fd5-3e33-4980-b03d-ac56419c6ce5", "658b8200-3165-4f87-9127-4f88c1e910bf", 93500.0, "4a60fcdf-c9fd-49d3-99de-35ccdf9cba41", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true },
                    { "a7641cfd-34e2-4cd5-897b-f09c433f4695", "1cbade01-c4d5-4480-8495-a4088c6acdb9", 156750.0, "8d000849-4121-44b3-9777-3057c1513170", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true },
                    { "c0adfafc-58d2-4c95-be90-d98cb24678e0", "1cbade01-c4d5-4480-8495-a4088c6acdb9", 185250.0, "e67c5639-c603-47f8-815b-35748a532702", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true },
                    { "cc160bdd-b316-4bf0-be6f-309823ae466f", "658b8200-3165-4f87-9127-4f88c1e910bf", 287500.0, "f6700740-1e8b-4169-aace-5f04b6813cc1", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true },
                    { "cdbe2a27-7208-4a67-9fe6-cccff1fe805e", "8cd11c10-ad0b-4e03-ad52-45ef1df68233", 65000.0, "8d12a359-fc7c-45a6-b58a-e9d2bc7bff3b", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true },
                    { "d4f578fb-41e0-4193-baf2-1cd8cbf26bad", "8cd11c10-ad0b-4e03-ad52-45ef1df68233", 75000.0, "699b952b-1629-4f8f-9fc0-beb98db2ddbf", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true },
                    { "e84d0c9f-51f7-4ab5-b037-01f54a34fcdf", "658b8200-3165-4f87-9127-4f88c1e910bf", 82500.0, "75e98b88-6198-48aa-9ecd-af3e57d9d2a9", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true },
                    { "e8809dd1-7e1d-4539-b4b0-3f0d3ffe3292", "ff8e4878-df25-47c6-bff2-81e2ef25f6ec", 85000.0, "6e3e4f58-30fa-41a4-9aac-ada1f7f607ed", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true },
                    { "f53b1a9c-ea7a-4002-bebf-f059d22c7677", "ff8e4878-df25-47c6-bff2-81e2ef25f6ec", 75000.0, "699b952b-1629-4f8f-9fc0-beb98db2ddbf", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true }
                });

            migrationBuilder.InsertData(
                table: "Orders",
                columns: new[] { "Id", "IsPaid", "OrderDate", "OrderId", "TableCode" },
                values: new object[,]
                {
                    { "88d6e5a7-d47c-4bcf-aac4-5fbb01d02e23", true, new DateTime(2025, 1, 15, 13, 15, 0, 0, DateTimeKind.Unspecified), "ORD002", "T006" },
                    { "d0945bce-518c-4f47-8c6b-94c70c86b93a", false, new DateTime(2025, 1, 15, 12, 30, 0, 0, DateTimeKind.Unspecified), "ORD001", "T001" },
                    { "f832f1d1-a77f-4355-a9ed-ad4b31668ab1", false, new DateTime(2025, 1, 15, 14, 0, 0, 0, DateTimeKind.Unspecified), "ORD003", "T004" }
                });

            migrationBuilder.InsertData(
                table: "OrderDetails",
                columns: new[] { "Id", "DishId", "OrderDetailId", "OrderId", "Quantity", "UnitPrice" },
                values: new object[,]
                {
                    { "491ef346-f99f-48bd-8be4-00b7378d5ea6", "D008", "OD005", "ORD003", 1, 185250.0 },
                    { "73f80a8d-1363-4416-bd68-e5a0cd7836c6", "D014", "OD004", "ORD002", 2, 93500.0 },
                    { "951deff1-28b2-4529-b5e9-c1f1ae424bfc", "D001", "OD003", "ORD002", 1, 402500.0 },
                    { "d940d964-e021-4363-a783-f8a675626b53", "D015", "OD006", "ORD003", 3, 25000.0 },
                    { "de0156cc-dffd-4f6b-a925-e767367d35b5", "D001", "OD001", "ORD001", 2, 350000.0 },
                    { "fc9ddb44-8efb-471d-8b76-58c913b065c0", "D004", "OD002", "ORD001", 1, 75000.0 }
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

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetails_DishId",
                table: "OrderDetails",
                column: "DishId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetails_OrderId",
                table: "OrderDetails",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_TableCode",
                table: "Orders",
                column: "TableCode");

            migrationBuilder.CreateIndex(
                name: "IX_Tables_AreaId",
                table: "Tables",
                column: "AreaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AreaDishPrices");

            migrationBuilder.DropTable(
                name: "OrderDetails");

            migrationBuilder.DropTable(
                name: "Dishes");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "Kitchens");

            migrationBuilder.DropTable(
                name: "Tables");

            migrationBuilder.DropTable(
                name: "Areas");
        }
    }
}
