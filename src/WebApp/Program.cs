using WebApp.Components;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.HttpOverrides;


namespace WebApp;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);


        // Add services to the container.
        builder.Services.AddRazorComponents()
            .AddInteractiveServerComponents();

        builder.Services.AddAuthentication(options =>
        {
            options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
        })
        .AddCookie()
        .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
        {
            var config = builder.Configuration.GetSection("Cognito");
            options.Authority = config["Authority"];
            options.ClientId = config["ClientId"];
            options.ResponseType = "code";
            options.MetadataAddress = config["MetadataAddress"];
            options.CallbackPath = "/signin-oidc";

            options.SaveTokens = true;
            options.Scope.Add("openid");
            options.Scope.Add("profile");
        });
        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }



        app.UseHttpsRedirection();

        app.UseForwardedHeaders(new ForwardedHeadersOptions
        {
            ForwardedHeaders = ForwardedHeaders.XForwardedProto
        });

        app.UseAntiforgery();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapStaticAssets();
        app.MapRazorComponents<App>()
            .AddInteractiveServerRenderMode();


        app.MapGet("/login", async context =>
        {
            await context.ChallengeAsync(OpenIdConnectDefaults.AuthenticationScheme, new AuthenticationProperties
            {
                RedirectUri = "/"
            });
        });

        app.MapGet("/logout", async context =>
        {
            await context.SignOutAsync("Cookies");
            await context.SignOutAsync("oidc", new AuthenticationProperties
            {
                RedirectUri = "/"
            });
        });

        app.Use(async (context, next) =>
        {
            if (context.Request.Path.StartsWithSegments("/login"))
            {
                Console.WriteLine("Login requested.");
            }

            await next();
        });

        app.Run();
    }
}
