using PasskeyDemo.Interfaces;
using PasskeyDemo.Services;

namespace PasskeyDemo;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllersWithViews();
        
        ConfigureFido2(builder);
        AddCors(builder);
        AddServices(builder.Services);

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseRouting();


        app.MapControllerRoute(
            name: "default",
            pattern: "{controller}/{action=Index}/{id?}");

        app.MapFallbackToFile("index.html");
        ;

        app.Run();

    }
    
    private static void ConfigureFido2(WebApplicationBuilder builder)
    {
        var origins = new HashSet<string>();
        //TODO: Remember to update the Release Configuration with the appropriate settings
        origins.Add(builder.Configuration["fido2:origin"]);
        builder.Services.AddFido2(options =>
        {
            options.ServerDomain = builder.Configuration["fido2:serverDomain"];
            options.ServerName = "WebAuthn Test";
            options.Origins = origins;
            options.TimestampDriftTolerance = builder.Configuration.GetValue<int>("fido2:timestampDriftTolerance");
        });
    }

    private static void AddCors(WebApplicationBuilder builder)
    {
        builder.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
            {
                policy.AllowAnyOrigin();
                policy.AllowAnyHeader();
            });
        });
    }

    private static void AddServices(IServiceCollection services)
    {
        services.AddTransient<IUserRepository, DemoUserRepository>();
    }
}