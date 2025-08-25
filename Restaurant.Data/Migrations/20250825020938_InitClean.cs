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
                    AreaId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    AreaName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Id = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Areas", x => x.AreaId);
                });

            migrationBuilder.CreateTable(
                name: "DishGroups",
                columns: table => new
                {
                    GroupId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    GroupName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Id = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DishGroups", x => x.GroupId);
                });

            migrationBuilder.CreateTable(
                name: "Kitchens",
                columns: table => new
                {
                    KitchenId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    KitchenName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Id = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Kitchens", x => x.KitchenId);
                });

            migrationBuilder.CreateTable(
                name: "Tables",
                columns: table => new
                {
                    TableId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TableCode = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TableName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Capacity = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AreaId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Id = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tables", x => x.TableId);
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
                    DishId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    DishName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BasePrice = table.Column<double>(type: "float", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    KitchenId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    GroupId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Id = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dishes", x => x.DishId);
                    table.ForeignKey(
                        name: "FK_Dishes_DishGroups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "DishGroups",
                        principalColumn: "GroupId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Dishes_Kitchens_KitchenId",
                        column: x => x.KitchenId,
                        principalTable: "Kitchens",
                        principalColumn: "KitchenId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TableSessions",
                columns: table => new
                {
                    SessionId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TableId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    OpenAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CloseAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    OpenedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClosedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Id = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TableSessions", x => x.SessionId);
                    table.ForeignKey(
                        name: "FK_TableSessions_Tables_TableId",
                        column: x => x.TableId,
                        principalTable: "Tables",
                        principalColumn: "TableId",
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
                        principalColumn: "AreaId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AreaDishPrices_Dishes_DishId",
                        column: x => x.DishId,
                        principalTable: "Dishes",
                        principalColumn: "DishId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    OrderId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ClosedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsPaid = table.Column<bool>(type: "bit", nullable: false),
                    PrimaryAreaId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    OrderStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TableSessionId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Id = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.OrderId);
                    table.ForeignKey(
                        name: "FK_Orders_Areas_PrimaryAreaId",
                        column: x => x.PrimaryAreaId,
                        principalTable: "Areas",
                        principalColumn: "AreaId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Orders_TableSessions_TableSessionId",
                        column: x => x.TableSessionId,
                        principalTable: "TableSessions",
                        principalColumn: "SessionId");
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
                    UnitPrice = table.Column<double>(type: "float", nullable: false),
                    TableId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    AreaId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    PriceSource = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderDetails_Areas_AreaId",
                        column: x => x.AreaId,
                        principalTable: "Areas",
                        principalColumn: "AreaId");
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
                    table.ForeignKey(
                        name: "FK_OrderDetails_Tables_TableId",
                        column: x => x.TableId,
                        principalTable: "Tables",
                        principalColumn: "TableId");
                });

            migrationBuilder.CreateTable(
                name: "OrderTables",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    OrderId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TableId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    IsPrimary = table.Column<bool>(type: "bit", nullable: false),
                    FromTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ToTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderTables", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderTables_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "OrderId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderTables_Tables_TableId",
                        column: x => x.TableId,
                        principalTable: "Tables",
                        principalColumn: "TableId");
                });

            migrationBuilder.InsertData(
                table: "Areas",
                columns: new[] { "AreaId", "AreaName", "CreatedAt", "Description", "Id", "IsActive" },
                values: new object[,]
                {
                    { "A001", "Tầng Trệt - Khu A", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Khu vực tầng trệt, phù hợp cho gia đình", "A001-STATIC-ID-GUID-000000000001", true },
                    { "A002", "Tầng Trệt - Khu B", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Khu vực tầng trệt, gần cửa sổ", "A002-STATIC-ID-GUID-000000000002", true },
                    { "A003", "Tầng 2 - Khu VIP", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Khu vực VIP tầng 2, view đẹp", "A003-STATIC-ID-GUID-000000000003", true },
                    { "A004", "Tầng 2 - Khu Thường", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Khu vực thường tầng 2", "A004-STATIC-ID-GUID-000000000004", true },
                    { "A005", "Sân Thượng", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Khu vực sân thượng, không gian mở", "A005-STATIC-ID-GUID-000000000005", false },
                    { "A006", "Tầng Trệt - Khu C", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Khu vực yên tĩnh, phù hợp làm việc", "A006-STATIC-ID-GUID-000000000006", true },
                    { "A007", "Tầng 3 - Private Room", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Phòng riêng cho doanh nghiệp và tiệc nhỏ", "A007-STATIC-ID-GUID-000000000007", true }
                });

            migrationBuilder.InsertData(
                table: "DishGroups",
                columns: new[] { "GroupId", "CreatedAt", "Description", "GroupName", "Id", "IsActive" },
                values: new object[,]
                {
                    { "DG001", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Các món ăn khai vị, salad và appetizer", "Món Khai Vị", "DG001-STATIC-ID-GUID-00000000001", true },
                    { "DG002", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Các món ăn chính như steak, pasta, cơm, phở", "Món Chính", "DG002-STATIC-ID-GUID-00000000002", true },
                    { "DG003", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Các món nướng, BBQ và thịt nướng", "Món Nướng BBQ", "DG003-STATIC-ID-GUID-00000000003", true },
                    { "DG004", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Các món hải sản tươi sống", "Hải Sản", "DG004-STATIC-ID-GUID-00000000004", true },
                    { "DG005", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Các món lẩu, nồi và ăn tập thể", "Lẩu & Nồi", "DG005-STATIC-ID-GUID-00000000005", true },
                    { "DG006", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Các món tráng miệng, bánh ngọt và dessert", "Tráng Miệng", "DG006-STATIC-ID-GUID-00000000006", true },
                    { "DG007", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Đồ uống, cocktail, nước ép và cà phê", "Thức Uống", "DG007-STATIC-ID-GUID-00000000007", true }
                });

            migrationBuilder.InsertData(
                table: "Kitchens",
                columns: new[] { "KitchenId", "CreatedAt", "Description", "Id", "IsActive", "KitchenName" },
                values: new object[,]
                {
                    { "K001", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Chuyên món Âu, Mỹ, Ý", "K001-STATIC-ID-GUID-000000000001", true, "Bếp Âu" },
                    { "K002", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Chuyên món Việt, Trung, Nhật, Hàn", "K002-STATIC-ID-GUID-000000000002", true, "Bếp Á" },
                    { "K003", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Chuyên các món nướng, BBQ", "K003-STATIC-ID-GUID-000000000003", true, "Bếp Nướng BBQ" },
                    { "K004", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Chuyên tráng miệng, đồ ngọt", "K004-STATIC-ID-GUID-000000000004", true, "Bếp Dessert" },
                    { "K005", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Chuyên pha chế đồ uống, cocktail", "K005-STATIC-ID-GUID-000000000005", true, "Bar & Thức Uống" },
                    { "K006", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Chuyên các món lẩu, nồi và ăn tập thể", "K006-STATIC-ID-GUID-000000000006", true, "Bếp Lẩu & Nồi" },
                    { "K007", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Chuyên chế biến hải sản tươi sống", "K007-STATIC-ID-GUID-000000000007", true, "Bếp Hải Sản" }
                });

            migrationBuilder.InsertData(
                table: "Dishes",
                columns: new[] { "DishId", "BasePrice", "CreatedAt", "DishName", "GroupId", "Id", "IsActive", "KitchenId" },
                values: new object[,]
                {
                    { "D001", 350000.0, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Steak Bò Mỹ", "DG002", "D001-STATIC-ID-GUID-000000000001", true, "K001" },
                    { "D002", 180000.0, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Pasta Carbonara", "DG002", "D002-STATIC-ID-GUID-000000000002", true, "K001" },
                    { "D003", 280000.0, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Salmon Teriyaki", "DG002", "D003-STATIC-ID-GUID-000000000003", true, "K001" },
                    { "D004", 75000.0, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Phở Bò Đặc Biệt", "DG002", "D004-STATIC-ID-GUID-000000000004", true, "K002" },
                    { "D005", 65000.0, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Bún Chả Hà Nội", "DG002", "D005-STATIC-ID-GUID-000000000005", true, "K002" },
                    { "D006", 85000.0, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Cơm Gà Hải Nam", "DG002", "D006-STATIC-ID-GUID-000000000006", true, "K002" },
                    { "D007", 250000.0, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Lẩu Thái Chua Cay", "DG005", "D007-STATIC-ID-GUID-000000000007", true, "K002" },
                    { "D008", 195000.0, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Sườn Nướng BBQ", "DG003", "D008-STATIC-ID-GUID-000000000008", true, "K003" },
                    { "D009", 165000.0, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Gà Nướng Mật Ong", "DG003", "D009-STATIC-ID-GUID-000000000009", true, "K003" },
                    { "D010", 145000.0, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Bò Nướng Lá Lốt", "DG003", "D010-STATIC-ID-GUID-000000000010", true, "K003" },
                    { "D011", 65000.0, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Tiramisu", "DG006", "D011-STATIC-ID-GUID-000000000011", true, "K004" },
                    { "D012", 75000.0, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Chocolate Lava Cake", "DG006", "D012-STATIC-ID-GUID-000000000012", true, "K004" },
                    { "D013", 45000.0, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Kem Vanilla Pháp", "DG006", "D013-STATIC-ID-GUID-000000000013", true, "K004" },
                    { "D014", 85000.0, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Mojito Classic", "DG007", "D014-STATIC-ID-GUID-000000000014", true, "K005" },
                    { "D015", 25000.0, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Cà Phê Sữa Đá", "DG007", "D015-STATIC-ID-GUID-000000000015", true, "K005" },
                    { "D016", 35000.0, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Sinh Tố Bơ", "DG007", "D016-STATIC-ID-GUID-000000000016", true, "K005" },
                    { "D020", 220000.0, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Chicken Cordon Bleu", "DG002", "D020-STATIC-ID-GUID-000000000020", true, "K001" },
                    { "D021", 380000.0, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Lamb Chop Rosemary", "DG002", "D021-STATIC-ID-GUID-000000000021", true, "K001" },
                    { "D022", 165000.0, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Mushroom Risotto", "DG002", "D022-STATIC-ID-GUID-000000000022", true, "K001" },
                    { "D023", 95000.0, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Pad Thai Tôm", "DG002", "D023-STATIC-ID-GUID-000000000023", true, "K002" },
                    { "D024", 320000.0, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Sushi Set Deluxe", "DG002", "D024-STATIC-ID-GUID-000000000024", true, "K002" },
                    { "D025", 135000.0, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Ramen Tonkotsu", "DG002", "D025-STATIC-ID-GUID-000000000025", true, "K002" },
                    { "D026", 185000.0, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Chả Cá Lăng", "DG003", "D026-STATIC-ID-GUID-000000000026", true, "K003" },
                    { "D027", 210000.0, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Thịt Nướng Hàn Quốc", "DG003", "D027-STATIC-ID-GUID-000000000027", true, "K003" },
                    { "D028", 35000.0, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Bánh Flan Caramel", "DG006", "D028-STATIC-ID-GUID-000000000028", true, "K004" },
                    { "D029", 55000.0, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Panna Cotta Berry", "DG006", "D029-STATIC-ID-GUID-000000000029", true, "K004" },
                    { "D030", 45000.0, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Trà Đào Cam Sả", "DG007", "D030-STATIC-ID-GUID-000000000030", true, "K005" },
                    { "D031", 95000.0, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Cocktail Passion Fruit", "DG007", "D031-STATIC-ID-GUID-000000000031", true, "K005" },
                    { "D032", 280000.0, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Lẩu Bò Nhúng Dấm", "DG005", "D032-STATIC-ID-GUID-000000000032", true, "K006" },
                    { "D033", 220000.0, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Lẩu Gà Lá Giang", "DG005", "D033-STATIC-ID-GUID-000000000033", true, "K006" },
                    { "D034", 195000.0, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Nồi Cá Kho Tộ", "DG005", "D034-STATIC-ID-GUID-000000000034", true, "K006" },
                    { "D035", 650000.0, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Tôm Hùm Nướng Phô Mai", "DG004", "D035-STATIC-ID-GUID-000000000035", true, "K007" },
                    { "D036", 420000.0, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Cua Rang Me", "DG004", "D036-STATIC-ID-GUID-000000000036", true, "K007" },
                    { "D037", 380000.0, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Cá Mú Hấp Gừng", "DG004", "D037-STATIC-ID-GUID-000000000037", true, "K007" },
                    { "D038", 85000.0, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Nghêu Hấp Lá Chuối", "DG004", "D038-STATIC-ID-GUID-000000000038", true, "K007" }
                });

            migrationBuilder.InsertData(
                table: "Orders",
                columns: new[] { "OrderId", "ClosedAt", "CreatedAt", "Id", "IsPaid", "OrderStatus", "PrimaryAreaId", "TableSessionId" },
                values: new object[,]
                {
                    { "ORD001", null, new DateTime(2025, 1, 15, 12, 30, 0, 0, DateTimeKind.Unspecified), "ORD001-STATIC-ID-GUID-0000000001", false, "Open", "A001", null },
                    { "ORD002", new DateTime(2025, 1, 15, 15, 30, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 1, 15, 13, 15, 0, 0, DateTimeKind.Unspecified), "ORD002-STATIC-ID-GUID-0000000002", true, "Paid", "A003", null },
                    { "ORD003", null, new DateTime(2025, 1, 15, 14, 0, 0, 0, DateTimeKind.Unspecified), "ORD003-STATIC-ID-GUID-0000000003", false, "Open", "A002", null },
                    { "ORD004", null, new DateTime(2025, 1, 15, 18, 15, 0, 0, DateTimeKind.Unspecified), "ORD004-STATIC-ID-GUID-0000000004", false, "Open", "A002", null },
                    { "ORD005", null, new DateTime(2025, 1, 15, 19, 30, 0, 0, DateTimeKind.Unspecified), "ORD005-STATIC-ID-GUID-0000000005", false, "Open", "A007", null },
                    { "ORD006", new DateTime(2025, 1, 14, 22, 30, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 1, 14, 20, 0, 0, 0, DateTimeKind.Unspecified), "ORD006-STATIC-ID-GUID-0000000006", true, "Paid", "A003", null }
                });

            migrationBuilder.InsertData(
                table: "Tables",
                columns: new[] { "TableId", "AreaId", "Capacity", "Id", "IsActive", "Status", "TableCode", "TableName" },
                values: new object[,]
                {
                    { "T001", "A001", 4, "T001-STATIC-ID-GUID-000000000001", true, "Available", "T001", "Bàn A1" },
                    { "T002", "A001", 6, "T002-STATIC-ID-GUID-000000000002", true, "Available", "T002", "Bàn A2" },
                    { "T003", "A001", 2, "T003-STATIC-ID-GUID-000000000003", true, "Available", "T003", "Bàn A3" },
                    { "T004", "A002", 4, "T004-STATIC-ID-GUID-000000000004", true, "Available", "T004", "Bàn B1" },
                    { "T005", "A002", 8, "T005-STATIC-ID-GUID-000000000005", true, "Available", "T005", "Bàn B2" },
                    { "T006", "A003", 6, "T006-STATIC-ID-GUID-000000000006", true, "Available", "T006", "Bàn VIP 1" },
                    { "T007", "A003", 10, "T007-STATIC-ID-GUID-000000000007", true, "Available", "T007", "Bàn VIP 2" },
                    { "T008", "A004", 4, "T008-STATIC-ID-GUID-000000000008", true, "Available", "T008", "Bàn T2-1" },
                    { "T009", "A004", 6, "T009-STATIC-ID-GUID-000000000009", true, "Available", "T009", "Bàn T2-2" },
                    { "T010", "A001", 8, "T010-STATIC-ID-GUID-000000000010", true, "Available", "T010", "Bàn A4" },
                    { "T011", "A002", 6, "T011-STATIC-ID-GUID-000000000011", true, "Available", "T011", "Bàn B3" },
                    { "T012", "A003", 12, "T012-STATIC-ID-GUID-000000000012", true, "Available", "T012", "Bàn VIP 3" },
                    { "T013", "A004", 4, "T013-STATIC-ID-GUID-000000000013", true, "Available", "T013", "Bàn T2-3" },
                    { "T014", "A006", 2, "T014-STATIC-ID-GUID-000000000014", true, "Available", "T014", "Bàn C1" },
                    { "T015", "A006", 4, "T015-STATIC-ID-GUID-000000000015", true, "Available", "T015", "Bàn C2" },
                    { "T016", "A007", 15, "T016-STATIC-ID-GUID-000000000016", true, "Available", "T016", "Phòng Riêng 1" },
                    { "T017", "A007", 20, "T017-STATIC-ID-GUID-000000000017", true, "Available", "T017", "Phòng Riêng 2" }
                });

            migrationBuilder.InsertData(
                table: "AreaDishPrices",
                columns: new[] { "Id", "AreaId", "CustomPrice", "DishId", "EffectiveDate", "IsActive" },
                values: new object[,]
                {
                    { "ADP001-STATIC-ID-GUID-000000001", "A001", 350000.0, "D001", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true },
                    { "ADP002-STATIC-ID-GUID-000000002", "A001", 75000.0, "D004", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true },
                    { "ADP003-STATIC-ID-GUID-000000003", "A001", 65000.0, "D011", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true },
                    { "ADP004-STATIC-ID-GUID-000000004", "A001", 85000.0, "D014", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true },
                    { "ADP005-STATIC-ID-GUID-000000005", "A001", 25000.0, "D015", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true },
                    { "ADP006-STATIC-ID-GUID-000000006", "A002", 185250.0, "D008", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true },
                    { "ADP007-STATIC-ID-GUID-000000007", "A002", 156750.0, "D009", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true },
                    { "ADP008-STATIC-ID-GUID-000000008", "A002", 137750.0, "D010", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true },
                    { "ADP009-STATIC-ID-GUID-000000009", "A002", 199500.0, "D027", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true },
                    { "ADP010-STATIC-ID-GUID-000000010", "A003", 402500.0, "D001", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true },
                    { "ADP011-STATIC-ID-GUID-000000011", "A003", 207000.0, "D002", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true },
                    { "ADP012-STATIC-ID-GUID-000000012", "A003", 322000.0, "D003", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true },
                    { "ADP013-STATIC-ID-GUID-000000013", "A003", 287500.0, "D007", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true },
                    { "ADP014-STATIC-ID-GUID-000000014", "A003", 437000.0, "D021", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true },
                    { "ADP015-STATIC-ID-GUID-000000015", "A003", 71500.0, "D011", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true },
                    { "ADP016-STATIC-ID-GUID-000000016", "A003", 82500.0, "D012", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true },
                    { "ADP017-STATIC-ID-GUID-000000017", "A003", 93500.0, "D014", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true },
                    { "ADP018-STATIC-ID-GUID-000000018", "A004", 75000.0, "D004", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true },
                    { "ADP019-STATIC-ID-GUID-000000019", "A004", 65000.0, "D005", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true },
                    { "ADP020-STATIC-ID-GUID-000000020", "A004", 85000.0, "D006", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true },
                    { "ADP021-STATIC-ID-GUID-000000021", "A004", 135000.0, "D025", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true },
                    { "ADP022-STATIC-ID-GUID-000000022", "A006", 24250.0, "D015", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true },
                    { "ADP023-STATIC-ID-GUID-000000023", "A006", 43650.0, "D030", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true },
                    { "ADP024-STATIC-ID-GUID-000000024", "A006", 160050.0, "D022", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true },
                    { "ADP025-STATIC-ID-GUID-000000025", "A007", 456000.0, "D021", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true },
                    { "ADP026-STATIC-ID-GUID-000000026", "A007", 384000.0, "D024", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true },
                    { "ADP027-STATIC-ID-GUID-000000027", "A007", 780000.0, "D035", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true },
                    { "ADP028-STATIC-ID-GUID-000000028", "A007", 114000.0, "D031", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true }
                });

            migrationBuilder.InsertData(
                table: "OrderDetails",
                columns: new[] { "Id", "AreaId", "DishId", "OrderDetailId", "OrderId", "PriceSource", "Quantity", "TableId", "UnitPrice" },
                values: new object[,]
                {
                    { "OD001-STATIC-ID-GUID-0000000001", "A001", "D001", "OD001", "ORD001", "Base", 2, "T001", 350000.0 },
                    { "OD002-STATIC-ID-GUID-0000000002", "A001", "D004", "OD002", "ORD001", "Base", 1, "T001", 75000.0 },
                    { "OD003-STATIC-ID-GUID-0000000003", "A003", "D001", "OD003", "ORD002", "Custom", 1, "T006", 402500.0 },
                    { "OD004-STATIC-ID-GUID-0000000004", "A003", "D014", "OD004", "ORD002", "Custom", 2, "T006", 93500.0 },
                    { "OD005-STATIC-ID-GUID-0000000005", "A002", "D008", "OD005", "ORD003", "Custom", 1, "T004", 185250.0 },
                    { "OD006-STATIC-ID-GUID-0000000006", "A002", "D015", "OD006", "ORD003", "Base", 3, "T004", 25000.0 },
                    { "OD007-STATIC-ID-GUID-0000000007", "A002", "D027", "OD007", "ORD004", "Base", 2, "T011", 210000.0 },
                    { "OD008-STATIC-ID-GUID-0000000008", "A002", "D032", "OD008", "ORD004", "Base", 1, "T011", 280000.0 },
                    { "OD009-STATIC-ID-GUID-0000000009", "A007", "D021", "OD009", "ORD005", "Base", 5, "T016", 380000.0 },
                    { "OD010-STATIC-ID-GUID-0000000010", "A007", "D024", "OD010", "ORD005", "Base", 3, "T016", 320000.0 },
                    { "OD011-STATIC-ID-GUID-0000000011", "A003", "D036", "OD011", "ORD006", "Base", 2, "T012", 420000.0 },
                    { "OD012-STATIC-ID-GUID-0000000012", "A003", "D037", "OD012", "ORD006", "Base", 1, "T012", 380000.0 },
                    { "OD013-STATIC-ID-GUID-0000000013", "A003", "D011", "OD013", "ORD006", "Custom", 4, "T012", 71500.0 },
                    { "OD014-STATIC-ID-GUID-0000000014", "A003", "D014", "OD014", "ORD006", "Custom", 3, "T012", 93500.0 },
                    { "OD015-STATIC-ID-GUID-0000000015", "A001", "D015", "OD015", "ORD001", "Base", 3, "T001", 25000.0 },
                    { "OD016-STATIC-ID-GUID-0000000016", "A003", "D035", "OD016", "ORD002", "Base", 1, "T006", 650000.0 },
                    { "OD017-STATIC-ID-GUID-0000000017", "A002", "D023", "OD017", "ORD003", "Base", 2, "T004", 95000.0 },
                    { "OD018-STATIC-ID-GUID-0000000018", "A002", "D030", "OD018", "ORD004", "Base", 4, "T011", 45000.0 },
                    { "OD019-STATIC-ID-GUID-0000000019", "A007", "D031", "OD019", "ORD005", "Base", 8, "T016", 95000.0 }
                });

            migrationBuilder.InsertData(
                table: "OrderTables",
                columns: new[] { "Id", "FromTime", "IsPrimary", "OrderId", "TableId", "ToTime" },
                values: new object[,]
                {
                    { "OT001-STATIC-ID-GUID-0000000001", new DateTime(2025, 1, 15, 12, 30, 0, 0, DateTimeKind.Unspecified), true, "ORD001", "T001", null },
                    { "OT002-STATIC-ID-GUID-0000000002", new DateTime(2025, 1, 15, 13, 15, 0, 0, DateTimeKind.Unspecified), true, "ORD002", "T006", new DateTime(2025, 1, 15, 15, 30, 0, 0, DateTimeKind.Unspecified) },
                    { "OT003-STATIC-ID-GUID-0000000003", new DateTime(2025, 1, 15, 14, 0, 0, 0, DateTimeKind.Unspecified), true, "ORD003", "T004", null },
                    { "OT004-STATIC-ID-GUID-0000000004", new DateTime(2025, 1, 15, 18, 15, 0, 0, DateTimeKind.Unspecified), true, "ORD004", "T011", null },
                    { "OT005-STATIC-ID-GUID-0000000005", new DateTime(2025, 1, 15, 19, 30, 0, 0, DateTimeKind.Unspecified), true, "ORD005", "T016", null },
                    { "OT006-STATIC-ID-GUID-0000000006", new DateTime(2025, 1, 14, 20, 0, 0, 0, DateTimeKind.Unspecified), true, "ORD006", "T012", new DateTime(2025, 1, 14, 22, 30, 0, 0, DateTimeKind.Unspecified) }
                });

            migrationBuilder.InsertData(
                table: "TableSessions",
                columns: new[] { "SessionId", "CloseAt", "ClosedBy", "Id", "OpenAt", "OpenedBy", "Status", "TableId" },
                values: new object[,]
                {
                    { "TS001", null, null, "TS001-STATIC-ID-GUID-00000000001", new DateTime(2025, 1, 15, 12, 0, 0, 0, DateTimeKind.Unspecified), "Staff001", "Available", "T001" },
                    { "TS002", new DateTime(2025, 1, 15, 15, 30, 0, 0, DateTimeKind.Unspecified), "Staff002", "TS002-STATIC-ID-GUID-00000000002", new DateTime(2025, 1, 15, 13, 0, 0, 0, DateTimeKind.Unspecified), "Staff001", "Closed", "T006" },
                    { "TS003", null, null, "TS003-STATIC-ID-GUID-00000000003", new DateTime(2025, 1, 15, 18, 0, 0, 0, DateTimeKind.Unspecified), "Staff003", "Available", "T011" },
                    { "TS004", new DateTime(2025, 1, 15, 16, 0, 0, 0, DateTimeKind.Unspecified), "Staff001", "TS004-STATIC-ID-GUID-00000000004", new DateTime(2025, 1, 15, 14, 30, 0, 0, DateTimeKind.Unspecified), "Staff002", "Closed", "T014" },
                    { "TS005", null, null, "TS005-STATIC-ID-GUID-00000000005", new DateTime(2025, 1, 15, 19, 0, 0, 0, DateTimeKind.Unspecified), "Staff004", "Available", "T016" }
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
                name: "IX_Dishes_GroupId",
                table: "Dishes",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Dishes_KitchenId",
                table: "Dishes",
                column: "KitchenId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetails_AreaId",
                table: "OrderDetails",
                column: "AreaId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetails_DishId",
                table: "OrderDetails",
                column: "DishId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetails_OrderId",
                table: "OrderDetails",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetails_TableId",
                table: "OrderDetails",
                column: "TableId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_PrimaryAreaId",
                table: "Orders",
                column: "PrimaryAreaId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_TableSessionId",
                table: "Orders",
                column: "TableSessionId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderTables_OrderId",
                table: "OrderTables",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderTables_TableId",
                table: "OrderTables",
                column: "TableId");

            migrationBuilder.CreateIndex(
                name: "IX_Tables_AreaId",
                table: "Tables",
                column: "AreaId");

            migrationBuilder.CreateIndex(
                name: "IX_TableSessions_TableId",
                table: "TableSessions",
                column: "TableId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AreaDishPrices");

            migrationBuilder.DropTable(
                name: "OrderDetails");

            migrationBuilder.DropTable(
                name: "OrderTables");

            migrationBuilder.DropTable(
                name: "Dishes");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "DishGroups");

            migrationBuilder.DropTable(
                name: "Kitchens");

            migrationBuilder.DropTable(
                name: "TableSessions");

            migrationBuilder.DropTable(
                name: "Tables");

            migrationBuilder.DropTable(
                name: "Areas");
        }
    }
}
