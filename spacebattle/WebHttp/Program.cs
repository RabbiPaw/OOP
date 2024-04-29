using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using System.Diagnostics.CodeAnalysis;
using System.Net;
[ExcludeFromCodeCoverage]
internal class Program
{
    private static void Main(string[] args)
    {
        IWebHostBuilder builder = WebHost.CreateDefaultBuilder(args)
        .UseKestrel(options =>
        {
            options.ListenAnyIP(8080);
        })
        .UseStartup<Startup>();

        IWebHost app = builder.Build();
        app.Run();
    }
}
