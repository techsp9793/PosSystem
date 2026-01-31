using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MudBlazor.Services;
using PosSystem.Client.Services;
using PosSystem.Components;
using PosSystem.Data;
using PosSystem.Data.Entities;
using PosSystem.Data.Repositories.Implementations;
using PosSystem.Data.Repositories.Interfaces;
using PosSystem.Data.Seeders;
using PosSystem.Services;

namespace PosSystem
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddMudServices();
            // Add services to the container.
            builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents(options =>
                {
                    // This will allow you to see the real C# exception in the browser console
                    options.DetailedErrors = builder.Environment.IsDevelopment();
                })
                .AddInteractiveWebAssemblyComponents();
            builder.Services.AddScoped<ThemeService>();
            builder.Services.AddScoped<ITenantService, TenantService>();
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped<SystemSettingsService>();
            builder.Services.AddScoped<NotificationService>();
            builder.Services.AddCascadingAuthenticationState();
            builder.Services.AddScoped<TerminologyService>();
            builder.Services.AddScoped<PosSystem.Services.PosStateService>();
            builder.Services.AddScoped<PosSystem.Services.ReceiptService>();
            builder.Services.AddScoped<PosSystem.Services.DashboardService>();
            builder.Services.AddScoped<PosSystem.Services.StockService>();
            builder.Services.AddAuthentication(options =>
                {
                    options.DefaultScheme = IdentityConstants.ApplicationScheme;
                    options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
                })
                .AddGoogle(options =>
                {
                    options.ClientId = builder.Configuration["Authentication:Google:ClientId"]
                        ?? throw new InvalidOperationException("Missing Google ClientId in configuration");
                    options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"]
                        ?? throw new InvalidOperationException("Missing Google ClientSecret in configuration");
                    options.SignInScheme = IdentityConstants.ExternalScheme;
                    options.SaveTokens = true; // Optional: stores tokens for later use
                })
            .AddFacebook(options =>
            {
                options.AppId = builder.Configuration["Authentication:Facebook:AppId"]
                    ?? throw new InvalidOperationException("Missing Facebook AppId in configuration");
                options.AppSecret = builder.Configuration["Authentication:Facebook:AppSecret"]
                    ?? throw new InvalidOperationException("Missing Facebook AppSecret in configuration");
                options.SignInScheme = IdentityConstants.ExternalScheme;
                options.SaveTokens = true;
            })
                .AddIdentityCookies();

            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            builder.Services.AddDbContextFactory<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            builder.Services.AddIdentityCore<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddRoles<ApplicationRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddSignInManager()
                .AddDefaultTokenProviders();
            //  Register the custom factory that adds tenant_id to the cookie
            builder.Services.AddScoped<IUserClaimsPrincipalFactory<ApplicationUser>, CustomUserClaimsPrincipalFactory>();
            builder.Services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
            builder.Services.AddScoped<IAuthorizationHandler, PermissionHandler>();
            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/login";
                options.LogoutPath = "/auth/logout";

                // This helps prevent the browser from showing 404 on "Back" 
                // by ensuring the redirect hits a valid route.
                options.Events.OnRedirectToLogin = context =>
                {
                    context.Response.Redirect(context.RedirectUri);
                    return Task.CompletedTask;
                };
            });
            var app = builder.Build();
            
            // This ensures scoped services like ApplicationDbContext can be resolved at startup
            using (var scope = app.Services.CreateScope())
            {
                await PermissionSeeder.SeedPermissionsAsync(scope.ServiceProvider);
                await DbInitializer.SeedRolesAndPermissionsAsync(scope.ServiceProvider);
            }
            
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseWebAssemblyDebugging();
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseStaticFiles();
            app.UseAntiforgery();

            app.MapRazorComponents<App>()
                .AddInteractiveServerRenderMode()
                .AddInteractiveWebAssemblyRenderMode()
                .AddAdditionalAssemblies(typeof(Client._Imports).Assembly);

            app.MapGet("/auth/login-bridge", async (string userId, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager) =>
            {
                // FIX: Search directly using EF Core to ignore Global Filters
                var user = await userManager.Users
                    .IgnoreQueryFilters()
                    .FirstOrDefaultAsync(u => u.Id == userId);

                if (user == null) return Results.Redirect("/login");

                // This runs on the server, securely setting the cookie
                await signInManager.SignInAsync(user, isPersistent: true);
                return Results.Redirect("/dashboard");
            });
            //app.MapGet("/auth/login-bridge", async (string userId, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager) =>
            //{
            //    var user = await userManager.FindByIdAsync(userId);
            //    if (user == null) return Results.Redirect("/login");

            //    // This runs on the server, securely setting the cookie
            //    await signInManager.SignInAsync(user, isPersistent: true);
            //    return Results.Redirect("/dashboard");
            //});
            app.MapGet("/auth/logout", async (SignInManager<ApplicationUser> signInManager) =>
            {
                await signInManager.SignOutAsync();
                return Results.Redirect("/login");
            });
            app.Run();
        }
    }
}
