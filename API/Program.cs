using API.Data;
using API.Extensions;
using API.Middleware;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Moving application services to ApplicationServiceExtensions and using it here.
builder.Services.AddApplicationServices(builder.Configuration);

// Moving identity services to IdentityServiceExtensions and using it here.
builder.Services.AddIdentityServices(builder.Configuration);

// app is the object that will be used to configure the request pipeline.
var app = builder.Build();

// ExceptionMiddleware is used to handle exceptions.
app.UseMiddleware<ExceptionMiddleware>();

// app.UseCors() is used to allow cross-origin requests.
app.UseCors(policy => policy.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());

// First we need to use authentication and then authorization.
// UseAuthentication() authenticates if the user is valid or not.
app.UseAuthentication();

// UseAuthorization() authorizes if the user is allowed to access the resource or not.
app.UseAuthorization();

// Configure the HTTP request pipeline.
app.MapControllers();


// Seeding the database.
using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;
try 
{
    var context = services.GetRequiredService<DataContext>();
    await context.Database.MigrateAsync();
    await Seed.SeedUsers(context);
    await Seed.SeedDonations(context);
}
catch (Exception ex)
{
    var logger = services.GetService<ILogger<Program>>();
    logger.LogError(ex, "An error occurred during migration");
}

// Run the app.
app.Run();
