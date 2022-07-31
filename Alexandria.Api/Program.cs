using Alexandria.Api.Configurations;
using Alexandria.Api.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

#region Register Services
//
#region Database Connection
var connString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AlexandriaDbContext>(options => options.UseSqlServer(connString));
#endregion

#region Register Identity
builder.Services
    .AddIdentityCore<ApiUser>()
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<AlexandriaDbContext>(); // Comment: User database can be separated from Applications database
#endregion

#region Logger - SeriLog
// Comment-1: Serilog - 3rd party library for Logging
// Comment-2: Serilog.Expressions - lets us add configurations in the appsettings.json
builder.Host.UseSerilog((ctx, lc) =>
{
    lc.WriteTo.Console().ReadFrom.Configuration(ctx.Configuration);
    // Comment-3:
    // ctx - configuration context
    // lc - logging configuration
});
#endregion

#region Register Cors
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", b => b.AllowAnyMethod().AllowAnyHeader().AllowAnyOrigin());
});
#endregion

#region Register Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero,
        ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
        ValidAudience = builder.Configuration["JwtSettings:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Key"]))
    };
});
#endregion

#region Register Automapper
builder.Services.AddAutoMapper(typeof(MapperConfig));
#endregion
//
#endregion

#region Example how to register services container in .NET 6
builder.Services.ExampleRegisterServicesExtension(builder.Configuration); 
#endregion

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

#region Use Services
//
#region Use Cors with Policy
app.UseCors("AllowAll");
#endregion

#region Use Authentication
// Comment: Authentication before Authorization
app.UseAuthentication();
#endregion 
//
#endregion

app.UseAuthorization();

app.MapControllers();

app.Run();
