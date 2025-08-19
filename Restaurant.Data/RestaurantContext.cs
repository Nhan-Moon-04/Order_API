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

            var fixedDate = new DateTime(2025, 01, 01); // ngày cố định

            // Seed Areas
            modelBuilder.Entity<Areas>().HasData(
                new Areas 
                { 
                    Id = "bf1d3a14-cec7-4217-afef-3070cc964d2b",
                    AreaId = "A001", 
                    AreaName = "Tầng Trệt - Khu A", 
                    Description = "Khu vực tầng trệt, phù hợp cho gia đình", 
                    IsActive = true, 
                    CreatedAt = fixedDate 
                },
                new Areas 
                { 
                    Id = "87cce1fb-d6f9-49a2-b026-15bb0740385c",
                    AreaId = "A002", 
                    AreaName = "Tầng Trệt - Khu B", 
                    Description = "Khu vực tầng trệt, gần cửa sổ", 
                    IsActive = true, 
                    CreatedAt = fixedDate 
                },
                new Areas 
                { 
                    Id = "a57ca4df-4212-4271-ab4e-3c2761987e0b",
                    AreaId = "A003", 
                    AreaName = "Tầng 2 - Khu VIP", 
                    Description = "Khu vực VIP tầng 2, view đẹp", 
                    IsActive = true, 
                    CreatedAt = fixedDate 
                },
                new Areas 
                { 
                    Id = "e3da7474-bab3-48de-b4bd-7887170d7e59",
                    AreaId = "A004", 
                    AreaName = "Tầng 2 - Khu Thường", 
                    Description = "Khu vực thường tầng 2", 
                    IsActive = true, 
                    CreatedAt = fixedDate 
                },
                new Areas 
                { 
                    Id = "eb837d6c-079e-44b9-8df5-da3ac31bc03c",
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
                    Id = "2a8d7cbf-f8c0-426f-bebc-da0d67428aad",
                    KitchenId = "K001", 
                    KitchenName = "Bếp Âu", 
                    Description = "Chuyên món Âu, Mỹ, Ý", 
                    IsActive = true, 
                    CreatedAt = fixedDate 
                },
                new Kitchens 
                { 
                    Id = "ed39caf8-325b-4670-a339-9a0b17983250",
                    KitchenId = "K002", 
                    KitchenName = "Bếp Á", 
                    Description = "Chuyên món Việt, Trung, Nhật, Hàn", 
                    IsActive = true, 
                    CreatedAt = fixedDate 
                },
                new Kitchens 
                { 
                    Id = "dcabb860-de38-4cb8-a190-8ef255deba01",
                    KitchenId = "K003", 
                    KitchenName = "Bếp Nướng BBQ", 
                    Description = "Chuyên các món nướng, BBQ", 
                    IsActive = true, 
                    CreatedAt = fixedDate 
                },
                new Kitchens 
                { 
                    Id = "f6729fb5-bf42-4a1c-9e5e-845f7964f71a",
                    KitchenId = "K004", 
                    KitchenName = "Bếp Dessert", 
                    Description = "Chuyên tráng miệng, đồ ngọt", 
                    IsActive = true, 
                    CreatedAt = fixedDate 
                },
                new Kitchens 
                { 
                    Id = "8820e289-12fc-4434-820e-440479c9b462",
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
                    Id = "7a1e24dc-ca9e-4bf5-aa2d-c0036c4043fa",
                    DishId = "D001", 
                    DishName = "Steak Bò Mỹ", 
                    Price = (double)350000, 
                    KitchenId = "2a8d7cbf-f8c0-426f-bebc-da0d67428aad", 
                    IsActive = true, 
                    CreatedAt = fixedDate 
                },
                new Dishes 
                { 
                    Id = "5667d134-2d25-4a16-a5d1-157308227a17",
                    DishId = "D002", 
                    DishName = "Pasta Carbonara", 
                    Price = (double)180000, 
                    KitchenId = "2a8d7cbf-f8c0-426f-bebc-da0d67428aad", 
                    IsActive = true, 
                    CreatedAt = fixedDate 
                },
                new Dishes 
                { 
                    Id = "eba3c6e2-a1c1-43ab-9d2a-33c13b335e65",
                    DishId = "D003", 
                    DishName = "Salmon Teriyaki", 
                    Price = (double)280000, 
                    KitchenId = "2a8d7cbf-f8c0-426f-bebc-da0d67428aad", 
                    IsActive = true, 
                    CreatedAt = fixedDate 
                },
                
                // Món Á
                new Dishes 
                { 
                    Id = "d9ccfb52-47f7-47be-b8fe-f5eacac402b1",
                    DishId = "D004", 
                    DishName = "Phở Bò Đặc Biệt", 
                    Price = (double)75000, 
                    KitchenId = "ed39caf8-325b-4670-a339-9a0b17983250", 
                    IsActive = true, 
                    CreatedAt = fixedDate 
                },
                new Dishes 
                { 
                    Id = "cff053a4-a929-40c7-ad66-dd4c3b8a6d65",
                    DishId = "D005", 
                    DishName = "Bún Chả Hà Nội", 
                    Price = (double)65000, 
                    KitchenId = "ed39caf8-325b-4670-a339-9a0b17983250", 
                    IsActive = true, 
                    CreatedAt = fixedDate 
                },
                new Dishes 
                { 
                    Id = "5c19bd27-58d1-4b05-9a68-cbf20ababb2c",
                    DishId = "D006", 
                    DishName = "Cơm Gà Hải Nam", 
                    Price = (double)85000, 
                    KitchenId = "ed39caf8-325b-4670-a339-9a0b17983250", 
                    IsActive = true, 
                    CreatedAt = fixedDate 
                },
                new Dishes 
                { 
                    Id = "f40ac9ad-43af-4a54-86fe-f47ff0999b34",
                    DishId = "D007", 
                    DishName = "Lẩu Thái Chua Cay", 
                    Price = (double)250000, 
                    KitchenId = "ed39caf8-325b-4670-a339-9a0b17983250", 
                    IsActive = true, 
                    CreatedAt = fixedDate 
                },
                
                // Món Nướng BBQ
                new Dishes 
                { 
                    Id = "6d2a44ca-6e63-472f-9d80-9cecd5ba8d04",
                    DishId = "D008", 
                    DishName = "Sườn Nướng BBQ", 
                    Price = (double)195000, 
                    KitchenId = "dcabb860-de38-4cb8-a190-8ef255deba01", 
                    IsActive = true, 
                    CreatedAt = fixedDate 
                },
                new Dishes 
                { 
                    Id = "bfee412d-3269-45b4-b044-d57d1da9d9f0",
                    DishId = "D009", 
                    DishName = "Gà Nướng Mật Ong", 
                    Price = (double)165000, 
                    KitchenId = "dcabb860-de38-4cb8-a190-8ef255deba01", 
                    IsActive = true, 
                    CreatedAt = fixedDate 
                },
                new Dishes 
                { 
                    Id = "1a3c182a-9605-44e1-ae27-df362a15494c",
                    DishId = "D010", 
                    DishName = "Bò Nướng Lá Lốt", 
                    Price = (double)145000, 
                    KitchenId = "dcabb860-de38-4cb8-a190-8ef255deba01", 
                    IsActive = true, 
                    CreatedAt = fixedDate 
                },
                
                // Dessert
                new Dishes 
                { 
                    Id = "c6dc0e86-f5d0-45fc-b4de-d7299c581a33",
                    DishId = "D011", 
                    DishName = "Tiramisu", 
                    Price = (double)65000, 
                    KitchenId = "f6729fb5-bf42-4a1c-9e5e-845f7964f71a", 
                    IsActive = true, 
                    CreatedAt = fixedDate 
                },
                new Dishes 
                { 
                    Id = "021d17c2-fc6a-459a-af8d-e38eeac64faa",
                    DishId = "D012", 
                    DishName = "Chocolate Lava Cake", 
                    Price = (double)75000, 
                    KitchenId = "f6729fb5-bf42-4a1c-9e5e-845f7964f71a", 
                    IsActive = true, 
                    CreatedAt = fixedDate 
                },
                new Dishes 
                { 
                    Id = "f985a4f6-43d3-4545-a967-7aba4556a108",
                    DishId = "D013", 
                    DishName = "Kem Vanilla Pháp", 
                    Price = (double)45000, 
                    KitchenId = "f6729fb5-bf42-4a1c-9e5e-845f7964f71a", 
                    IsActive = true, 
                    CreatedAt = fixedDate 
                },
                
                // Thức uống
                new Dishes 
                { 
                    Id = "3d1559a8-bb69-4c03-b341-6df3035d5341",
                    DishId = "D014", 
                    DishName = "Mojito Classic", 
                    Price = (double)85000, 
                    KitchenId = "8820e289-12fc-4434-820e-440479c9b462", 
                    IsActive = true, 
                    CreatedAt = fixedDate 
                },
                new Dishes 
                { 
                    Id = "de57ba84-10b9-4ec3-84a7-7296917dd829",
                    DishId = "D015", 
                    DishName = "Cà Phê Sữa Đá", 
                    Price = (double)25000, 
                    KitchenId = "8820e289-12fc-4434-820e-440479c9b462", 
                    IsActive = true, 
                    CreatedAt = fixedDate 
                },
                new Dishes 
                { 
                    Id = "1f86ddb0-3b7e-441f-9628-3bc7b2a7de9e",
                    DishId = "D016", 
                    DishName = "Sinh Tố Bơ", 
                    Price = (double)35000, 
                    KitchenId = "8820e289-12fc-4434-820e-440479c9b462", 
                    IsActive = true, 
                    CreatedAt = fixedDate 
                }
            );

            // Seed AreaDishPrices
            modelBuilder.Entity<AreaDishPrices>().HasData(
                // Khu A - Tầng trệt (giá chuẩn)
                new AreaDishPrices 
                { 
                    Id = "61945c0d-deff-4057-9e8a-f721a5c0cb7b",
                    AreaId = "bf1d3a14-cec7-4217-afef-3070cc964d2b", 
                    DishId = "7a1e24dc-ca9e-4bf5-aa2d-c0036c4043fa", 
                    CustomPrice = (double)350000, 
                    EffectiveDate = fixedDate, 
                    IsActive = true 
                },
                new AreaDishPrices 
                { 
                    Id = "774bac0c-974a-402e-bf86-55112d3f7b32",
                    AreaId = "bf1d3a14-cec7-4217-afef-3070cc964d2b", 
                    DishId = "d9ccfb52-47f7-47be-b8fe-f5eacac402b1", 
                    CustomPrice = (double)75000, 
                    EffectiveDate = fixedDate, 
                    IsActive = true 
                },
                
                // Khu VIP - Tầng 2 (giá cao hơn 15%)
                new AreaDishPrices 
                { 
                    Id = "b9e21b5c-a1b9-40a2-a37c-2d54645772fd",
                    AreaId = "a57ca4df-4212-4271-ab4e-3c2761987e0b", 
                    DishId = "7a1e24dc-ca9e-4bf5-aa2d-c0036c4043fa", 
                    CustomPrice = (double)402500, // +15%
                    EffectiveDate = fixedDate, 
                    IsActive = true 
                },
                new AreaDishPrices 
                { 
                    Id = "4326b2e6-74f3-4fb7-98ce-d93c603bb3ae",
                    AreaId = "a57ca4df-4212-4271-ab4e-3c2761987e0b", 
                    DishId = "5667d134-2d25-4a16-a5d1-157308227a17", 
                    CustomPrice = (double)207000, // +15%
                    EffectiveDate = fixedDate, 
                    IsActive = true 
                },
                new AreaDishPrices 
                { 
                    Id = "88f46e3a-5ee1-4afc-b006-dda8a8ba613f",
                    AreaId = "a57ca4df-4212-4271-ab4e-3c2761987e0b", 
                    DishId = "eba3c6e2-a1c1-43ab-9d2a-33c13b335e65", 
                    CustomPrice = (double)322000, // +15%
                    EffectiveDate = fixedDate, 
                    IsActive = true 
                },
                new AreaDishPrices 
                { 
                    Id = "e589b937-ce64-4a17-b37c-c3824ed536fa",
                    AreaId = "a57ca4df-4212-4271-ab4e-3c2761987e0b", 
                    DishId = "f40ac9ad-43af-4a54-86fe-f47ff0999b34", 
                    CustomPrice = (double)287500, // +15%
                    EffectiveDate = fixedDate, 
                    IsActive = true 
                },
                
                // Khu B - Tầng trệt (giá giảm 5% để thu hút khách)
                new AreaDishPrices 
                { 
                    Id = "933b82d7-80b3-412a-9095-641178588217",
                    AreaId = "87cce1fb-d6f9-49a2-b026-15bb0740385c", 
                    DishId = "6d2a44ca-6e63-472f-9d80-9cecd5ba8d04", 
                    CustomPrice = (double)185250, // -5%
                    EffectiveDate = fixedDate, 
                    IsActive = true 
                },
                new AreaDishPrices 
                { 
                    Id = "5ebea2d1-85b2-444f-ad62-1333c379d38d",
                    AreaId = "87cce1fb-d6f9-49a2-b026-15bb0740385c", 
                    DishId = "bfee412d-3269-45b4-b044-d57d1da9d9f0", 
                    CustomPrice = (double)156750, // -5%
                    EffectiveDate = fixedDate, 
                    IsActive = true 
                },
                new AreaDishPrices 
                { 
                    Id = "ab2daf7d-8b87-4c85-83be-caed26fdb0cd",
                    AreaId = "87cce1fb-d6f9-49a2-b026-15bb0740385c", 
                    DishId = "1a3c182a-9605-44e1-ae27-df362a15494c", 
                    CustomPrice = (double)137750, // -5%
                    EffectiveDate = fixedDate, 
                    IsActive = true 
                },
                
                // Khu Thường Tầng 2 (giá chuẩn)
                new AreaDishPrices 
                { 
                    Id = "4780ada7-3329-4d75-9865-b01ad347348d",
                    AreaId = "e3da7474-bab3-48de-b4bd-7887170d7e59", 
                    DishId = "d9ccfb52-47f7-47be-b8fe-f5eacac402b1", 
                    CustomPrice = (double)75000, 
                    EffectiveDate = fixedDate, 
                    IsActive = true 
                },
                new AreaDishPrices 
                { 
                    Id = "91dcdba4-88f7-4c1a-b030-fda2240684fd",
                    AreaId = "e3da7474-bab3-48de-b4bd-7887170d7e59", 
                    DishId = "cff053a4-a929-40c7-ad66-dd4c3b8a6d65", 
                    CustomPrice = (double)65000, 
                    EffectiveDate = fixedDate, 
                    IsActive = true 
                },
                new AreaDishPrices 
                { 
                    Id = "5f7ad5cd-1aef-4152-a8d8-223aec2c5f52",
                    AreaId = "e3da7474-bab3-48de-b4bd-7887170d7e59", 
                    DishId = "5c19bd27-58d1-4b05-9a68-cbf20ababb2c", 
                    CustomPrice = (double)85000, 
                    EffectiveDate = fixedDate, 
                    IsActive = true 
                },
                
                // Dessert & Drinks cho tất cả khu vực (giá chuẩn)
                new AreaDishPrices 
                { 
                    Id = "aa4d00b3-42ab-4af8-9d04-c75a0ee08593",
                    AreaId = "bf1d3a14-cec7-4217-afef-3070cc964d2b", 
                    DishId = "c6dc0e86-f5d0-45fc-b4de-d7299c581a33", 
                    CustomPrice = (double)65000, 
                    EffectiveDate = fixedDate, 
                    IsActive = true 
                },
                new AreaDishPrices 
                { 
                    Id = "45eafca6-f793-48de-8f0d-a19154392783",
                    AreaId = "bf1d3a14-cec7-4217-afef-3070cc964d2b", 
                    DishId = "3d1559a8-bb69-4c03-b341-6df3035d5341", 
                    CustomPrice = (double)85000, 
                    EffectiveDate = fixedDate, 
                    IsActive = true 
                },
                new AreaDishPrices 
                { 
                    Id = "6ba71674-fa01-4ee0-abe8-40a9b707a47b",
                    AreaId = "bf1d3a14-cec7-4217-afef-3070cc964d2b", 
                    DishId = "de57ba84-10b9-4ec3-84a7-7296917dd829", 
                    CustomPrice = (double)25000, 
                    EffectiveDate = fixedDate, 
                    IsActive = true 
                },
                
                // VIP Dessert (giá cao hơn 10%)
                new AreaDishPrices 
                { 
                    Id = "ca3797b3-d380-42a1-a25e-8a265d9e3988",
                    AreaId = "a57ca4df-4212-4271-ab4e-3c2761987e0b", 
                    DishId = "c6dc0e86-f5d0-45fc-b4de-d7299c581a33", 
                    CustomPrice = (double)71500, // +10%
                    EffectiveDate = fixedDate, 
                    IsActive = true 
                },
                new AreaDishPrices 
                { 
                    Id = "31b9a1c7-2e62-4532-b11f-197644b732b7",
                    AreaId = "a57ca4df-4212-4271-ab4e-3c2761987e0b", 
                    DishId = "021d17c2-fc6a-459a-af8d-e38eeac64faa", 
                    CustomPrice = (double)82500, // +10%
                    EffectiveDate = fixedDate, 
                    IsActive = true 
                },
                new AreaDishPrices 
                { 
                    Id = "36745970-4738-442d-81ac-7c757068102b",
                    AreaId = "a57ca4df-4212-4271-ab4e-3c2761987e0b", 
                    DishId = "3d1559a8-bb69-4c03-b341-6df3035d5341", 
                    CustomPrice = (double)93500, // +10%
                    EffectiveDate = fixedDate, 
                    IsActive = true 
                }
            );
        }
    }
}
