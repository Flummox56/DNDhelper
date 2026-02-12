using Microsoft.EntityFrameworkCore;
using DNDhelper.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AuthDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AuthDbContext>();
    try
    {
        dbContext.Database.Migrate();
        Console.WriteLine("Success");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"DB Error: {ex.Message}");
    }
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthorization();
app.MapControllers();

app.Run();

//dotnet ef migrations add InitialCreate  <--- это только в директории ./DNDhelper/DNDhelper/
//docker exec -it auth_postgres psql -U auth_user -d auth_db