using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace SuggestionAppConsole
{
    class Program
    {
        static void Main(string [] args)
        {
            var host = ConfigContainer.CreatеHostBuilder(args).Build();
            using(var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var app = services.GetRequiredService<IApplication>();
                    app.Run();
                }
                catch (Exception)
                {
                    throw;
                }
            }
            host.Run();
        }
    }
}