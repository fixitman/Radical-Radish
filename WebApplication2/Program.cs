using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.DataProtection;
using System.Security.Claims;
using Isopoh.Cryptography;
using Microsoft.Extensions.Logging.Console;
using WebApplication2.Models;
using Isopoh.Cryptography.Argon2;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace WebApplication2
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddAuthentication("Cookies")
                .AddCookie("Cookies", opt =>
                {
                    opt.LoginPath = "/loginPage";
                    
                });            
            
            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("IsAdmin", policyBuilder =>
                {
                    policyBuilder
                        .RequireAuthenticatedUser()
                        .RequireClaim("admin");
                });
            });
            
            builder.Services.AddScoped<AuthData>((_) =>
            {
                return new AuthData(builder.Configuration.GetConnectionString("Default"));
            });
                         
            var app = builder.Build();




            app.UseAuthentication();
            app.UseAuthorization();
            //app.UseStatusCodePages();

            app.MapGet("/", (HttpContext ctx, IConfiguration _config,  AuthData authData) =>
            {
                var users = authData.GetData();

                return users;
            }).RequireAuthorization("IsAdmin");
            
            app.MapGet("/loginPage", (HttpContext ctx, IConfiguration _config,  AuthData authData) =>
            {
                return Results.Content("<h3>Login Please</h3>");
               
                
            });

            app.MapPost("/logout", () =>
            {
                return Results.SignOut(new AuthenticationProperties() { RedirectUri = "/" });
            });
            
            
            app.MapPost("/Account/login", async (LoginViewModel model, HttpContext ctx, AuthData authData) =>
            {
                if(string.IsNullOrEmpty(model.UserName) || string.IsNullOrEmpty(model.Password))
                {
                    return Results.BadRequest();
                }

                var user = authData.GetUserByName(model.UserName);
                if (user == null)
                {
                    return Results.Unauthorized();
                }

                var verified = Argon2.Verify(user.Password, model.Password);
                if (!verified)
                {
                    return Results.Unauthorized();
                }

                List<Claim> claims = new List<Claim>()
                {
                    new Claim("userId", user.Id.ToString()),
                    new Claim("userName", user.UserName),
                    new Claim("admin","true")
                };
                
                var cid = new ClaimsIdentity(claims,"Cookies");

                //await ctx.SignInAsync(new ClaimsPrincipal(cid));



                return Results.SignIn(new ClaimsPrincipal(cid), new AuthenticationProperties() { RedirectUri = "/" });
                
            });

            app.MapPost("/Account/Register", (RegisterViewModel model, AuthData authData) =>
            {
                authData.AddUser(new UserModel() 
                { 
                    UserName=model.UserName, 
                    Password=Argon2.Hash(model.Password)
                });
                return;
            });
                        

            app.Run();
        }
    }
}