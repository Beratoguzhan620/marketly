using App.Application;
using App.Data;
using App.Eticaret.Infrastructure;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
    options.ModelMetadataDetailsProviders.Add(new TurkishValidationMetadataProvider());

    var messages = options.ModelBindingMessageProvider;
    messages.SetValueMustNotBeNullAccessor(_ => "Bu alan zorunludur.");
    messages.SetAttemptedValueIsInvalidAccessor((value, field) => $"'{value}' {field} için geçerli bir değer değil.");
    messages.SetValueIsInvalidAccessor(value => $"'{value}' geçerli bir değer değil.");
    messages.SetValueMustBeANumberAccessor(field => $"{field} bir sayı olmalıdır.");
    messages.SetMissingBindRequiredValueAccessor(field => $"{field} alanı zorunludur.");
    messages.SetUnknownValueIsInvalidAccessor(field => $"{field} için girilen değer geçerli değil.");
});

builder.Services.AddDataLayer(builder.Configuration.GetConnectionString("SqlServer")!);
builder.Services.AddApplicationLayer(builder.Configuration);

builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/login";
        options.AccessDeniedPath = "/access-denied";
    });

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/not-found");

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

await app.Services.MigrateDatabaseAsync();

await app.RunAsync();
