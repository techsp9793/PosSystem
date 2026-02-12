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
using PosSystem.Hubs; // [NEW] Required for PaymentHub

namespace PosSystem
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddHttpContextAccessor();
            builder.Services.AddMudServices();

            // [NEW] Required for Razorpay Webhook & Real-time updates
            builder.Services.AddControllers();
            builder.Services.AddSignalR();

            // Add services to the container.
            builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents(options =>
                {
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

            // Domain Services
            builder.Services.AddScoped<PosSystem.Services.PosStateService>();
            builder.Services.AddScoped<PosSystem.Services.ReceiptService>();
            builder.Services.AddScoped<PosSystem.Services.DashboardService>();
            builder.Services.AddScoped<PosSystem.Services.StockService>();
            // In Program.cs
            builder.Services.AddScoped<PosSystem.Services.SmsService>();
            // [NEW] Register Payment Service (Missing in your undo)
            builder.Services.AddScoped<IPaymentService, PaymentService>();
            // Add this line with your other service registrations
            builder.Services.AddScoped<QrCoreService>();
            builder.Services.AddScoped<ReportService>();
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
                    options.SaveTokens = true;
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

            // 1. Primary SQL Server
            builder.Services.AddDbContextFactory<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));

            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            // 2. Local SQLite (The Resilient Core)
            builder.Services.AddDbContextFactory<LocalDbContext>(options =>
                options.UseSqlite("Data Source=pos_local_core.db"));

            // 3. Background Sync Service
            builder.Services.AddHostedService<BackgroundSyncService>();

            builder.Services.AddIdentityCore<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddRoles<ApplicationRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddSignInManager()
                .AddDefaultTokenProviders();

            builder.Services.AddScoped<IUserClaimsPrincipalFactory<ApplicationUser>, CustomUserClaimsPrincipalFactory>();
            builder.Services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
            builder.Services.AddScoped<IAuthorizationHandler, PermissionHandler>();

            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/login";
                options.LogoutPath = "/auth/logout";
                options.Events.OnRedirectToLogin = context =>
                {
                    context.Response.Redirect(context.RedirectUri);
                    return Task.CompletedTask;
                };
            });

            var app = builder.Build();

            // --- INITIALIZATION BLOCK ---
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                // A. Create the Local SQLite File immediately
                try
                {
                    var localDbFactory = services.GetRequiredService<IDbContextFactory<LocalDbContext>>();
                    using var localDb = localDbFactory.CreateDbContext();
                    localDb.Database.EnsureCreated();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Local DB Init Error: {ex.Message}");
                }

                // B. Seed SQL Server Data
                await PermissionSeeder.SeedPermissionsAsync(services);
                await DbInitializer.SeedRolesAndPermissionsAsync(services);
            }
            // ---------------------------

            if (app.Environment.IsDevelopment())
            {
                app.UseWebAssemblyDebugging();
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseAntiforgery();

            // [NEW] Map Endpoints for Webhook & SignalR
            app.MapControllers();
            app.MapHub<PaymentHub>("/paymentHub");

            app.MapRazorComponents<App>()
                .AddInteractiveServerRenderMode()
                .AddInteractiveWebAssemblyRenderMode()
                .AddAdditionalAssemblies(typeof(Client._Imports).Assembly);

            app.MapGet("/auth/login-bridge", async (string userId, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager) =>
            {
                var user = await userManager.Users
                    .IgnoreQueryFilters()
                    .FirstOrDefaultAsync(u => u.Id == userId);

                if (user == null) return Results.Redirect("/login");

                await signInManager.SignInAsync(user, isPersistent: true);
                return Results.Redirect("/dashboard");
            });

            app.MapGet("/auth/logout", async (SignInManager<ApplicationUser> signInManager) =>
            {
                await signInManager.SignOutAsync();
                return Results.Redirect("/login");
            });

            app.Run();
        }
    }
}