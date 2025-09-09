using Microsoft.EntityFrameworkCore;
using Restaurant.Data;
using Restaurant.Service.Interfaces;
using Restaurant.Service.Services;
using System.Text.Json.Serialization;
using static Restaurant.Service.Services.AreasService;
using static Restaurant.Service.Services.TableService;

var builder = WebApplication.CreateBuilder(args);

// ===== CORS: khai báo policy =====
const string AllowAngularDev = "AllowAngularDev";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: AllowAngularDev, policy =>
    {
        policy.WithOrigins(
                "http://localhost:4200"         // Angular dev server (mặc định HTTP)
                                                // ,"https://localhost:4200"     // Nếu bạn chạy Angular dev bằng HTTPS, mở thêm
            )
            .AllowAnyHeader()
            .AllowAnyMethod();
        // .AllowCredentials();             // BẬT nếu bạn thật sự cần cookie/Authorization qua CORS
    });
});
// ==================================

builder.Services.AddDbContext<RestaurantDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configure JSON options for better Angular compatibility
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IAreaDishPriceService, AreaDishPriceService>();
builder.Services.AddScoped<IAreasService, AreasService>();
builder.Services.AddScoped<IDishesService, DishesService>();
builder.Services.AddScoped<IDishesGroupService, DishGroupService>();
builder.Services.AddScoped<IKitchensService, KitchensService>();
builder.Services.AddScoped<ITableService, TableService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IOrderDetailService, OrderDetailService>();
builder.Services.AddScoped<IOrderTableService, OrderTableService>();
builder.Services.AddScoped<ITableSessionService, TableSessionService>();
builder.Services.AddScoped<TableDapperService>();
builder.Services.AddScoped<AreasDapperService>();


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<RestaurantDbContext>();

}

app.UseHttpsRedirection();

app.UseCors(AllowAngularDev); // Đặt TRƯỚC UseAuthorization & MapControllers


app.UseAuthorization();




app.MapControllers();



////set up để public qua ngrok
//app.UseCors("AllowAll");
//app.UseDefaultFiles();
//app.UseStaticFiles();


app.Run();
