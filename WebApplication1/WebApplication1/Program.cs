using Microsoft.AspNetCore.Authorization;
using WebApplication1.Constants;
using WebApplication1.Data;

// ReSharper disable once CommentTypo
//Пароль от амдин-аккаунта — Whbs6aKx2PVSDdR?

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ??
                       throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));

builder.Services.AddIdentity<ApplicationUser, ApplicationRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultUI()
    .AddDefaultTokenProviders();

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddAuthorizationBuilder()
    .SetFallbackPolicy(new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build());
builder.Services.AddControllersWithViews();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var roleManager = services.GetRequiredService<RoleManager<ApplicationRole>>();
    await using var context = services.GetRequiredService<ApplicationDbContext>();
    await context.Database.MigrateAsync();

    await SeedRolesAsync(roleManager);
}


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets().AllowAnonymous();

app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Project}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapRazorPages()
    .WithStaticAssets();

await app.RunAsync();
return;


async Task SeedRolesAsync(RoleManager<ApplicationRole> roleManager)
{
    string[] roles = [AuthConstants.AdminRole, AuthConstants.ManagerRole, AuthConstants.ClientRole];

    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new ApplicationRole(role));
        }
    }
}