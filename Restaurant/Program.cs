using Microsoft.EntityFrameworkCore;
using Restaurant.Data;
using Restaurant.Service.Interfaces;
using Restaurant.Service.Services;

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

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IAreaDishPriceService, AreaDishPriceService>();
builder.Services.AddScoped<IAreasService, AreasService>();
builder.Services.AddScoped<IDishesService, DishesService>();
builder.Services.AddScoped<IKitchensService, KitchensService>();
builder.Services.AddScoped<ITableService, TableService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IOrderDetailService, OrderDetailService>();

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

// ===== CORS: đặt đúng thứ tự trong pipeline =====
app.UseCors(AllowAngularDev); // Đặt TRƯỚC UseAuthorization & MapControllers


app.UseAuthorization();

app.MapControllers();

app.Run();
