using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
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
        builder.Services.AddControllers();
        
        ConfigureFido2(builder);
        AddCors(builder);
        AddAuthentication(builder);
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
        app.UseAuthentication();
        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });


        app.MapControllerRoute(
            name: "default",
            pattern: "{controller}/{action=Index}/{id?}");

        app.MapFallbackToFile("index.html");

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
        services.AddTransient<ICredentialRepository, DemoUserRepository>();
        services.AddTransient<IReadUserCredential, DemoUserRepository>();
        services.AddTransient<ITokenGenerator, JwtSecurityGenerator>();
        services.AddTransient<IWebAuthentication, WebAuthentication>();
        services.AddTransient<IUserRegistration, DefaultUserRegistration>();
    }

    private static void AddAuthentication(WebApplicationBuilder builder)
    {
        var secret = builder.Configuration["Security:AppSecretKey"];
        if (secret is null or "") throw new Exception("AppSecretKey not set");

        var key = Encoding.ASCII.GetBytes(secret);

        builder.Services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                };
            });
    }
}