using API.Extensions;
using API.Middleware;
using Infrastructure.Data;
using Infrastructure.Logging;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ---------- Logging (Serilog, configured in Infrastructure/Logging) ----------
builder.Host.UseAppSerilog(builder.Configuration);

// ---------- Services ----------
builder.Services.AddApiControllers();
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddValidationServices();
builder.Services.AddApiVersioningServices();
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddSwaggerDocumentation();
builder.Services.AddResponseCompression();

builder.Services.AddCors(options =>
{
    options.AddPolicy("DefaultCorsPolicy", policy =>
    {
        policy.WithOrigins(builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? Array.Empty<string>())
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// ---------- Apply migrations automatically on startup ----------
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();
}

// ---------- Middleware pipeline ----------
app.UseGlobalExceptionHandling();
app.UseSecurityHeaders();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Product Catalog API v1");
    });
}

app.UseHttpsRedirection();
app.UseResponseCompression();
app.UseCors("DefaultCorsPolicy");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program { }