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
        public DbSet<Table> Tables { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            var fixedDate = new DateTime(2025, 01, 01); // ngày cố định

            // Configure unique constraints for business codes
            modelBuilder.Entity<Areas>()
                .HasAlternateKey(a => a.AreaId);

            modelBuilder.Entity<Table>()
                .HasAlternateKey(t => t.TableCode);

            modelBuilder.Entity<Order>()
                .HasAlternateKey(o => o.OrderId);

            modelBuilder.Entity<Dishes>()
                .HasAlternateKey(d => d.DishId);

            // Configure foreign key relationships
            modelBuilder.Entity<Table>()
                .HasOne(t => t.Area)
                .WithMany(a => a.Tables)
                .HasForeignKey(t => t.AreaId)
                .HasPrincipalKey(a => a.AreaId);

            modelBuilder.Entity<Order>()
                .HasOne(o => o.Table)
                .WithMany(t => t.Orders)
                .HasForeignKey(o => o.TableCode)
                .HasPrincipalKey(t => t.TableCode);

            // Configure OrderDetail relationship to use OrderId business code, not GUID Id
            modelBuilder.Entity<OrderDetail>()
                .HasOne(od => od.Order)
                .WithMany(o => o.OrderDetails)
                .HasForeignKey(od => od.OrderId)
                .HasPrincipalKey(o => o.OrderId);

            // Configure OrderDetail relationship to use DishId business code, not GUID Id  
            modelBuilder.Entity<OrderDetail>()
                .HasOne(od => od.Dish)
                .WithMany(d => d.OrderDetails)
                .HasForeignKey(od => od.DishId)
                .HasPrincipalKey(d => d.DishId);

            // Seed Areas - Using static GUIDs from migration
            modelBuilder.Entity<Areas>().HasData(
                new Areas 
                { 
                    Id = "8cd11c10-ad0b-4e03-ad52-45ef1df68233",
                    AreaId = "A001", 
                    AreaName = "Tầng Trệt - Khu A", 
                    Description = "Khu vực tầng trệt, phù hợp cho gia đình", 
                    IsActive = true, 
                    CreatedAt = fixedDate 
                },
                new Areas 
                { 
                    Id = "1cbade01-c4d5-4480-8495-a4088c6acdb9",
                    AreaId = "A002", 
                    AreaName = "Tầng Trệt - Khu B", 
                    Description = "Khu vực tầng trệt, gần cửa sổ", 
                    IsActive = true, 
                    CreatedAt = fixedDate 
                },
                new Areas 
                { 
                    Id = "658b8200-3165-4f87-9127-4f88c1e910bf",
                    AreaId = "A003", 
                    AreaName = "Tầng 2 - Khu VIP", 
                    Description = "Khu vực VIP tầng 2, view đẹp", 
                    IsActive = true, 
                    CreatedAt = fixedDate 
                },
                new Areas 
                { 
                    Id = "ff8e4878-df25-47c6-bff2-81e2ef25f6ec",
                    AreaId = "A004", 
                    AreaName = "Tầng 2 - Khu Thường", 
                    Description = "Khu vực thường tầng 2", 
                    IsActive = true, 
                    CreatedAt = fixedDate 
                },
                new Areas 
                { 
                    Id = "6d904071-1b94-4869-9fc4-990419fe89bf",
                    AreaId = "A005", 
                    AreaName = "Sân Thượng", 
                    Description = "Khu vực sân thượng, không gian mở", 
                    IsActive = false, 
                    CreatedAt = fixedDate 
                }
            );

            // Seed Kitchens - Using static GUIDs from migration
            modelBuilder.Entity<Kitchens>().HasData(
                new Kitchens 
                { 
                    Id = "ffd4d425-2b9a-4da8-afc1-25b0a3ff32ca",
                    KitchenId = "K001", 
                    KitchenName = "Bếp Âu", 
                    Description = "Chuyên món Âu, Mỹ, Ý", 
                    IsActive = true, 
                    CreatedAt = fixedDate 
                },
                new Kitchens 
                { 
                    Id = "934a1a27-2191-463f-8706-aa8857f8f414",
                    KitchenId = "K002", 
                    KitchenName = "Bếp Á", 
                    Description = "Chuyên món Việt, Trung, Nhật, Hàn", 
                    IsActive = true, 
                    CreatedAt = fixedDate 
                },
                new Kitchens 
                { 
                    Id = "cbee4395-3e11-483e-97c8-358e061874b5",
                    KitchenId = "K003", 
                    KitchenName = "Bếp Nướng BBQ", 
                    Description = "Chuyên các món nướng, BBQ", 
                    IsActive = true, 
                    CreatedAt = fixedDate 
                },
                new Kitchens 
                { 
                    Id = "894ea134-a20a-4bb6-ac52-6bd8f371e12e",
                    KitchenId = "K004", 
                    KitchenName = "Bếp Dessert", 
                    Description = "Chuyên tráng miệng, đồ ngọt", 
                    IsActive = true, 
                    CreatedAt = fixedDate 
                },
                new Kitchens 
                { 
                    Id = "24754043-632a-47ed-8249-452c4608fa6b",
                    KitchenId = "K005", 
                    KitchenName = "Bar & Thức Uống", 
                    Description = "Chuyên pha chế đồ uống, cocktail", 
                    IsActive = true, 
                    CreatedAt = fixedDate 
                }
            );

            // Seed Dishes - Using static GUIDs from migration
            modelBuilder.Entity<Dishes>().HasData(
                // Món Âu
                new Dishes 
                { 
                    Id = "40dace6f-dabd-444d-9378-c98967a3c183",
                    DishId = "D001", 
                    DishName = "Steak Bò Mỹ", 
                    BasePrice = (double)350000, 
                    KitchenId = "ffd4d425-2b9a-4da8-afc1-25b0a3ff32ca", 
                    IsActive = true, 
                    CreatedAt = fixedDate 
                },
                new Dishes 
                { 
                    Id = "fa885c4c-5a2d-4824-8757-405703cdc128",
                    DishId = "D002", 
                    DishName = "Pasta Carbonara", 
                    BasePrice = (double)180000, 
                    KitchenId = "ffd4d425-2b9a-4da8-afc1-25b0a3ff32ca", 
                    IsActive = true, 
                    CreatedAt = fixedDate 
                },
                new Dishes 
                { 
                    Id = "0bf3e2fa-0059-40bd-aaba-2856a604a758",
                    DishId = "D003", 
                    DishName = "Salmon Teriyaki", 
                    BasePrice = (double)280000, 
                    KitchenId = "ffd4d425-2b9a-4da8-afc1-25b0a3ff32ca", 
                    IsActive = true, 
                    CreatedAt = fixedDate 
                },
                
                // Món Á
                new Dishes 
                { 
                    Id = "699b952b-1629-4f8f-9fc0-beb98db2ddbf",
                    DishId = "D004", 
                    DishName = "Phở Bò Đặc Biệt", 
                    BasePrice = (double)75000, 
                    KitchenId = "934a1a27-2191-463f-8706-aa8857f8f414", 
                    IsActive = true, 
                    CreatedAt = fixedDate 
                },
                new Dishes 
                { 
                    Id = "ac336386-ae8d-4902-90e2-6d33e09d0a6c",
                    DishId = "D005", 
                    DishName = "Bún Chả Hà Nội", 
                    BasePrice = (double)65000, 
                    KitchenId = "934a1a27-2191-463f-8706-aa8857f8f414", 
                    IsActive = true, 
                    CreatedAt = fixedDate 
                },
                new Dishes 
                { 
                    Id = "6e3e4f58-30fa-41a4-9aac-ada1f7f607ed",
                    DishId = "D006", 
                    DishName = "Cơm Gà Hải Nam", 
                    BasePrice = (double)85000, 
                    KitchenId = "934a1a27-2191-463f-8706-aa8857f8f414", 
                    IsActive = true, 
                    CreatedAt = fixedDate 
                },
                new Dishes 
                { 
                    Id = "f6700740-1e8b-4169-aace-5f04b6813cc1",
                    DishId = "D007", 
                    DishName = "Lẩu Thái Chua Cay", 
                    BasePrice = (double)250000, 
                    KitchenId = "934a1a27-2191-463f-8706-aa8857f8f414", 
                    IsActive = true, 
                    CreatedAt = fixedDate 
                },
                
                // Món Nướng BBQ
                new Dishes 
                { 
                    Id = "e67c5639-c603-47f8-815b-35748a532702",
                    DishId = "D008", 
                    DishName = "Sườn Nướng BBQ", 
                    BasePrice = (double)195000, 
                    KitchenId = "cbee4395-3e11-483e-97c8-358e061874b5", 
                    IsActive = true, 
                    CreatedAt = fixedDate 
                },
                new Dishes 
                { 
                    Id = "8d000849-4121-44b3-9777-3057c1513170",
                    DishId = "D009", 
                    DishName = "Gà Nướng Mật Ong", 
                    BasePrice = (double)165000, 
                    KitchenId = "cbee4395-3e11-483e-97c8-358e061874b5", 
                    IsActive = true, 
                    CreatedAt = fixedDate 
                },
                new Dishes 
                { 
                    Id = "741804b4-2ae4-4919-858f-4a99bd68fb96",
                    DishId = "D010", 
                    DishName = "Bò Nướng Lá Lốt", 
                    BasePrice = (double)145000, 
                    KitchenId = "cbee4395-3e11-483e-97c8-358e061874b5", 
                    IsActive = true, 
                    CreatedAt = fixedDate 
                },
                
                // Dessert
                new Dishes 
                { 
                    Id = "8d12a359-fc7c-45a6-b58a-e9d2bc7bff3b",
                    DishId = "D011", 
                    DishName = "Tiramisu", 
                    BasePrice = (double)65000, 
                    KitchenId = "894ea134-a20a-4bb6-ac52-6bd8f371e12e", 
                    IsActive = true, 
                    CreatedAt = fixedDate 
                },
                new Dishes 
                { 
                    Id = "75e98b88-6198-48aa-9ecd-af3e57d9d2a9",
                    DishId = "D012", 
                    DishName = "Chocolate Lava Cake", 
                    BasePrice = (double)75000, 
                    KitchenId = "894ea134-a20a-4bb6-ac52-6bd8f371e12e", 
                    IsActive = true, 
                    CreatedAt = fixedDate 
                },
                new Dishes 
                { 
                    Id = "86c8cd1b-fde6-4408-a03c-ba28b417f1c6",
                    DishId = "D013", 
                    DishName = "Kem Vanilla Pháp", 
                    BasePrice = (double)45000, 
                    KitchenId = "894ea134-a20a-4bb6-ac52-6bd8f371e12e", 
                    IsActive = true, 
                    CreatedAt = fixedDate 
                },
                
                // Thức uống
                new Dishes 
                { 
                    Id = "4a60fcdf-c9fd-49d3-99de-35ccdf9cba41",
                    DishId = "D014", 
                    DishName = "Mojito Classic", 
                    BasePrice = (double)85000, 
                    KitchenId = "24754043-632a-47ed-8249-452c4608fa6b", 
                    IsActive = true, 
                    CreatedAt = fixedDate 
                },
                new Dishes 
                { 
                    Id = "81630539-2abb-4452-8fd9-37ea67259144",
                    DishId = "D015", 
                    DishName = "Cà Phê Sữa Đá", 
                    BasePrice = (double)25000, 
                    KitchenId = "24754043-632a-47ed-8249-452c4608fa6b", 
                    IsActive = true, 
                    CreatedAt = fixedDate 
                },
                new Dishes 
                { 
                    Id = "4b593b0f-6dd5-41ac-83a6-739953f43e33",
                    DishId = "D016", 
                    DishName = "Sinh Tố Bơ", 
                    BasePrice = (double)35000, 
                    KitchenId = "24754043-632a-47ed-8249-452c4608fa6b", 
                    IsActive = true, 
                    CreatedAt = fixedDate 
                }
            );

            // Seed Tables - Using static GUIDs from migration
            modelBuilder.Entity<Table>().HasData(
                // Tables for Area A001
                new Table 
                { 
                    Id = "6b22e489-0cee-4650-802e-85655c5deae5",
                    TableCode = "T001", 
                    TableName = "Bàn A1", 
                    Capacity = 4, 
                    AreaId = "A001", 
                    IsActive = true 
                },
                new Table 
                { 
                    Id = "6e77a75d-8655-4075-9332-e9e1c0b8a2d3",
                    TableCode = "T002", 
                    TableName = "Bàn A2", 
                    Capacity = 6, 
                    AreaId = "A001", 
                    IsActive = true 
                },
                new Table 
                { 
                    Id = "7edc5369-8967-40a7-909f-13f51ad9995c",
                    TableCode = "T003", 
                    TableName = "Bàn A3", 
                    Capacity = 2, 
                    AreaId = "A001", 
                    IsActive = true 
                },
                
                // Tables for Area A002
                new Table 
                { 
                    Id = "d545a871-cf12-4c7c-9433-bbc747aa7ce9",
                    TableCode = "T004", 
                    TableName = "Bàn B1", 
                    Capacity = 4, 
                    AreaId = "A002", 
                    IsActive = true 
                },
                new Table 
                { 
                    Id = "164e54f1-c092-416e-a887-892d7e7f70c9",
                    TableCode = "T005", 
                    TableName = "Bàn B2", 
                    Capacity = 8, 
                    AreaId = "A002", 
                    IsActive = true 
                },
                
                // Tables for Area A003 (VIP)
                new Table 
                { 
                    Id = "0d1d76e6-c625-4966-beb1-ffbaac5eb6bd",
                    TableCode = "T006", 
                    TableName = "Bàn VIP 1", 
                    Capacity = 6, 
                    AreaId = "A003", 
                    IsActive = true 
                },
                new Table 
                { 
                    Id = "861e63e3-b81d-444d-b7aa-d98ee06b99bb",
                    TableCode = "T007", 
                    TableName = "Bàn VIP 2", 
                    Capacity = 10, 
                    AreaId = "A003", 
                    IsActive = true 
                },
                
                // Tables for Area A004
                new Table 
                { 
                    Id = "86850bb5-7e3f-48ef-9fb8-dfc00184f36e",
                    TableCode = "T008", 
                    TableName = "Bàn T2-1", 
                    Capacity = 4, 
                    AreaId = "A004", 
                    IsActive = true 
                },
                new Table 
                { 
                    Id = "18ef20f5-7d24-4bfd-98a2-7e5e243f7d18",
                    TableCode = "T009", 
                    TableName = "Bàn T2-2", 
                    Capacity = 6, 
                    AreaId = "A004", 
                    IsActive = true 
                }
            );

            // Seed Orders - Using static GUIDs from migration
            modelBuilder.Entity<Order>().HasData(
                new Order 
                { 
                    Id = "d0945bce-518c-4f47-8c6b-94c70c86b93a",
                    OrderId = "ORD001", 
                    OrderDate = new DateTime(2025, 1, 15, 12, 30, 0), 
                    TableCode = "T001", 
                    IsPaid = false 
                },
                new Order 
                { 
                    Id = "88d6e5a7-d47c-4bcf-aac4-5fbb01d02e23",
                    OrderId = "ORD002", 
                    OrderDate = new DateTime(2025, 1, 15, 13, 15, 0), 
                    TableCode = "T006", 
                    IsPaid = true 
                },
                new Order 
                { 
                    Id = "f832f1d1-a77f-4355-a9ed-ad4b31668ab1",
                    OrderId = "ORD003", 
                    OrderDate = new DateTime(2025, 1, 15, 14, 0, 0), 
                    TableCode = "T004", 
                    IsPaid = false 
                }
            );

            // Seed OrderDetails - Now using business codes for OrderId and DishId
            modelBuilder.Entity<OrderDetail>().HasData(
                // Order 1 details
                new OrderDetail 
                { 
                    Id = "de0156cc-dffd-4f6b-a925-e767367d35b5",
                    OrderDetailId = "OD001", 
                    OrderId = "ORD001", 
                    DishId = "D001", 
                    Quantity = 2, 
                    UnitPrice = 350000 
                },
                new OrderDetail 
                { 
                    Id = "fc9ddb44-8efb-471d-8b76-58c913b065c0",
                    OrderDetailId = "OD002", 
                    OrderId = "ORD001", 
                    DishId = "D004", 
                    Quantity = 1, 
                    UnitPrice = 75000 
                },
                
                // Order 2 details (VIP pricing)
                new OrderDetail 
                { 
                    Id = "951deff1-28b2-4529-b5e9-c1f1ae424bfc",
                    OrderDetailId = "OD003", 
                    OrderId = "ORD002", 
                    DishId = "D001", 
                    Quantity = 1, 
                    UnitPrice = 402500 
                },
                new OrderDetail 
                { 
                    Id = "73f80a8d-1363-4416-bd68-e5a0cd7836c6",
                    OrderDetailId = "OD004", 
                    OrderId = "ORD002", 
                    DishId = "D014", 
                    Quantity = 2, 
                    UnitPrice = 93500 
                },
                
                // Order 3 details
                new OrderDetail 
                { 
                    Id = "491ef346-f99f-48bd-8be4-00b7378d5ea6",
                    OrderDetailId = "OD005", 
                    OrderId = "ORD003", 
                    DishId = "D008", 
                    Quantity = 1, 
                    UnitPrice = 185250 
                },
                new OrderDetail 
                { 
                    Id = "d940d964-e021-4363-a783-f8a675626b53",
                    OrderDetailId = "OD006", 
                    OrderId = "ORD003", 
                    DishId = "D015", 
                    Quantity = 3, 
                    UnitPrice = 25000 
                }
            );

            // Seed AreaDishPrices - Using static GUIDs from migration
            modelBuilder.Entity<AreaDishPrices>().HasData(
                // Khu A - Tầng trệt (giá chuẩn)
                new AreaDishPrices 
                { 
                    Id = "0922a426-69f2-4a42-8241-50273afe04c3",
                    AreaId = "8cd11c10-ad0b-4e03-ad52-45ef1df68233", 
                    DishId = "40dace6f-dabd-444d-9378-c98967a3c183", 
                    CustomPrice = (double)350000, 
                    EffectiveDate = fixedDate, 
                    IsActive = true 
                },
                new AreaDishPrices 
                { 
                    Id = "d4f578fb-41e0-4193-baf2-1cd8cbf26bad",
                    AreaId = "8cd11c10-ad0b-4e03-ad52-45ef1df68233", 
                    DishId = "699b952b-1629-4f8f-9fc0-beb98db2ddbf", 
                    CustomPrice = (double)75000, 
                    EffectiveDate = fixedDate, 
                    IsActive = true 
                },
                
                // Khu VIP - Tầng 2 (giá cao hơn 15%)
                new AreaDishPrices 
                { 
                    Id = "28b1f3df-561c-4f36-94f1-a28beab291f7",
                    AreaId = "658b8200-3165-4f87-9127-4f88c1e910bf", 
                    DishId = "40dace6f-dabd-444d-9378-c98967a3c183", 
                    CustomPrice = (double)402500, // +15%
                    EffectiveDate = fixedDate, 
                    IsActive = true 
                },
                new AreaDishPrices 
                { 
                    Id = "2926d8f8-8bbe-4981-b4e8-839fc89ff022",
                    AreaId = "658b8200-3165-4f87-9127-4f88c1e910bf", 
                    DishId = "fa885c4c-5a2d-4824-8757-405703cdc128", 
                    CustomPrice = (double)207000, // +15%
                    EffectiveDate = fixedDate, 
                    IsActive = true 
                },
                new AreaDishPrices 
                { 
                    Id = "832ef5c0-6c81-4087-8433-9f08c5319f29",
                    AreaId = "658b8200-3165-4f87-9127-4f88c1e910bf", 
                    DishId = "0bf3e2fa-0059-40bd-aaba-2856a604a758", 
                    CustomPrice = (double)322000, // +15%
                    EffectiveDate = fixedDate, 
                    IsActive = true 
                },
                new AreaDishPrices 
                { 
                    Id = "cc160bdd-b316-4bf0-be6f-309823ae466f",
                    AreaId = "658b8200-3165-4f87-9127-4f88c1e910bf", 
                    DishId = "f6700740-1e8b-4169-aace-5f04b6813cc1", 
                    CustomPrice = (double)287500, // +15%
                    EffectiveDate = fixedDate, 
                    IsActive = true 
                },
                
                // Khu B - Tầng trệt (giá giảm 5% để thu hút khách)
                new AreaDishPrices 
                { 
                    Id = "c0adfafc-58d2-4c95-be90-d98cb24678e0",
                    AreaId = "1cbade01-c4d5-4480-8495-a4088c6acdb9", 
                    DishId = "e67c5639-c603-47f8-815b-35748a532702", 
                    CustomPrice = (double)185250, // -5%
                    EffectiveDate = fixedDate, 
                    IsActive = true 
                },
                new AreaDishPrices 
                { 
                    Id = "a7641cfd-34e2-4cd5-897b-f09c433f4695",
                    AreaId = "1cbade01-c4d5-4480-8495-a4088c6acdb9", 
                    DishId = "8d000849-4121-44b3-9777-3057c1513170", 
                    CustomPrice = (double)156750, // -5%
                    EffectiveDate = fixedDate, 
                    IsActive = true 
                },
                new AreaDishPrices 
                { 
                    Id = "48d13ef0-0072-4918-a2aa-f36cbcededc8",
                    AreaId = "1cbade01-c4d5-4480-8495-a4088c6acdb9", 
                    DishId = "741804b4-2ae4-4919-858f-4a99bd68fb96", 
                    CustomPrice = (double)137750, // -5%
                    EffectiveDate = fixedDate, 
                    IsActive = true 
                },
                
                // Khu Thường Tầng 2 (giá chuẩn)
                new AreaDishPrices 
                { 
                    Id = "f53b1a9c-ea7a-4002-bebf-f059d22c7677",
                    AreaId = "ff8e4878-df25-47c6-bff2-81e2ef25f6ec", 
                    DishId = "699b952b-1629-4f8f-9fc0-beb98db2ddbf", 
                    CustomPrice = (double)75000, 
                    EffectiveDate = fixedDate, 
                    IsActive = true 
                },
                new AreaDishPrices 
                { 
                    Id = "2469db73-9d2d-4a3a-aefd-f60533e95e01",
                    AreaId = "ff8e4878-df25-47c6-bff2-81e2ef25f6ec", 
                    DishId = "ac336386-ae8d-4902-90e2-6d33e09d0a6c", 
                    CustomPrice = (double)65000, 
                    EffectiveDate = fixedDate, 
                    IsActive = true 
                },
                new AreaDishPrices 
                { 
                    Id = "e8809dd1-7e1d-4539-b4b0-3f0d3ffe3292",
                    AreaId = "ff8e4878-df25-47c6-bff2-81e2ef25f6ec", 
                    DishId = "6e3e4f58-30fa-41a4-9aac-ada1f7f607ed", 
                    CustomPrice = (double)85000, 
                    EffectiveDate = fixedDate, 
                    IsActive = true 
                },
                
                // Dessert & Drinks cho tất cả khu vực (giá chuẩn)
                new AreaDishPrices 
                { 
                    Id = "cdbe2a27-7208-4a67-9fe6-cccff1fe805e",
                    AreaId = "8cd11c10-ad0b-4e03-ad52-45ef1df68233", 
                    DishId = "8d12a359-fc7c-45a6-b58a-e9d2bc7bff3b", 
                    CustomPrice = (double)65000, 
                    EffectiveDate = fixedDate, 
                    IsActive = true 
                },
                new AreaDishPrices 
                { 
                    Id = "1cf3d568-a7e0-4717-ac92-8d7768259b76",
                    AreaId = "8cd11c10-ad0b-4e03-ad52-45ef1df68233", 
                    DishId = "4a60fcdf-c9fd-49d3-99de-35ccdf9cba41", 
                    CustomPrice = (double)85000, 
                    EffectiveDate = fixedDate, 
                    IsActive = true 
                },
                new AreaDishPrices 
                { 
                    Id = "441f3e6a-7ab5-4fcf-9aae-5345478cbd9e",
                    AreaId = "8cd11c10-ad0b-4e03-ad52-45ef1df68233", 
                    DishId = "81630539-2abb-4452-8fd9-37ea67259144", 
                    CustomPrice = (double)25000, 
                    EffectiveDate = fixedDate, 
                    IsActive = true 
                },
                
                // VIP Dessert (giá cao hơn 10%)
                new AreaDishPrices 
                { 
                    Id = "63f33e16-2c60-486e-b371-8fa6f8834e96",
                    AreaId = "658b8200-3165-4f87-9127-4f88c1e910bf", 
                    DishId = "8d12a359-fc7c-45a6-b58a-e9d2bc7bff3b", 
                    CustomPrice = (double)71500, // +10%
                    EffectiveDate = fixedDate, 
                    IsActive = true 
                },
                new AreaDishPrices 
                { 
                    Id = "e84d0c9f-51f7-4ab5-b037-01f54a34fcdf",
                    AreaId = "658b8200-3165-4f87-9127-4f88c1e910bf", 
                    DishId = "75e98b88-6198-48aa-9ecd-af3e57d9d2a9", 
                    CustomPrice = (double)82500, // +10%
                    EffectiveDate = fixedDate, 
                    IsActive = true 
                },
                new AreaDishPrices 
                { 
                    Id = "9a012fd5-3e33-4980-b03d-ac56419c6ce5",
                    AreaId = "658b8200-3165-4f87-9127-4f88c1e910bf", 
                    DishId = "4a60fcdf-c9fd-49d3-99de-35ccdf9cba41", 
                    CustomPrice = (double)93500, // +10%
                    EffectiveDate = fixedDate, 
                    IsActive = true 
                }
            );
        }
    }
}
