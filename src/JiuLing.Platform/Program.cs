using Blazored.LocalStorage;
using JiuLing.Platform.Common;
using JiuLing.Platform.Common.Services;
using JiuLing.Platform.Components;
using JiuLing.Platform.Hub;
using JiuLing.Platform.Repositories;
using JiuLing.Platform.Repositories.Contexts;
using JiuLing.Platform.Services.HashCheckService;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MudBlazor;
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

        builder.Services.AddMudServices(config =>
        {
            config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.TopCenter;
        });
        builder.Services.AddMudExtensions();

        builder.Services.AddDbContextFactory<AppDbContext>
            (options => options.UseNpgsql(builder.Configuration.GetConnectionString("appConnection")));
        // 处理该死的时间问题
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

        // 显示详细错误信息
        builder.Services.AddServerSideBlazor().AddCircuitOptions(options => { options.DetailedErrors = true; });

        builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));

        builder.Services.Configure<FilePathConfig>(builder.Configuration.GetSection("FilePaths"));

        builder.Services.AddHttpClient("VirusTotal", (sp, client) =>
        {
            var appSettings = sp.GetService<IOptions<AppSettings>>()?.Value ?? throw new ArgumentException("配置文件异常");
            client.BaseAddress = new Uri("https://www.virustotal.com/");
            client.DefaultRequestHeaders.Add("x-apikey", appSettings.VirusTotalApiKey);
        });

        builder.Services.AddHttpContextAccessor();
        builder.Services.AddMemoryCache();
        builder.Services.AddSignalR();
        builder.Services.AddControllers().AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.PropertyNamingPolicy = null;  // 取消驼峰命名
        });

        builder.Services.AddSingleton<EmailService>(sp =>
        {
            var email = sp.GetRequiredService<IOptions<AppSettings>>().Value.Email;
            var settings = new EmailSettings()
            {
                Host = email.Host,
                Port = email.Port,
                Username = email.Username,
                Password = email.Password,
                Address = email.Address,
                DisplayName = email.DisplayName
            };
            return new EmailService(settings);
        });

        builder.Services.AddSingleton<JwtTokenService>();
        builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();
        builder.Services.AddAuthorizationCore();
        builder.Services.AddBlazoredLocalStorage();

        builder.Services.AddScoped<HashServiceFactory>();
        builder.Services.AddScoped<IDatabaseConfigService, DatabaseConfigService>();
        builder.Services.AddScoped<IVirusTotalService, VirusTotalService>();
        builder.Services.AddScoped<IAppService, AppService>();
        builder.Services.AddScoped<IDonationService, DonationService>();
        builder.Services.AddScoped<IUserService, UserService>();
        builder.Services.AddScoped<IIssueService, IssueService>();

        builder.Services.AddScoped<IAppReleaseRepository, AppReleaseRepository>();
        builder.Services.AddScoped<IConfigBaseRepository, ConfigBaseRepository>();
        builder.Services.AddScoped<IAppBaseRepository, AppBaseRepository>();
        builder.Services.AddScoped<IDonationUsageRepository, DonationUsageRepository>();
        builder.Services.AddScoped<IDonationRepository, DonationRepository>();
        builder.Services.AddScoped<IUserRepository, UserRepository>();
        builder.Services.AddScoped<IIssueRepository, IssueRepository>();

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

        app.MapControllers();

        app.UseStaticFiles();
        app.UseStatusCodePagesWithRedirects("/404");
        app.Run();
    }
}
