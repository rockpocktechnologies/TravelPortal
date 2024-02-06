using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Web.UI;
using Microsoft.Identity.Web;


var builder = WebApplication.CreateBuilder(args);

string isLocal = builder.Configuration.GetValue<string>("EmailSettings:IsLocal");

if (isLocal == "true")
{
    builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation(); ;

}
else
{
    builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
        .AddMicrosoftIdentityWebApp(builder.Configuration)
        .EnableTokenAcquisitionToCallDownstreamApi(builder.Configuration.GetValue<string>("DownstreamApi:Scopes")?.Split(' '))
        .AddMicrosoftGraph(builder.Configuration.GetSection("DownstreamApi"))
        .AddInMemoryTokenCaches();

    builder.Services.AddControllersWithViews(options =>
    {
        var policy = new AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser()
            .Build();
        options.Filters.Add(new AuthorizeFilter(policy));
    }).AddRazorRuntimeCompilation();

}



builder.Services.AddRazorPages()
    .AddMicrosoftIdentityUI();

builder.Services.AddServerSideBlazor()
    .AddMicrosoftIdentityConsentHandler();

builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(10);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});



var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=NewRequest}/{action=Index}/{id?}");

app.MapRazorPages();
app.Run();
