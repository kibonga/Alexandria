using Microsoft.EntityFrameworkCore;
using Serilog;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

#region Database Connection
var connString = builder.Configuration.GetConnectionString("DefaultConnection");
//builder.Services.AddDbContext<AlexandriaDbContext>(options => options.UseSqlServer(connString));
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

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

#region Use Cors with Policy
app.UseCors("AllowAll"); 
#endregion

app.UseAuthorization();

app.MapControllers();

app.Run();
