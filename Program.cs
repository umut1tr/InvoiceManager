using IdentityApp.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using IdentityApp.Authorization;

var builder = WebApplication.CreateBuilder(args);

// gets connectionString from secrets.json file
var connectionString = builder.Configuration.GetValue<string>("DBConnectionString");

// Add services to the container.

// old method gets DB connectionString from appsettings.json
// var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>() // adds User Roles to IdentityUser
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddRazorPages();


// Identity User config
builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 4;
    options.Lockout.MaxFailedAccessAttempts = 3;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
    options.Lockout.AllowedForNewUsers = true;
    options.User.RequireUniqueEmail = true;
});


// Adds Authorization to Page that you can not access the page without being logged in
builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
    
});

// everytime using something with Identity use AddScoped to register service else only do Add a Singleton
builder.Services.AddScoped<IAuthorizationHandler, InvoiceCreatorAuthorizationHandler>(); // registers our Creator to Auth

// not using any Identity stuff so we need to use Singleton here
builder.Services.AddSingleton<IAuthorizationHandler, InvoiceManagerAuthorizationHandler>(); // register our Auth handler for Manager
builder.Services.AddSingleton<IAuthorizationHandler, InvoiceAdminAuthorizationHandler>(); // register our Auth handler for Admin

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{

    var services = scope.ServiceProvider;

    // this will check if a DB is concurrent if not then it will try to apply and migrate a DB on hosting machine
    var context = services.GetRequiredService<ApplicationDbContext>();
    context.Database.Migrate();

    // this gets the credentials saved in the secret.json in user secrets manager 
    var seedUserPass = builder.Configuration.GetValue<string>("SeedUserPass");
    
    // initialize accountant/manager/admin user accounts
    await SeedData.Initialize(services, seedUserPass);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
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

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.Run();
