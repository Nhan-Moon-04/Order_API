using Microsoft.EntityFrameworkCore;
using Restaurant.Domain.Entities;

namespace Restaurant.Data
{
    public class RestaurantDbContext : DbContext
    {
        public RestaurantDbContext(DbContextOptions<RestaurantDbContext> options)
            : base(options) { }

        public DbSet<Areas> Areas { get; set; }
        public DbSet<Kitchens> Kitchens { get; set; }
        public DbSet<Dishes> Dishes { get; set; }
        public DbSet<AreaDishPrices> AreaDishPrices { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure entity relationships
            ConfigureRelationships(modelBuilder);

            var fixedDate = new DateTime(2025, 01, 01); // ngày cố định

            // Seed Areas
            modelBuilder.Entity<Areas>().HasData(
                new Areas 
                { 
                    Id = "AREA001", 
                    AreaId = "A001", 
                    AreaName = "Tầng Trệt - Khu A", 
                    Description = "Khu vực tầng trệt, phù hợp cho gia đình", 
                    IsActive = true, 
                    CreatedAt = fixedDate 
                },
                new Areas 
                { 
                    Id = "AREA002", 
                    AreaId = "A002", 
                    AreaName = "Tầng Trệt - Khu B", 
                    Description = "Khu vực tầng trệt, gần cửa sổ", 
                    IsActive = true, 
                    CreatedAt = fixedDate 
                },
                new Areas 
                { 
                    Id = "AREA003", 
                    AreaId = "A003", 
                    AreaName = "Tầng 2 - Khu VIP", 
                    Description = "Khu vực VIP tầng 2, view đẹp", 
                    IsActive = true, 
                    CreatedAt = fixedDate 
                },
                new Areas 
                { 
                    Id = "AREA004", 
                    AreaId = "A004", 
                    AreaName = "Tầng 2 - Khu Thường", 
                    Description = "Khu vực thường tầng 2", 
                    IsActive = true, 
                    CreatedAt = fixedDate 
                },
                new Areas 
                { 
                    Id = "AREA005", 
                    AreaId = "A005", 
                    AreaName = "Sân Thượng", 
                    Description = "Khu vực sân thượng, không gian mở", 
                    IsActive = false, 
                    CreatedAt = fixedDate 
                }
            );

            // Seed Kitchens
            modelBuilder.Entity<Kitchens>().HasData(
                new Kitchens 
                { 
                    Id = "KITCHEN001", 
                    KitchenId = "K001", 
                    KitchenName = "Bếp Âu", 
                    Description = "Chuyên món Âu, Mỹ, Ý", 
                    IsActive = true, 
                    CreatedAt = fixedDate 
                },
                new Kitchens 
                { 
                    Id = "KITCHEN002", 
                    KitchenId = "K002", 
                    KitchenName = "Bếp Á", 
                    Description = "Chuyên món Việt, Trung, Nhật, Hàn", 
                    IsActive = true, 
                    CreatedAt = fixedDate 
                },
                new Kitchens 
                { 
                    Id = "KITCHEN003", 
                    KitchenId = "K003", 
                    KitchenName = "Bếp Nướng BBQ", 
                    Description = "Chuyên các món nướng, BBQ", 
                    IsActive = true, 
                    CreatedAt = fixedDate 
                },
                new Kitchens 
                { 
                    Id = "KITCHEN004", 
                    KitchenId = "K004", 
                    KitchenName = "Bếp Dessert", 
                    Description = "Chuyên tráng miệng, đồ ngọt", 
                    IsActive = true, 
                    CreatedAt = fixedDate 
                },
                new Kitchens 
                { 
                    Id = "KITCHEN005", 
                    KitchenId = "K005", 
                    KitchenName = "Bar & Thức Uống", 
                    Description = "Chuyên pha chế đồ uống, cocktail", 
                    IsActive = true, 
                    CreatedAt = fixedDate 
                }
            );

            // Seed Dishes
            modelBuilder.Entity<Dishes>().HasData(
                // Món Âu
                new Dishes 
                { 
                    Id = "DISH001", 
                    DishId = "D001", 
                    DishName = "Steak Bò Mỹ", 
                    Price = (double)350000, 
                    KitchenId = "KITCHEN001", 
                    IsActive = true, 
                    CreatedAt = fixedDate 
                },
                new Dishes 
                { 
                    Id = "DISH002", 
                    DishId = "D002", 
                    DishName = "Pasta Carbonara", 
                    Price = (double)180000, 
                    KitchenId = "KITCHEN001", 
                    IsActive = true, 
                    CreatedAt = fixedDate 
                },
                new Dishes 
                { 
                    Id = "DISH003", 
                    DishId = "D003", 
                    DishName = "Salmon Teriyaki", 
                    Price = (double)280000, 
                    KitchenId = "KITCHEN001", 
                    IsActive = true, 
                    CreatedAt = fixedDate 
                },
                
                // Món Á
                new Dishes 
                { 
                    Id = "DISH004", 
                    DishId = "D004", 
                    DishName = "Phở Bò Đặc Biệt", 
                    Price = (double)75000, 
                    KitchenId = "KITCHEN002", 
                    IsActive = true, 
                    CreatedAt = fixedDate 
                },
                new Dishes 
                { 
                    Id = "DISH005", 
                    DishId = "D005", 
                    DishName = "Bún Chả Hà Nội", 
                    Price = (double)65000, 
                    KitchenId = "KITCHEN002", 
                    IsActive = true, 
                    CreatedAt = fixedDate 
                },
                new Dishes 
                { 
                    Id = "DISH006", 
                    DishId = "D006", 
                    DishName = "Cơm Gà Hải Nam", 
                    Price = (double)85000, 
                    KitchenId = "KITCHEN002", 
                    IsActive = true, 
                    CreatedAt = fixedDate 
                },
                new Dishes 
                { 
                    Id = "DISH007", 
                    DishId = "D007", 
                    DishName = "Lẩu Thái Chua Cay", 
                    Price = (double)250000, 
                    KitchenId = "KITCHEN002", 
                    IsActive = true, 
                    CreatedAt = fixedDate 
                },
                
                // Món Nướng BBQ
                new Dishes 
                { 
                    Id = "DISH008", 
                    DishId = "D008", 
                    DishName = "Sườn Nướng BBQ", 
                    Price = (double)195000, 
                    KitchenId = "KITCHEN003", 
                    IsActive = true, 
                    CreatedAt = fixedDate 
                },
                new Dishes 
                { 
                    Id = "DISH009", 
                    DishId = "D009", 
                    DishName = "Gà Nướng Mật Ong", 
                    Price = (double)165000, 
                    KitchenId = "KITCHEN003", 
                    IsActive = true, 
                    CreatedAt = fixedDate 
                },
                new Dishes 
                { 
                    Id = "DISH010", 
                    DishId = "D010", 
                    DishName = "Bò Nướng Lá Lốt", 
                    Price = (double)145000, 
                    KitchenId = "KITCHEN003", 
                    IsActive = true, 
                    CreatedAt = fixedDate 
                },
                
                // Dessert
                new Dishes 
                { 
                    Id = "DISH011", 
                    DishId = "D011", 
                    DishName = "Tiramisu", 
                    Price = (double)65000, 
                    KitchenId = "KITCHEN004", 
                    IsActive = true, 
                    CreatedAt = fixedDate 
                },
                new Dishes 
                { 
                    Id = "DISH012", 
                    DishId = "D012", 
                    DishName = "Chocolate Lava Cake", 
                    Price = (double)75000, 
                    KitchenId = "KITCHEN004", 
                    IsActive = true, 
                    CreatedAt = fixedDate 
                },
                new Dishes 
                { 
                    Id = "DISH013", 
                    DishId = "D013", 
                    DishName = "Kem Vanilla Pháp", 
                    Price = (double)45000, 
                    KitchenId = "KITCHEN004", 
                    IsActive = true, 
                    CreatedAt = fixedDate 
                },
                
                // Thức uống
                new Dishes 
                { 
                    Id = "DISH014", 
                    DishId = "D014", 
                    DishName = "Mojito Classic", 
                    Price = (double)85000, 
                    KitchenId = "KITCHEN005", 
                    IsActive = true, 
                    CreatedAt = fixedDate 
                },
                new Dishes 
                { 
                    Id = "DISH015", 
                    DishId = "D015", 
                    DishName = "Cà Phê Sữa Đá", 
                    Price = (double)25000, 
                    KitchenId = "KITCHEN005", 
                    IsActive = true, 
                    CreatedAt = fixedDate 
                },
                new Dishes 
                { 
                    Id = "DISH016", 
                    DishId = "D016", 
                    DishName = "Sinh Tố Bơ", 
                    Price = (double)35000, 
                    KitchenId = "KITCHEN005", 
                    IsActive = true, 
                    CreatedAt = fixedDate 
                }
            );

            // Seed AreaDishPrices
            modelBuilder.Entity<AreaDishPrices>().HasData(
                // Khu A - Tầng trệt (giá chuẩn)
                new AreaDishPrices 
                { 
                    Id = "ADP001", 
                    AreaId = "AREA001", 
                    DishId = "DISH001", 
                    CustomPrice = (double)350000, 
                    EffectiveDate = fixedDate, 
                    IsActive = true 
                },
                new AreaDishPrices 
                { 
                    Id = "ADP002", 
                    AreaId = "AREA001", 
                    DishId = "DISH004", 
                    CustomPrice = (double)75000, 
                    EffectiveDate = fixedDate, 
                    IsActive = true 
                },
                
                // Khu VIP - Tầng 2 (giá cao hơn 15%)
                new AreaDishPrices 
                { 
                    Id = "ADP003", 
                    AreaId = "AREA003", 
                    DishId = "DISH001", 
                    CustomPrice = (double)402500, // +15%
                    EffectiveDate = fixedDate, 
                    IsActive = true 
                },
                new AreaDishPrices 
                { 
                    Id = "ADP004", 
                    AreaId = "AREA003", 
                    DishId = "DISH002", 
                    CustomPrice = (double)207000, // +15%
                    EffectiveDate = fixedDate, 
                    IsActive = true 
                },
                new AreaDishPrices 
                { 
                    Id = "ADP005", 
                    AreaId = "AREA003", 
                    DishId = "DISH003", 
                    CustomPrice = (double)322000, // +15%
                    EffectiveDate = fixedDate, 
                    IsActive = true 
                },
                new AreaDishPrices 
                { 
                    Id = "ADP006", 
                    AreaId = "AREA003", 
                    DishId = "DISH007", 
                    CustomPrice = (double)287500, // +15%
                    EffectiveDate = fixedDate, 
                    IsActive = true 
                },
                
                // Khu B - Tầng trệt (giá giảm 5% để thu hút khách)
                new AreaDishPrices 
                { 
                    Id = "ADP007", 
                    AreaId = "AREA002", 
                    DishId = "DISH008", 
                    CustomPrice = (double)185250, // -5%
                    EffectiveDate = fixedDate, 
                    IsActive = true 
                },
                new AreaDishPrices 
                { 
                    Id = "ADP008", 
                    AreaId = "AREA002", 
                    DishId = "DISH009", 
                    CustomPrice = (double)156750, // -5%
                    EffectiveDate = fixedDate, 
                    IsActive = true 
                },
                new AreaDishPrices 
                { 
                    Id = "ADP009", 
                    AreaId = "AREA002", 
                    DishId = "DISH010", 
                    CustomPrice = (double)137750, // -5%
                    EffectiveDate = fixedDate, 
                    IsActive = true 
                },
                
                // Khu Thường Tầng 2 (giá chuẩn)
                new AreaDishPrices 
                { 
                    Id = "ADP010", 
                    AreaId = "AREA004", 
                    DishId = "DISH004", 
                    CustomPrice = (double)75000, 
                    EffectiveDate = fixedDate, 
                    IsActive = true 
                },
                new AreaDishPrices 
                { 
                    Id = "ADP011", 
                    AreaId = "AREA004", 
                    DishId = "DISH005", 
                    CustomPrice = (double)65000, 
                    EffectiveDate = fixedDate, 
                    IsActive = true 
                },
                new AreaDishPrices 
                { 
                    Id = "ADP012", 
                    AreaId = "AREA004", 
                    DishId = "DISH006", 
                    CustomPrice = (double)85000, 
                    EffectiveDate = fixedDate, 
                    IsActive = true 
                },
                
                // Dessert & Drinks cho tất cả khu vực (giá chuẩn)
                new AreaDishPrices 
                { 
                    Id = "ADP013", 
                    AreaId = "AREA001", 
                    DishId = "DISH011", 
                    CustomPrice = (double)65000, 
                    EffectiveDate = fixedDate, 
                    IsActive = true 
                },
                new AreaDishPrices 
                { 
                    Id = "ADP014", 
                    AreaId = "AREA001", 
                    DishId = "DISH014", 
                    CustomPrice = (double)85000, 
                    EffectiveDate = fixedDate, 
                    IsActive = true 
                },
                new AreaDishPrices 
                { 
                    Id = "ADP015", 
                    AreaId = "AREA001", 
                    DishId = "DISH015", 
                    CustomPrice = (double)25000, 
                    EffectiveDate = fixedDate, 
                    IsActive = true 
                },
                
                // VIP Dessert (giá cao hơn 10%)
                new AreaDishPrices 
                { 
                    Id = "ADP016", 
                    AreaId = "AREA003", 
                    DishId = "DISH011", 
                    CustomPrice = (double)71500, // +10%
                    EffectiveDate = fixedDate, 
                    IsActive = true 
                },
                new AreaDishPrices 
                { 
                    Id = "ADP017", 
                    AreaId = "AREA003", 
                    DishId = "DISH012", 
                    CustomPrice = (double)82500, // +10%
                    EffectiveDate = fixedDate, 
                    IsActive = true 
                },
                new AreaDishPrices 
                { 
                    Id = "ADP018", 
                    AreaId = "AREA003", 
                    DishId = "DISH014", 
                    CustomPrice = (double)93500, // +10%
                    EffectiveDate = fixedDate, 
                    IsActive = true 
                }
            );
        }

        private void ConfigureRelationships(ModelBuilder modelBuilder)
        {
            // Configure Kitchens -> Dishes relationship (One-to-Many)
            modelBuilder.Entity<Dishes>()
                .HasOne(d => d.Kitchen)
                .WithMany(k => k.Dishes)
                .HasForeignKey(d => d.KitchenId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();

            // Configure Areas -> AreaDishPrices relationship (One-to-Many)
            modelBuilder.Entity<AreaDishPrices>()
                .HasOne(adp => adp.Area)
                .WithMany(a => a.AreaDishPrices)
                .HasForeignKey(adp => adp.AreaId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();

            // Configure Dishes -> AreaDishPrices relationship (One-to-Many)
            modelBuilder.Entity<AreaDishPrices>()
                .HasOne(adp => adp.Dish)
                .WithMany(d => d.AreaDishPrices)
                .HasForeignKey(adp => adp.DishId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();

            // Configure primary keys (if needed for custom configuration)
            modelBuilder.Entity<Areas>()
                .HasKey(a => a.Id);

            modelBuilder.Entity<Kitchens>()
                .HasKey(k => k.Id);

            modelBuilder.Entity<Dishes>()
                .HasKey(d => d.Id);

            modelBuilder.Entity<AreaDishPrices>()
                .HasKey(adp => adp.Id);

            // Configure indexes for better performance
            modelBuilder.Entity<Areas>()
                .HasIndex(a => a.AreaId)
                .IsUnique();
            modelBuilder.Entity<Kitchens>()
                .HasIndex(k => k.KitchenId)
                .IsUnique();

            modelBuilder.Entity<Dishes>()
                .HasIndex(d => d.DishId)
                .IsUnique();

            // Configure composite index for AreaDishPrices
            modelBuilder.Entity<AreaDishPrices>()
                .HasIndex(adp => new { adp.AreaId, adp.DishId, adp.EffectiveDate })
                .HasDatabaseName("IX_AreaDishPrices_AreaId_DishId_EffectiveDate");
        }
    }
}
