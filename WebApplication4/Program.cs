using BLL.Mapper;
using BLL.Service;
using DAL;
using DAL.Entities;
using DAL.Reposistories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// --- START SwaggerGen Configuration with JWT Authorization ---
builder.Services.AddSwaggerGen(option =>
{
    option.SwaggerDoc("v1", new OpenApiInfo { Title = "Your API Name", Version = "v1" });

    // 1. Define the Security Scheme (Bearer Token)
    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header, 
        Description = "Please enter a valid token", 
        Name = "Authorization", 
        Type = SecuritySchemeType.Http, 
        BearerFormat = "JWT", 
        Scheme = "Bearer" 
    });
    // 2. Add a security requirement that uses the defined scheme
    // This makes the "Authorize" button appear and adds lock icons to protected endpoints.
    option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer" // Refers to the "Bearer" scheme defined above
                }
            },
            new string[] { } // Scopes required for this security scheme (empty for standard JWT)
        }
    });
});

//----- JWT hanlde ------
var jwtSettings = builder.Configuration.GetSection("Jwt");
var secretKey = jwtSettings["Key"];
var issuer = jwtSettings["Issuer"];
var audience = jwtSettings["Audience"];


builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,

        ValidIssuer = issuer,
        ValidAudience = audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),

        // Optional: ClockSkew to allow for minor time differences between servers
        // Set to TimeSpan.Zero for strict validation.
        ClockSkew = TimeSpan.Zero
    };
});





builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly);
builder.Services.AddScoped<IGenericRepository<Account>, GenericRepo<Account>>();
builder.Services.AddScoped<IGenericRepository<Product>, GenericRepo<Product>>();
builder.Services.AddScoped<IGenericRepository<Cart>, GenericRepo<Cart>>();
builder.Services.AddScoped<IGenericRepository<CartItem>, GenericRepo<CartItem>>();
builder.Services.AddScoped<IGenericRepository<Order>, GenericRepo<Order>>();
builder.Services.AddScoped<IGenericRepository<Feedback>, GenericRepo<Feedback>>();  
builder.Services.AddScoped<FeedbackService>();
builder.Services.AddScoped<OrderService>();
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

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
