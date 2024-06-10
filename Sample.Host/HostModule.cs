using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using pandx.Wheel.DependencyInjection;
using pandx.Wheel.Extensions;
using pandx.Wheel.Modules;
using pandx.Wheel.Security;
using Sample.Domain.Authorization.Permissions;
using Sample.Domain.Authorization.Roles;
using Sample.Domain.Authorization.Users;
using Sample.EntityFrameworkCore;

namespace Sample.Host;

public class HostModule : IModule
{
    public void Initialize(WebApplicationBuilder builder)
    {
        builder.Services.AddServicesByConvention(typeof(HostModule).Assembly);
        //options
        builder.Services.AddOptions<SecuritySettings>()
            .Bind(builder.Configuration.GetSection("SecuritySettings"))
            .ValidateDataAnnotations()
            .ValidateOnStart();
        //cors
        builder.Services.AddCors(options => options.AddPolicy("default", policyBuilder => policyBuilder.WithOrigins(
                builder.Configuration["App:CorsOrigins"]!
                    .Split(";", StringSplitOptions.RemoveEmptyEntries)
                    .Select(c => c.RemovePostFix("/"))
                    .ToArray()
            )
            .SetIsOriginAllowedToAllowWildcardSubdomains()
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials()));
        //identity
        builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
        {
            options.Password.RequiredLength = 6;
            options.Password.RequireDigit = false;
            options.Password.RequireLowercase = false;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = false;
            options.User.RequireUniqueEmail = true;
        }).AddEntityFrameworkStores<SampleDbContext>().AddDefaultTokenProviders();
        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
        {
            options.SaveToken = true;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey =
                    new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(builder.Configuration["SecuritySettings:JwtBearer:SecurityKey"] ??
                                               throw new InvalidOperationException("没有获取到SecurityKey"))),

                ValidateIssuer = false,
                ValidIssuer = builder.Configuration["SecuritySettings:JwtBearer:Issuer"],

                ValidateAudience = false,
                ValidAudience = builder.Configuration["SecuritySettings:JwtBearer:Audience"],

                ValidateLifetime = true,

                ClockSkew = TimeSpan.Zero
            };

            options.Events = new JwtBearerEvents
            {
                OnMessageReceived = context =>
                {
                    if (context.HttpContext.Request.Path.HasValue &&
                        (context.HttpContext.Request.Path.Value.StartsWith("/signalr",
                             StringComparison.OrdinalIgnoreCase) ||
                         context.HttpContext.Request.Path.Value.StartsWith("/notifications",
                             StringComparison.OrdinalIgnoreCase)))
                    {
                        var encryptedToken = context.Request.Query["encrypted-token"].FirstOrDefault();
                        if (encryptedToken is not null)
                        {
                            var x = SimpleStringCipher.Instance.Decrypt(encryptedToken);
                            context.Token = SimpleStringCipher.Instance.Decrypt(encryptedToken);
                        }
                    }

                    return Task.CompletedTask;
                }
            };
        });
        builder.Services.AddScoped<IAuthorizationHandler, SamplePermissionAuthorizationHandler>();
    }
}