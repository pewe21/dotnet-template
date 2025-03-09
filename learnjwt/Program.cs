using System.Text;
using learnjwt.AppContext;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<MyDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    options.UseSqlServer(connectionString);
});

var  MyAllowSpecificOrigins = "_myAllowSpecificOrigins"; //ini
builder.Services.AddCors(options => // ini satu block
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
        policy  =>
        {
            policy.WithOrigins("http://localhost:5173"); //ini adalah url frontend
            policy.AllowAnyHeader();
            policy.AllowAnyMethod();
        });
});

var secretKey = builder.Configuration["JwtSettings:Secret"];

if (string.IsNullOrEmpty(secretKey) || secretKey.Length < 32)
{
    throw new InvalidOperationException("JWT Secret Key is missing or too short. It must be at least 32 characters long.");
}

var key = Encoding.UTF8.GetBytes(secretKey);

builder.Services.AddAuthentication(x =>
    {
        x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {   
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
            ValidAudience = builder.Configuration["JwtSettings:Audience"],
            ClockSkew = TimeSpan.Zero // Set agar token langsung kadaluarsa tanpa toleransi waktu
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseCors(MyAllowSpecificOrigins); // ini

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
        "default",
        "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

// app.Use(async (context, next) =>
// {
//     var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
//     await next();
// });

app.Run();