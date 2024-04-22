﻿using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using System.Net;

IWebHostBuilder builder = WebHost.CreateDefaultBuilder(args)
    .UseKestrel(options =>
    {
        options.ListenAnyIP(8080);
    })
    .UseStartup<Startup>();

IWebHost app = builder.Build();
app.Run();
