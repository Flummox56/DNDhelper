partial class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var app = builder.Build();

        app.Map("/" ,() => "Hello world");

        app.Run();        
    }
}