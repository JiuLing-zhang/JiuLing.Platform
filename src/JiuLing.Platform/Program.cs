using JiuLing.Platform.Components;
using JiuLing.Platform.Hub;
using JiuLing.Platform.Repositories;
using JiuLing.Platform.Repositories.Contexts;
using JiuLing.Platform.Services.HashCheckService;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MudBlazor.Services;
using MudExtensions.Services;

namespace JiuLing.Platform;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.AddServiceDefaults();

        // Add services to the container.
        builder.Services.AddRazorComponents()
            .AddInteractiveServerComponents();

        builder.Services.AddMudServices();
        builder.Services.AddMudExtensions();

        builder.Services.AddDbContextFactory<AppDbContext>
            (options => options.UseNpgsql(builder.Configuration.GetConnectionString("appConnection")));

        builder.Services.AddServerSideBlazor().AddCircuitOptions(options => { options.DetailedErrors = true; });

        builder.Services.AddScoped<HashServiceFactory>();

        builder.Services.AddScoped<IDatabaseConfigService, DatabaseConfigService>();
        builder.Services.AddScoped<IVirusTotalService, VirusTotalService>();
        builder.Services.AddScoped<IAppService, AppService>();
        builder.Services.AddScoped<IDonationService, DonationService>();

        builder.Services.AddScoped<IAppReleaseRepository, AppReleaseRepository>();
        builder.Services.AddScoped<IConfigBaseRepository, ConfigBaseRepository>();
        builder.Services.AddScoped<IAppBaseRepository, AppBaseRepository>();
        builder.Services.AddScoped<IDonationRepository, DonationRepository>();
        builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));

        builder.Services.AddHttpClient("VirusTotal", (sp, client) =>
        {
            var appSettings = sp.GetService<IOptions<AppSettings>>()?.Value ?? throw new ArgumentException("配置文件异常");
            client.BaseAddress = new Uri("https://www.virustotal.com/");
            client.DefaultRequestHeaders.Add("x-apikey", appSettings.VirusTotalApiKey);
        });

        builder.Services.AddSignalR();

        var app = builder.Build();

        app.MapDefaultEndpoints();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.MapHub<FileTransferHub>("/file-transfer-hub");

        app.UseHttpsRedirection();

        app.UseAntiforgery();

        app.MapStaticAssets();
        app.MapRazorComponents<App>()
            .AddInteractiveServerRenderMode();

        app.Run();
    }
}
