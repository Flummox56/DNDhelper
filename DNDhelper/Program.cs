using Microsoft.EntityFrameworkCore;
using DNDhelper.Data;
using DNDhelper.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// Подключение к PostgreSQL
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Добавляем AuthService в DI
builder.Services.AddScoped<AuthService>();

var app = builder.Build();

// Автоматическое применение миграций при старте
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    try
    {
        db.Database.Migrate();
        Console.WriteLine("Миграции применены успешно.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Ошибка при применении миграций: {ex.Message}");
    }
}

// Настройка маршрутов
app.UseRouting();
app.UseEndpoints(endpoints => endpoints.MapControllers());

app.Run();
