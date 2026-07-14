using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using ProductionSheet.Components;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/login";
        options.AccessDeniedPath = "/access-denied";
        options.ExpireTimeSpan = TimeSpan.FromDays(30);
        options.SlidingExpiration = true;
    });
builder.Services.AddAuthorization();

builder.Services.AddScoped<QCTestLogService>();
builder.Services.AddScoped<OperatorAuditService>();
builder.Services.AddScoped<QCAuditService>();
builder.Services.AddScoped<SpecOptionsService>();
builder.Services.AddScoped<SpecRuleService>();
builder.Services.AddScoped<AutosaveSettingsService>();
builder.Services.AddScoped<EditRequestService>();
builder.Services.AddScoped<DailyQualityIssuesService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<CurrentUserService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();

app.UseAntiforgery();

app.MapGet("/login-as/{id:int}", async (int id, HttpContext ctx, UserService userService) =>
{
    var user = await userService.GetUserByIdAsync(id);
    if (user is null) return Results.Redirect("/login");

    var claims = new List<Claim>
    {
        new(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new(ClaimTypes.Name, user.Name),
    };
    if (user.IsMaster) claims.Add(new Claim(ClaimTypes.Role, "Master"));

    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
    await ctx.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity),
        new AuthenticationProperties { IsPersistent = true });

    return Results.Redirect("/");
}).AllowAnonymous();

app.MapGet("/logout", async (HttpContext ctx) =>
{
    await ctx.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    return Results.Redirect("/login");
}).AllowAnonymous();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
