using Microsoft.Extensions.DependencyInjection;
using OtpModule.Abstractions;
using OtpModule.Core;
using OtpModule.Infrastructure;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using System.Reflection;


namespace OtpModule.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddOtpModule(
        this IServiceCollection services,
        Action<OtpOptions>? setup = null)
    {
        if (setup is not null)
            services.Configure(setup);
        else
            services.Configure<OtpOptions>(_ => { });

        services
            .AddScoped<IOtpGenerator, DefaultOtpGenerator>();
        services
            .AddScoped<IHashService, Sha256HashService>();
        services
            .AddScoped<IOtpService, OtpService>();

        services.AddControllers()
            .PartManager.ApplicationParts.Add(
                new AssemblyPart(Assembly.GetExecutingAssembly()));


        return services;
    }
}
