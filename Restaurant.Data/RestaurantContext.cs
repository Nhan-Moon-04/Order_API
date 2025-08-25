using Microsoft.EntityFrameworkCore;
using Restaurant.Domain.Entities;
using Restaurant.Domain.Enums;

namespace Restaurant.Data
{
    public class RestaurantDbContext : DbContext
    {
        public RestaurantDbContext(DbContextOptions<RestaurantDbContext> options)
            : base(options) { }

        public DbSet<Areas> Areas { get; set; }
        public DbSet<Table> Tables { get; set; }
        public DbSet<TableSession> TableSessions { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderTable> OrderTables { get; set; }
        public DbSet<Dishes> Dishes { get; set; }
        public DbSet<DishGroup> DishGroups { get; set; }
        public DbSet<AreaDishPrices> AreaDishPrices { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<Kitchens> Kitchens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            var fixedDate = new DateTime(2025, 01, 01);

            // Configure enum to string conversion
            modelBuilder.Entity<Table>()
                .Property(t => t.Status)
                .HasConversion<string>();

            modelBuilder.Entity<TableSession>()
                .Property(ts => ts.Status)
                .HasConversion<string>();

            modelBuilder.Entity<Order>()
                .Property(o => o.OrderStatus)
                .HasConversion<string>();

            modelBuilder.Entity<OrderDetail>()
                .Property(od => od.PriceSource)
                .HasConversion<string>();

            // Configure primary keys using business keys
            modelBuilder.Entity<Areas>()
                .HasKey(a => a.AreaId);
            
            modelBuilder.Entity<Table>()
                .HasKey(t => t.TableId);
            
            modelBuilder.Entity<TableSession>()
                .HasKey(ts => ts.SessionId);
            
            modelBuilder.Entity<Order>()
                .HasKey(o => o.OrderId);
            
            modelBuilder.Entity<Dishes>()
                .HasKey(d => d.DishId);
            
            modelBuilder.Entity<DishGroup>()
                .HasKey(dg => dg.GroupId);
            
            modelBuilder.Entity<Kitchens>()
                .HasKey(k => k.KitchenId);

            // Configure unique constraints for codes
            modelBuilder.Entity<Table>()
                .HasAlternateKey(t => t.TableCode);

            // Configure foreign key relationships
            modelBuilder.Entity<Table>()
                .HasOne(t => t.Area)
                .WithMany(a => a.Tables)
                .HasForeignKey(t => t.AreaId);

            modelBuilder.Entity<TableSession>()
                .HasOne(ts => ts.Table)
                .WithMany(t => t.TableSessions)
                .HasForeignKey(ts => ts.TableId);

            modelBuilder.Entity<Order>()
                .HasOne(o => o.PrimaryArea)
                .WithMany(a => a.PrimaryOrders)
                .HasForeignKey(o => o.PrimaryAreaId);

            // Configure Order to TableSession relationship
            modelBuilder.Entity<Order>()
                .HasOne(o => o.TableSession)
                .WithMany(ts => ts.Orders)
                .HasForeignKey(o => o.TableSessionId)
                .OnDelete(DeleteBehavior.NoAction); // Changed from SetNull to NoAction to avoid cascade conflicts

            modelBuilder.Entity<OrderTable>()
                .HasOne(ot => ot.Order)
                .WithMany(o => o.OrderTables)
                .HasForeignKey(ot => ot.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<OrderTable>()
                .HasOne(ot => ot.Table)
                .WithMany(t => t.OrderTables)
                .HasForeignKey(ot => ot.TableId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Dishes>()
                .HasOne(d => d.Kitchen)
                .WithMany(k => k.Dishes)
                .HasForeignKey(d => d.KitchenId);

            modelBuilder.Entity<Dishes>()
                .HasOne(d => d.DishGroup)
                .WithMany(dg => dg.Dishes)
                .HasForeignKey(d => d.GroupId);

            modelBuilder.Entity<AreaDishPrices>()
                .HasOne(adp => adp.Area)
                .WithMany(a => a.AreaDishPrices)
                .HasForeignKey(adp => adp.AreaId);

            modelBuilder.Entity<AreaDishPrices>()
                .HasOne(adp => adp.Dish)
                .WithMany(d => d.AreaDishPrices)
                .HasForeignKey(adp => adp.DishId);

            modelBuilder.Entity<OrderDetail>()
                .HasOne(od => od.Order)
                .WithMany(o => o.OrderDetails)
                .HasForeignKey(od => od.OrderId);

            modelBuilder.Entity<OrderDetail>()
                .HasOne(od => od.Dish)
                .WithMany(d => d.OrderDetails)
                .HasForeignKey(od => od.DishId);

            modelBuilder.Entity<OrderDetail>()
                .HasOne(od => od.Table)
                .WithMany(t => t.OrderDetails)
                .HasForeignKey(od => od.TableId);

            modelBuilder.Entity<OrderDetail>()
                .HasOne(od => od.Area)
                .WithMany(a => a.OrderDetails)
                .HasForeignKey(od => od.AreaId);

            // Seed DishGroups with static IDs
            modelBuilder.Entity<DishGroup>().HasData(
                new DishGroup 
                { 
                    Id = "DG001-STATIC-ID-GUID-00000000001",
                    GroupId = "DG001", 
                    GroupName = "Món Khai Vị", 
                    Description = "Các món ăn khai vị, salad và appetizer", 
                    IsActive = true, 
                    CreatedAt = fixedDate 
                },
                new DishGroup 
                { 
                    Id = "DG002-STATIC-ID-GUID-00000000002",
                    GroupId = "DG002", 
                    GroupName = "Món Chính", 
                    Description = "Các món ăn chính như steak, pasta, cơm, phở", 
                    IsActive = true, 
                    CreatedAt = fixedDate 
                },
                new DishGroup 
                { 
                    Id = "DG003-STATIC-ID-GUID-00000000003",
                    GroupId = "DG003", 
                    GroupName = "Món Nướng BBQ", 
                    Description = "Các món nướng, BBQ và thịt nướng", 
                    IsActive = true, 
                    CreatedAt = fixedDate 
                },
                new DishGroup 
                { 
                    Id = "DG004-STATIC-ID-GUID-00000000004",
                    GroupId = "DG004", 
                    GroupName = "Hải Sản", 
                    Description = "Các món hải sản tươi sống", 
                    IsActive = true, 
                    CreatedAt = fixedDate 
                },
                new DishGroup 
                { 
                    Id = "DG005-STATIC-ID-GUID-00000000005",
                    GroupId = "DG005", 
                    GroupName = "Lẩu & Nồi", 
                    Description = "Các món lẩu, nồi và ăn tập thể", 
                    IsActive = true, 
                    CreatedAt = fixedDate 
                },
                new DishGroup 
                { 
                    Id = "DG006-STATIC-ID-GUID-00000000006",
                    GroupId = "DG006", 
                    GroupName = "Tráng Miệng", 
                    Description = "Các món tráng miệng, bánh ngọt và dessert", 
                    IsActive = true, 
                    CreatedAt = fixedDate 
                },
                new DishGroup 
                { 
                    Id = "DG007-STATIC-ID-GUID-00000000007",
                    GroupId = "DG007", 
                    GroupName = "Thức Uống", 
                    Description = "Đồ uống, cocktail, nước ép và cà phê", 
                    IsActive = true, 
                    CreatedAt = fixedDate 
                }
            );

            // Seed Areas with static IDs
            modelBuilder.Entity<Areas>().HasData(
                new Areas 
                { 
                    Id = "A001-STATIC-ID-GUID-000000000001",
                    AreaId = "A001", 
                    AreaName = "Tầng Trệt - Khu A", 
                    Description = "Khu vực tầng trệt, phù hợp cho gia đình", 
                    IsActive = true, 
                    CreatedAt = fixedDate 
                },
                new Areas 
                { 
                    Id = "A002-STATIC-ID-GUID-000000000002",
                    AreaId = "A002", 
                    AreaName = "Tầng Trệt - Khu B", 
                    Description = "Khu vực tầng trệt, gần cửa sổ", 
                    IsActive = true, 
                    CreatedAt = fixedDate 
                },
                new Areas 
                { 
                    Id = "A003-STATIC-ID-GUID-000000000003",
                    AreaId = "A003", 
                    AreaName = "Tầng 2 - Khu VIP", 
                    Description = "Khu vực VIP tầng 2, view đẹp", 
                    IsActive = true, 
                    CreatedAt = fixedDate 
                },
                new Areas 
                { 
                    Id = "A004-STATIC-ID-GUID-000000000004",
                    AreaId = "A004", 
                    AreaName = "Tầng 2 - Khu Thường", 
                    Description = "Khu vực thường tầng 2", 
                    IsActive = true, 
                    CreatedAt = fixedDate 
                },
                new Areas 
                { 
                    Id = "A005-STATIC-ID-GUID-000000000005",
                    AreaId = "A005", 
                    AreaName = "Sân Thượng", 
                    Description = "Khu vực sân thượng, không gian mở", 
                    IsActive = false, 
                    CreatedAt = fixedDate 
                },
                new Areas 
                { 
                    Id = "A006-STATIC-ID-GUID-000000000006",
                    AreaId = "A006", 
                    AreaName = "Tầng Trệt - Khu C", 
                    Description = "Khu vực yên tĩnh, phù hợp làm việc", 
                    IsActive = true, 
                    CreatedAt = fixedDate 
                },
                new Areas 
                { 
                    Id = "A007-STATIC-ID-GUID-000000000007",
                    AreaId = "A007", 
                    AreaName = "Tầng 3 - Private Room", 
                    Description = "Phòng riêng cho doanh nghiệp và tiệc nhỏ", 
                    IsActive = true, 
                    CreatedAt = fixedDate 
                }
            );

            // Seed Kitchens with static IDs
            modelBuilder.Entity<Kitchens>().HasData(
                new Kitchens 
                { 
                    Id = "K001-STATIC-ID-GUID-000000000001",
                    KitchenId = "K001", 
                    KitchenName = "Bếp Âu", 
                    Description = "Chuyên món Âu, Mỹ, Ý", 
                    IsActive = true, 
                    CreatedAt = fixedDate 
                },
                new Kitchens 
                { 
                    Id = "K002-STATIC-ID-GUID-000000000002",
                    KitchenId = "K002", 
                    KitchenName = "Bếp Á", 
                    Description = "Chuyên món Việt, Trung, Nhật, Hàn", 
                    IsActive = true, 
                    CreatedAt = fixedDate 
                },
                new Kitchens 
                { 
                    Id = "K003-STATIC-ID-GUID-000000000003",
                    KitchenId = "K003", 
                    KitchenName = "Bếp Nướng BBQ", 
                    Description = "Chuyên các món nướng, BBQ", 
                    IsActive = true, 
                    CreatedAt = fixedDate 
                },
                new Kitchens 
                { 
                    Id = "K004-STATIC-ID-GUID-000000000004",
                    KitchenId = "K004", 
                    KitchenName = "Bếp Dessert", 
                    Description = "Chuyên tráng miệng, đồ ngọt", 
                    IsActive = true, 
                    CreatedAt = fixedDate 
                },
                new Kitchens 
                { 
                    Id = "K005-STATIC-ID-GUID-000000000005",
                    KitchenId = "K005", 
                    KitchenName = "Bar & Thức Uống", 
                    Description = "Chuyên pha chế đồ uống, cocktail", 
                    IsActive = true, 
                    CreatedAt = fixedDate 
                },
                new Kitchens 
                { 
                    Id = "K006-STATIC-ID-GUID-000000000006",
                    KitchenId = "K006", 
                    KitchenName = "Bếp Lẩu & Nồi", 
                    Description = "Chuyên các món lẩu, nồi và ăn tập thể", 
                    IsActive = true, 
                    CreatedAt = fixedDate 
                },
                new Kitchens 
                { 
                    Id = "K007-STATIC-ID-GUID-000000000007",
                    KitchenId = "K007", 
                    KitchenName = "Bếp Hải Sản", 
                    Description = "Chuyên chế biến hải sản tươi sống", 
                    IsActive = true, 
                    CreatedAt = fixedDate 
                }
            );

            // Seed Dishes with static IDs and GroupId assignments
            modelBuilder.Entity<Dishes>().HasData(
                // Món Chính Âu (K001, DG002)
                new Dishes { Id = "D001-STATIC-ID-GUID-000000000001", DishId = "D001", DishName = "Steak Bò Mỹ", BasePrice = 350000, KitchenId = "K001", GroupId = "DG002", IsActive = true, CreatedAt = fixedDate },
                new Dishes { Id = "D002-STATIC-ID-GUID-000000000002", DishId = "D002", DishName = "Pasta Carbonara", BasePrice = 180000, KitchenId = "K001", GroupId = "DG002", IsActive = true, CreatedAt = fixedDate },
                new Dishes { Id = "D003-STATIC-ID-GUID-000000000003", DishId = "D003", DishName = "Salmon Teriyaki", BasePrice = 280000, KitchenId = "K001", GroupId = "DG002", IsActive = true, CreatedAt = fixedDate },
                new Dishes { Id = "D020-STATIC-ID-GUID-000000000020", DishId = "D020", DishName = "Chicken Cordon Bleu", BasePrice = 220000, KitchenId = "K001", GroupId = "DG002", IsActive = true, CreatedAt = fixedDate },
                new Dishes { Id = "D021-STATIC-ID-GUID-000000000021", DishId = "D021", DishName = "Lamb Chop Rosemary", BasePrice = 380000, KitchenId = "K001", GroupId = "DG002", IsActive = true, CreatedAt = fixedDate },
                new Dishes { Id = "D022-STATIC-ID-GUID-000000000022", DishId = "D022", DishName = "Mushroom Risotto", BasePrice = 165000, KitchenId = "K001", GroupId = "DG002", IsActive = true, CreatedAt = fixedDate },

                // Món Chính Á (K002, DG002)
                new Dishes { Id = "D004-STATIC-ID-GUID-000000000004", DishId = "D004", DishName = "Phở Bò Đặc Biệt", BasePrice = 75000, KitchenId = "K002", GroupId = "DG002", IsActive = true, CreatedAt = fixedDate },
                new Dishes { Id = "D005-STATIC-ID-GUID-000000000005", DishId = "D005", DishName = "Bún Chả Hà Nội", BasePrice = 65000, KitchenId = "K002", GroupId = "DG002", IsActive = true, CreatedAt = fixedDate },
                new Dishes { Id = "D006-STATIC-ID-GUID-000000000006", DishId = "D006", DishName = "Cơm Gà Hải Nam", BasePrice = 85000, KitchenId = "K002", GroupId = "DG002", IsActive = true, CreatedAt = fixedDate },
                new Dishes { Id = "D023-STATIC-ID-GUID-000000000023", DishId = "D023", DishName = "Pad Thai Tôm", BasePrice = 95000, KitchenId = "K002", GroupId = "DG002", IsActive = true, CreatedAt = fixedDate },
                new Dishes { Id = "D024-STATIC-ID-GUID-000000000024", DishId = "D024", DishName = "Sushi Set Deluxe", BasePrice = 320000, KitchenId = "K002", GroupId = "DG002", IsActive = true, CreatedAt = fixedDate },
                new Dishes { Id = "D025-STATIC-ID-GUID-000000000025", DishId = "D025", DishName = "Ramen Tonkotsu", BasePrice = 135000, KitchenId = "K002", GroupId = "DG002", IsActive = true, CreatedAt = fixedDate },

                // Lẩu & Nồi (K002, DG005)
                new Dishes { Id = "D007-STATIC-ID-GUID-000000000007", DishId = "D007", DishName = "Lẩu Thái Chua Cay", BasePrice = 250000, KitchenId = "K002", GroupId = "DG005", IsActive = true, CreatedAt = fixedDate },

                // Món Nướng BBQ (K003, DG003)
                new Dishes { Id = "D008-STATIC-ID-GUID-000000000008", DishId = "D008", DishName = "Sườn Nướng BBQ", BasePrice = 195000, KitchenId = "K003", GroupId = "DG003", IsActive = true, CreatedAt = fixedDate },
                new Dishes { Id = "D009-STATIC-ID-GUID-000000000009", DishId = "D009", DishName = "Gà Nướng Mật Ong", BasePrice = 165000, KitchenId = "K003", GroupId = "DG003", IsActive = true, CreatedAt = fixedDate },
                new Dishes { Id = "D010-STATIC-ID-GUID-000000000010", DishId = "D010", DishName = "Bò Nướng Lá Lốt", BasePrice = 145000, KitchenId = "K003", GroupId = "DG003", IsActive = true, CreatedAt = fixedDate },
                new Dishes { Id = "D026-STATIC-ID-GUID-000000000026", DishId = "D026", DishName = "Chả Cá Lăng", BasePrice = 185000, KitchenId = "K003", GroupId = "DG003", IsActive = true, CreatedAt = fixedDate },
                new Dishes { Id = "D027-STATIC-ID-GUID-000000000027", DishId = "D027", DishName = "Thịt Nướng Hàn Quốc", BasePrice = 210000, KitchenId = "K003", GroupId = "DG003", IsActive = true, CreatedAt = fixedDate },

                // Dessert (K004, DG006)
                new Dishes { Id = "D011-STATIC-ID-GUID-000000000011", DishId = "D011", DishName = "Tiramisu", BasePrice = 65000, KitchenId = "K004", GroupId = "DG006", IsActive = true, CreatedAt = fixedDate },
                new Dishes { Id = "D012-STATIC-ID-GUID-000000000012", DishId = "D012", DishName = "Chocolate Lava Cake", BasePrice = 75000, KitchenId = "K004", GroupId = "DG006", IsActive = true, CreatedAt = fixedDate },
                new Dishes { Id = "D013-STATIC-ID-GUID-000000000013", DishId = "D013", DishName = "Kem Vanilla Pháp", BasePrice = 45000, KitchenId = "K004", GroupId = "DG006", IsActive = true, CreatedAt = fixedDate },
                new Dishes { Id = "D028-STATIC-ID-GUID-000000000028", DishId = "D028", DishName = "Bánh Flan Caramel", BasePrice = 35000, KitchenId = "K004", GroupId = "DG006", IsActive = true, CreatedAt = fixedDate },
                new Dishes { Id = "D029-STATIC-ID-GUID-000000000029", DishId = "D029", DishName = "Panna Cotta Berry", BasePrice = 55000, KitchenId = "K004", GroupId = "DG006", IsActive = true, CreatedAt = fixedDate },

                // Thức uống (K005, DG007)
                new Dishes { Id = "D014-STATIC-ID-GUID-000000000014", DishId = "D014", DishName = "Mojito Classic", BasePrice = 85000, KitchenId = "K005", GroupId = "DG007", IsActive = true, CreatedAt = fixedDate },
                new Dishes { Id = "D015-STATIC-ID-GUID-000000000015", DishId = "D015", DishName = "Cà Phê Sữa Đá", BasePrice = 25000, KitchenId = "K005", GroupId = "DG007", IsActive = true, CreatedAt = fixedDate },
                new Dishes { Id = "D016-STATIC-ID-GUID-000000000016", DishId = "D016", DishName = "Sinh Tố Bơ", BasePrice = 35000, KitchenId = "K005", GroupId = "DG007", IsActive = true, CreatedAt = fixedDate },
                new Dishes { Id = "D030-STATIC-ID-GUID-000000000030", DishId = "D030", DishName = "Trà Đào Cam Sả", BasePrice = 45000, KitchenId = "K005", GroupId = "DG007", IsActive = true, CreatedAt = fixedDate },
                new Dishes { Id = "D031-STATIC-ID-GUID-000000000031", DishId = "D031", DishName = "Cocktail Passion Fruit", BasePrice = 95000, KitchenId = "K005", GroupId = "DG007", IsActive = true, CreatedAt = fixedDate },

                // Lẩu & Nồi (K006, DG005)
                new Dishes { Id = "D032-STATIC-ID-GUID-000000000032", DishId = "D032", DishName = "Lẩu Bò Nhúng Dấm", BasePrice = 280000, KitchenId = "K006", GroupId = "DG005", IsActive = true, CreatedAt = fixedDate },
                new Dishes { Id = "D033-STATIC-ID-GUID-000000000033", DishId = "D033", DishName = "Lẩu Gà Lá Giang", BasePrice = 220000, KitchenId = "K006", GroupId = "DG005", IsActive = true, CreatedAt = fixedDate },
                new Dishes { Id = "D034-STATIC-ID-GUID-000000000034", DishId = "D034", DishName = "Nồi Cá Kho Tộ", BasePrice = 195000, KitchenId = "K006", GroupId = "DG005", IsActive = true, CreatedAt = fixedDate },

                // Hải sản (K007, DG004)
                new Dishes { Id = "D035-STATIC-ID-GUID-000000000035", DishId = "D035", DishName = "Tôm Hùm Nướng Phô Mai", BasePrice = 650000, KitchenId = "K007", GroupId = "DG004", IsActive = true, CreatedAt = fixedDate },
                new Dishes { Id = "D036-STATIC-ID-GUID-000000000036", DishId = "D036", DishName = "Cua Rang Me", BasePrice = 420000, KitchenId = "K007", GroupId = "DG004", IsActive = true, CreatedAt = fixedDate },
                new Dishes { Id = "D037-STATIC-ID-GUID-000000000037", DishId = "D037", DishName = "Cá Mú Hấp Gừng", BasePrice = 380000, KitchenId = "K007", GroupId = "DG004", IsActive = true, CreatedAt = fixedDate },
                new Dishes { Id = "D038-STATIC-ID-GUID-000000000038", DishId = "D038", DishName = "Nghêu Hấp Lá Chuối", BasePrice = 85000, KitchenId = "K007", GroupId = "DG004", IsActive = true, CreatedAt = fixedDate }
            );

            // Seed Tables with static IDs
            modelBuilder.Entity<Table>().HasData(
                // Tables for Area A001 (Tầng Trệt - Khu A)
                new Table { Id = "T001-STATIC-ID-GUID-000000000001", TableId = "T001", TableCode = "T001", TableName = "Bàn A1", Capacity = 4, AreaId = "A001", IsActive = true, Status = TableStatus.Available },
                new Table { Id = "T002-STATIC-ID-GUID-000000000002", TableId = "T002", TableCode = "T002", TableName = "Bàn A2", Capacity = 6, AreaId = "A001", IsActive = true, Status = TableStatus.Available },
                new Table { Id = "T003-STATIC-ID-GUID-000000000003", TableId = "T003", TableCode = "T003", TableName = "Bàn A3", Capacity = 2, AreaId = "A001", IsActive = true, Status = TableStatus.Available },
                new Table { Id = "T010-STATIC-ID-GUID-000000000010", TableId = "T010", TableCode = "T010", TableName = "Bàn A4", Capacity = 8, AreaId = "A001", IsActive = true, Status = TableStatus.Available },

                // Tables for Area A002 (Tầng Trệt - Khu B)
                new Table { Id = "T004-STATIC-ID-GUID-000000000004", TableId = "T004", TableCode = "T004", TableName = "Bàn B1", Capacity = 4, AreaId = "A002", IsActive = true, Status = TableStatus.Available },
                new Table { Id = "T005-STATIC-ID-GUID-000000000005", TableId = "T005", TableCode = "T005", TableName = "Bàn B2", Capacity = 8, AreaId = "A002", IsActive = true, Status = TableStatus.Available },
                new Table { Id = "T011-STATIC-ID-GUID-000000000011", TableId = "T011", TableCode = "T011", TableName = "Bàn B3", Capacity = 6, AreaId = "A002", IsActive = true, Status = TableStatus.Available },

                // Tables for Area A003 (VIP)
                new Table { Id = "T006-STATIC-ID-GUID-000000000006", TableId = "T006", TableCode = "T006", TableName = "Bàn VIP 1", Capacity = 6, AreaId = "A003", IsActive = true, Status = TableStatus.Available },
                new Table { Id = "T007-STATIC-ID-GUID-000000000007", TableId = "T007", TableCode = "T007", TableName = "Bàn VIP 2", Capacity = 10, AreaId = "A003", IsActive = true, Status = TableStatus.Available },
                new Table { Id = "T012-STATIC-ID-GUID-000000000012", TableId = "T012", TableCode = "T012", TableName = "Bàn VIP 3", Capacity = 12, AreaId = "A003", IsActive = true, Status = TableStatus.Available },

                // Tables for Area A004 (Tầng 2 - Thường)
                new Table { Id = "T008-STATIC-ID-GUID-000000000008", TableId = "T008", TableCode = "T008", TableName = "Bàn T2-1", Capacity = 4, AreaId = "A004", IsActive = true, Status = TableStatus.Available },
                new Table { Id = "T009-STATIC-ID-GUID-000000000009", TableId = "T009", TableCode = "T009", TableName = "Bàn T2-2", Capacity = 6, AreaId = "A004", IsActive = true, Status = TableStatus.Available },
                new Table { Id = "T013-STATIC-ID-GUID-000000000013", TableId = "T013", TableCode = "T013", TableName = "Bàn T2-3", Capacity = 4, AreaId = "A004", IsActive = true, Status = TableStatus.Available },

                // Tables for Area A006 (Khu C)
                new Table { Id = "T014-STATIC-ID-GUID-000000000014", TableId = "T014", TableCode = "T014", TableName = "Bàn C1", Capacity = 2, AreaId = "A006", IsActive = true, Status = TableStatus.Available },
                new Table { Id = "T015-STATIC-ID-GUID-000000000015", TableId = "T015", TableCode = "T015", TableName = "Bàn C2", Capacity = 4, AreaId = "A006", IsActive = true, Status = TableStatus.Available },

                // Tables for Area A007 (Private Room)
                new Table { Id = "T016-STATIC-ID-GUID-000000000016", TableId = "T016", TableCode = "T016", TableName = "Phòng Riêng 1", Capacity = 15, AreaId = "A007", IsActive = true, Status = TableStatus.Available },
                new Table { Id = "T017-STATIC-ID-GUID-000000000017", TableId = "T017", TableCode = "T017", TableName = "Phòng Riêng 2", Capacity = 20, AreaId = "A007", IsActive = true, Status = TableStatus.Available }
            );

            // Seed Table Sessions with static IDs
            modelBuilder.Entity<TableSession>().HasData(
                new TableSession { Id = "TS001-STATIC-ID-GUID-00000000001", SessionId = "TS001", TableId = "T001", OpenAt = new DateTime(2025, 1, 15, 12, 0, 0), OpenedBy = "Staff001", Status = SessionStatus.Available },
                new TableSession { Id = "TS002-STATIC-ID-GUID-00000000002", SessionId = "TS002", TableId = "T006", OpenAt = new DateTime(2025, 1, 15, 13, 0, 0), CloseAt = new DateTime(2025, 1, 15, 15, 30, 0), OpenedBy = "Staff001", ClosedBy = "Staff002", Status = SessionStatus.Closed },
                new TableSession { Id = "TS003-STATIC-ID-GUID-00000000003", SessionId = "TS003", TableId = "T011", OpenAt = new DateTime(2025, 1, 15, 18, 0, 0), OpenedBy = "Staff003", Status = SessionStatus.Available },
                new TableSession { Id = "TS004-STATIC-ID-GUID-00000000004", SessionId = "TS004", TableId = "T014", OpenAt = new DateTime(2025, 1, 15, 14, 30, 0), CloseAt = new DateTime(2025, 1, 15, 16, 0, 0), OpenedBy = "Staff002", ClosedBy = "Staff001", Status = SessionStatus.Closed },
                new TableSession { Id = "TS005-STATIC-ID-GUID-00000000005", SessionId = "TS005", TableId = "T016", OpenAt = new DateTime(2025, 1, 15, 19, 0, 0), OpenedBy = "Staff004", Status = SessionStatus.Available }
            );


            // Seed Orders with static IDs
            modelBuilder.Entity<Order>().HasData(
                new Order { Id = "ORD001-STATIC-ID-GUID-0000000001", OrderId = "ORD001", CreatedAt = new DateTime(2025, 1, 15, 12, 30, 0), PrimaryAreaId = "A001", IsPaid = false, OrderStatus = OrderStatus.Open },
                new Order { Id = "ORD002-STATIC-ID-GUID-0000000002", OrderId = "ORD002", CreatedAt = new DateTime(2025, 1, 15, 13, 15, 0), ClosedAt = new DateTime(2025, 1, 15, 15, 30, 0), PrimaryAreaId = "A003", IsPaid = true, OrderStatus = OrderStatus.Paid },
                new Order { Id = "ORD003-STATIC-ID-GUID-0000000003", OrderId = "ORD003", CreatedAt = new DateTime(2025, 1, 15, 14, 0, 0), PrimaryAreaId = "A002", IsPaid = false, OrderStatus = OrderStatus.Open },
                new Order { Id = "ORD004-STATIC-ID-GUID-0000000004", OrderId = "ORD004", CreatedAt = new DateTime(2025, 1, 15, 18, 15, 0), PrimaryAreaId = "A002", IsPaid = false, OrderStatus = OrderStatus.Open },
                new Order { Id = "ORD005-STATIC-ID-GUID-0000000005", OrderId = "ORD005", CreatedAt = new DateTime(2025, 1, 15, 19, 30, 0), PrimaryAreaId = "A007", IsPaid = false, OrderStatus = OrderStatus.Open },
                new Order { Id = "ORD006-STATIC-ID-GUID-0000000006", OrderId = "ORD006", CreatedAt = new DateTime(2025, 1, 14, 20, 0, 0), ClosedAt = new DateTime(2025, 1, 14, 22, 30, 0), PrimaryAreaId = "A003", IsPaid = true, OrderStatus = OrderStatus.Paid }
            );

            // Seed Order Tables with static IDs
            modelBuilder.Entity<OrderTable>().HasData(
                new OrderTable { Id = "OT001-STATIC-ID-GUID-0000000001", OrderId = "ORD001", TableId = "T001", IsPrimary = true, FromTime = new DateTime(2025, 1, 15, 12, 30, 0) },
                new OrderTable { Id = "OT002-STATIC-ID-GUID-0000000002", OrderId = "ORD002", TableId = "T006", IsPrimary = true, FromTime = new DateTime(2025, 1, 15, 13, 15, 0), ToTime = new DateTime(2025, 1, 15, 15, 30, 0) },
                new OrderTable { Id = "OT003-STATIC-ID-GUID-0000000003", OrderId = "ORD003", TableId = "T004", IsPrimary = true, FromTime = new DateTime(2025, 1, 15, 14, 0, 0) },
                new OrderTable { Id = "OT004-STATIC-ID-GUID-0000000004", OrderId = "ORD004", TableId = "T011", IsPrimary = true, FromTime = new DateTime(2025, 1, 15, 18, 15, 0) },
                new OrderTable { Id = "OT005-STATIC-ID-GUID-0000000005", OrderId = "ORD005", TableId = "T016", IsPrimary = true, FromTime = new DateTime(2025, 1, 15, 19, 30, 0) },
                new OrderTable { Id = "OT006-STATIC-ID-GUID-0000000006", OrderId = "ORD006", TableId = "T012", IsPrimary = true, FromTime = new DateTime(2025, 1, 14, 20, 0, 0), ToTime = new DateTime(2025, 1, 14, 22, 30, 0) }
            );

            // Seed OrderDetails with static IDs
            modelBuilder.Entity<OrderDetail>().HasData(
                // Order 1 (ORD001) - Family lunch
                new OrderDetail { Id = "OD001-STATIC-ID-GUID-0000000001", OrderDetailId = "OD001", OrderId = "ORD001", DishId = "D001", Quantity = 2, UnitPrice = 350000, TableId = "T001", AreaId = "A001", PriceSource = PriceSource.Base },
                new OrderDetail { Id = "OD002-STATIC-ID-GUID-0000000002", OrderDetailId = "OD002", OrderId = "ORD001", DishId = "D004", Quantity = 1, UnitPrice = 75000, TableId = "T001", AreaId = "A001", PriceSource = PriceSource.Base },
                new OrderDetail { Id = "OD015-STATIC-ID-GUID-0000000015", OrderDetailId = "OD015", OrderId = "ORD001", DishId = "D015", Quantity = 3, UnitPrice = 25000, TableId = "T001", AreaId = "A001", PriceSource = PriceSource.Base },

                // Order 2 (ORD002) - VIP dinner (completed)
                new OrderDetail { Id = "OD003-STATIC-ID-GUID-0000000003", OrderDetailId = "OD003", OrderId = "ORD002", DishId = "D001", Quantity = 1, UnitPrice = 402500, TableId = "T006", AreaId = "A003", PriceSource = PriceSource.Custom },
                new OrderDetail { Id = "OD004-STATIC-ID-GUID-0000000004", OrderDetailId = "OD004", OrderId = "ORD002", DishId = "D014", Quantity = 2, UnitPrice = 93500, TableId = "T006", AreaId = "A003", PriceSource = PriceSource.Custom },
                new OrderDetail { Id = "OD016-STATIC-ID-GUID-0000000016", OrderDetailId = "OD016", OrderId = "ORD002", DishId = "D035", Quantity = 1, UnitPrice = 650000, TableId = "T006", AreaId = "A003", PriceSource = PriceSource.Base },

                // Order 3 (ORD003) - Casual dinner
                new OrderDetail { Id = "OD005-STATIC-ID-GUID-0000000005", OrderDetailId = "OD005", OrderId = "ORD003", DishId = "D008", Quantity = 1, UnitPrice = 185250, TableId = "T004", AreaId = "A002", PriceSource = PriceSource.Custom },
                new OrderDetail { Id = "OD006-STATIC-ID-GUID-0000000006", OrderDetailId = "OD006", OrderId = "ORD003", DishId = "D015", Quantity = 3, UnitPrice = 25000, TableId = "T004", AreaId = "A002", PriceSource = PriceSource.Base },
                new OrderDetail { Id = "OD017-STATIC-ID-GUID-0000000017", OrderDetailId = "OD017", OrderId = "ORD003", DishId = "D023", Quantity = 2, UnitPrice = 95000, TableId = "T004", AreaId = "A002", PriceSource = PriceSource.Base },

                // Order 4 (ORD004) - Evening BBQ
                new OrderDetail { Id = "OD007-STATIC-ID-GUID-0000000007", OrderDetailId = "OD007", OrderId = "ORD004", DishId = "D027", Quantity = 2, UnitPrice = 210000, TableId = "T011", AreaId = "A002", PriceSource = PriceSource.Base },
                new OrderDetail { Id = "OD008-STATIC-ID-GUID-0000000008", OrderDetailId = "OD008", OrderId = "ORD004", DishId = "D032", Quantity = 1, UnitPrice = 280000, TableId = "T011", AreaId = "A002", PriceSource = PriceSource.Base },
                new OrderDetail { Id = "OD018-STATIC-ID-GUID-0000000018", OrderDetailId = "OD018", OrderId = "ORD004", DishId = "D030", Quantity = 4, UnitPrice = 45000, TableId = "T011", AreaId = "A002", PriceSource = PriceSource.Base },

                // Order 5 (ORD005) - Private room party
                new OrderDetail { Id = "OD009-STATIC-ID-GUID-0000000009", OrderDetailId = "OD009", OrderId = "ORD005", DishId = "D021", Quantity = 5, UnitPrice = 380000, TableId = "T016", AreaId = "A007", PriceSource = PriceSource.Base },
                new OrderDetail { Id = "OD010-STATIC-ID-GUID-0000000010", OrderDetailId = "OD010", OrderId = "ORD005", DishId = "D024", Quantity = 3, UnitPrice = 320000, TableId = "T016", AreaId = "A007", PriceSource = PriceSource.Base },
                new OrderDetail { Id = "OD019-STATIC-ID-GUID-0000000019", OrderDetailId = "OD019", OrderId = "ORD005", DishId = "D031", Quantity = 8, UnitPrice = 95000, TableId = "T016", AreaId = "A007", PriceSource = PriceSource.Base },

                // Order 6 (ORD006) - Yesterday's VIP completed order
                new OrderDetail { Id = "OD011-STATIC-ID-GUID-0000000011", OrderDetailId = "OD011", OrderId = "ORD006", DishId = "D036", Quantity = 2, UnitPrice = 420000, TableId = "T012", AreaId = "A003", PriceSource = PriceSource.Base },
                new OrderDetail { Id = "OD012-STATIC-ID-GUID-0000000012", OrderDetailId = "OD012", OrderId = "ORD006", DishId = "D037", Quantity = 1, UnitPrice = 380000, TableId = "T012", AreaId = "A003", PriceSource = PriceSource.Base },
                new OrderDetail { Id = "OD013-STATIC-ID-GUID-0000000013", OrderDetailId = "OD013", OrderId = "ORD006", DishId = "D011", Quantity = 4, UnitPrice = 71500, TableId = "T012", AreaId = "A003", PriceSource = PriceSource.Custom },
                new OrderDetail { Id = "OD014-STATIC-ID-GUID-0000000014", OrderDetailId = "OD014", OrderId = "ORD006", DishId = "D014", Quantity = 3, UnitPrice = 93500, TableId = "T012", AreaId = "A003", PriceSource = PriceSource.Custom }
            );

            // Enhanced AreaDishPrices with static IDs
            modelBuilder.Entity<AreaDishPrices>().HasData(
                // Khu A001 - Standard pricing
                new AreaDishPrices { Id = "ADP001-STATIC-ID-GUID-000000001", AreaId = "A001", DishId = "D001", CustomPrice = 350000, EffectiveDate = fixedDate, IsActive = true },
                new AreaDishPrices { Id = "ADP002-STATIC-ID-GUID-000000002", AreaId = "A001", DishId = "D004", CustomPrice = 75000, EffectiveDate = fixedDate, IsActive = true },
                new AreaDishPrices { Id = "ADP003-STATIC-ID-GUID-000000003", AreaId = "A001", DishId = "D011", CustomPrice = 65000, EffectiveDate = fixedDate, IsActive = true },
                new AreaDishPrices { Id = "ADP004-STATIC-ID-GUID-000000004", AreaId = "A001", DishId = "D014", CustomPrice = 85000, EffectiveDate = fixedDate, IsActive = true },
                new AreaDishPrices { Id = "ADP005-STATIC-ID-GUID-000000005", AreaId = "A001", DishId = "D015", CustomPrice = 25000, EffectiveDate = fixedDate, IsActive = true },

                // Khu A002 - Promotional pricing (-5%)
                new AreaDishPrices { Id = "ADP006-STATIC-ID-GUID-000000006", AreaId = "A002", DishId = "D008", CustomPrice = 185250, EffectiveDate = fixedDate, IsActive = true },
                new AreaDishPrices { Id = "ADP007-STATIC-ID-GUID-000000007", AreaId = "A002", DishId = "D009", CustomPrice = 156750, EffectiveDate = fixedDate, IsActive = true },
                new AreaDishPrices { Id = "ADP008-STATIC-ID-GUID-000000008", AreaId = "A002", DishId = "D010", CustomPrice = 137750, EffectiveDate = fixedDate, IsActive = true },
                new AreaDishPrices { Id = "ADP009-STATIC-ID-GUID-000000009", AreaId = "A002", DishId = "D027", CustomPrice = 199500, EffectiveDate = fixedDate, IsActive = true },

                // Khu A003 - VIP pricing (+15% for main dishes, +10% for desserts)
                new AreaDishPrices { Id = "ADP010-STATIC-ID-GUID-000000010", AreaId = "A003", DishId = "D001", CustomPrice = 402500, EffectiveDate = fixedDate, IsActive = true },
                new AreaDishPrices { Id = "ADP011-STATIC-ID-GUID-000000011", AreaId = "A003", DishId = "D002", CustomPrice = 207000, EffectiveDate = fixedDate, IsActive = true },
                new AreaDishPrices { Id = "ADP012-STATIC-ID-GUID-000000012", AreaId = "A003", DishId = "D003", CustomPrice = 322000, EffectiveDate = fixedDate, IsActive = true },
                new AreaDishPrices { Id = "ADP013-STATIC-ID-GUID-000000013", AreaId = "A003", DishId = "D007", CustomPrice = 287500, EffectiveDate = fixedDate, IsActive = true },
                new AreaDishPrices { Id = "ADP014-STATIC-ID-GUID-000000014", AreaId = "A003", DishId = "D021", CustomPrice = 437000, EffectiveDate = fixedDate, IsActive = true },
                new AreaDishPrices { Id = "ADP015-STATIC-ID-GUID-000000015", AreaId = "A003", DishId = "D011", CustomPrice = 71500, EffectiveDate = fixedDate, IsActive = true },
                new AreaDishPrices { Id = "ADP016-STATIC-ID-GUID-000000016", AreaId = "A003", DishId = "D012", CustomPrice = 82500, EffectiveDate = fixedDate, IsActive = true },
                new AreaDishPrices { Id = "ADP017-STATIC-ID-GUID-000000017", AreaId = "A003", DishId = "D014", CustomPrice = 93500, EffectiveDate = fixedDate, IsActive = true },

                // Khu A004 - Standard pricing
                new AreaDishPrices { Id = "ADP018-STATIC-ID-GUID-000000018", AreaId = "A004", DishId = "D004", CustomPrice = 75000, EffectiveDate = fixedDate, IsActive = true },
                new AreaDishPrices { Id = "ADP019-STATIC-ID-GUID-000000019", AreaId = "A004", DishId = "D005", CustomPrice = 65000, EffectiveDate = fixedDate, IsActive = true },
                new AreaDishPrices { Id = "ADP020-STATIC-ID-GUID-000000020", AreaId = "A004", DishId = "D006", CustomPrice = 85000, EffectiveDate = fixedDate, IsActive = true },
                new AreaDishPrices { Id = "ADP021-STATIC-ID-GUID-000000021", AreaId = "A004", DishId = "D025", CustomPrice = 135000, EffectiveDate = fixedDate, IsActive = true },

                // Khu A006 - Quiet zone, slight discount (-3%)
                new AreaDishPrices { Id = "ADP022-STATIC-ID-GUID-000000022", AreaId = "A006", DishId = "D015", CustomPrice = 24250, EffectiveDate = fixedDate, IsActive = true },
                new AreaDishPrices { Id = "ADP023-STATIC-ID-GUID-000000023", AreaId = "A006", DishId = "D030", CustomPrice = 43650, EffectiveDate = fixedDate, IsActive = true },
                new AreaDishPrices { Id = "ADP024-STATIC-ID-GUID-000000024", AreaId = "A006", DishId = "D022", CustomPrice = 160050, EffectiveDate = fixedDate, IsActive = true },

                // Khu A007 - Private room premium (+20%)
                new AreaDishPrices { Id = "ADP025-STATIC-ID-GUID-000000025", AreaId = "A007", DishId = "D021", CustomPrice = 456000, EffectiveDate = fixedDate, IsActive = true },
                new AreaDishPrices { Id = "ADP026-STATIC-ID-GUID-000000026", AreaId = "A007", DishId = "D024", CustomPrice = 384000, EffectiveDate = fixedDate, IsActive = true },
                new AreaDishPrices { Id = "ADP027-STATIC-ID-GUID-000000027", AreaId = "A007", DishId = "D035", CustomPrice = 780000, EffectiveDate = fixedDate, IsActive = true },
                new AreaDishPrices { Id = "ADP028-STATIC-ID-GUID-000000028", AreaId = "A007", DishId = "D031", CustomPrice = 114000, EffectiveDate = fixedDate, IsActive = true }
            );
        }
    }
}
