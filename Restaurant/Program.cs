

using Microsoft.EntityFrameworkCore;
using Restaurant.Data;
using Restaurant.Service.Interfaces;
using Restaurant.Service.Services;
using static Restaurant.Data.RestaurantDbContext;
var builder = WebApplication.CreateBuilder(args);

// Add DbContext
builder.Services.AddDbContext<RestaurantDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IAreaDishPriceService, AreaDishPriceService>();
builder.Services.AddScoped<IAreasService, AreasService>();
builder.Services.AddScoped<IDishesService,DishesService>();
builder.Services.AddScoped<IKitchensService, KitchensService>();
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
app.UseAuthorization();
app.MapControllers();

app.Run();
