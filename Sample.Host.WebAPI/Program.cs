using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.IdentityModel.Logging;
using Microsoft.OpenApi.Models;
using pandx.Wheel;
using pandx.Wheel.Extensions;
using pandx.Wheel.SignalR;
using Serilog;

Bootstrapper.ConfigureBootstrapLogger();
try
{
    var builder = WebApplication.CreateBuilder(args);
    //configureWheel
    builder.ConfigureWheel();
    //swagger
    builder.Services.AddEndpointsApiExplorer();

    builder.Services.AddSwaggerGen(options =>
    {
        options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
        {
            Description = "Token:Bearer Token",
            Name = "Authorization",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.ApiKey,
            BearerFormat = "JWT",
            Scheme = "Bearer"
        });
        options.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            [new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            }] = new string[] { }
        });
    });

    var app = builder.Build();
    //initial
    await Bootstrapper.InitializeAsync(app);
    //exception
    app.UseExceptionHandler();
    //environment
    if (app.Environment.IsDevelopment())
    {
        IdentityModelEventSource.ShowPII = true;
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    //forwardedHeaders
    app.UseForwardedHeaders(new ForwardedHeadersOptions
    {
        ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
    });
    //cors
    app.UseCors("default");
    app.UseHttpsRedirection();
    app.UseAuthentication();
    app.UseAuthorization();
    //useWheel
    app.UseWheel();
    //route
    app.MapHub<OnlineClientHub>("/signalr");
    app.MapControllers();
    //run
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "程序异常终止");
}
finally
{
    await Log.CloseAndFlushAsync();
}