using BLL.Mapper;
using BLL.Service;
using DAL;
using DAL.Entities;
using DAL.Reposistories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly);
builder.Services.AddScoped<IGenericRepository<Account>, GenericRepo<Account>>();
builder.Services.AddScoped<IGenericRepository<Product>, GenericRepo<Product>>();
builder.Services.AddScoped<IGenericRepository<Cart>, GenericRepo<Cart>>();
builder.Services.AddScoped<IGenericRepository<CartItem>, GenericRepo<CartItem>>();
builder.Services.AddScoped<CartService>();
builder.Services.AddScoped<ProductService>();
builder.Services.AddScoped<EmailService>();
// You would also need to configure your DbContext here if you're using Entity Framework 
builder.Services.AddScoped<AccountService>();
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<DataDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins", policy =>
    {
        policy.WithOrigins(
                "http://localhost:5173", // Cho môi trường phát triển
                "https://exe-201-gameproduct-2.vercel.app" // Thay bằng domain thật của frontend sau khi deploy
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials(); // Nếu bạn dùng cookie hoặc token
    });
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseHttpsRedirection();

app.UseCors("AllowSpecificOrigins");

app.UseAuthorization();

app.MapControllers();

app.Run();
