using Microsoft.AspNetCore.Components.Authorization;
using Radzen;
using RadzenUI.auth;
using RadzenUI.Data;

namespace RadzenUI;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddRazorPages();
        builder.Services.AddServerSideBlazor();
        
        builder.Services.AddScoped<IDataProvider, SqliteDataProvider>();
        builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();
        builder.Services.AddScoped<AppState>();
        builder.Services.AddRadzenComponents();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();

        app.UseStaticFiles();

        app.UseRouting();

        app.MapBlazorHub();

        // AuthEndpoints.MapEndpoints(app);
                
        app.MapFallbackToPage("/_Host");

        app.Run();
    }   

}

//public class AuthEndpoints
//{
//    public static void MapEndpoints(WebApplication app)
//    {
//        app.MapPost("/Auth/Login", (LoginViewModel model) =>
//        {
//            if(model == null) return Results.BadRequest();

//            if(string.IsNullOrEmpty(model.Username) || string.IsNullOrEmpty(model.Password)) return Results.BadRequest();   

//            if(model.Username.Equals("Mike") && model.Password.Equals("Penny"))
//            {
//                return Results.SignIn( new ClaimsPrincipal(
//                    new ClaimsIdentity(
//                        new List<Claim>()
//                        {

//                        },"Cookies"
//                    )
//                ), new AuthenticationProperties() { RedirectUri="/"});                
//            };

//            return Results.Unauthorized();
//        });
               
//    }
//}

//internal class LoginViewModel
//{
//    [Required]
//    public string Username { get; set; }

//    [Required]
//    [PasswordPropertyText]
//    public string Password { get; set; }

//    public bool IsValid { get => !string.IsNullOrEmpty(Username) && !string.IsNullOrEmpty(Password); }
//}