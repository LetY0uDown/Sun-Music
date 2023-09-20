using Host.Interfaces;
using Host.Services;
using Host.Services.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.IdentityModel.Tokens;
using Models.Database;
using System.Text;

namespace Host.Tools.Extensions;

internal static class ServiceCollectionExtensions
{
    internal static void SetupBase(this IServiceCollection services)
    {
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
    }

    internal static void AddServices(this IServiceCollection services)
    {
        services.AddTransient<IPasswordEncoder, PasswordEncoder>();
        services.AddTransient<IAuthTokenGen, JWTTokenGenerator>();
        services.AddSingleton<IPathHelper, PathHelper>();

        services.AddTransient<IMusicTrackService, MusicTrackService>();
    }

    internal static void AddRepositories(this IServiceCollection services)
    {
        services.AddTransient<IAsyncRepository<MusicTrack>, MusicTracksRepository>();
        services.AddTransient<IAsyncRepository<TrackLike>, TrackLikeRepository>();
        services.AddTransient<IAsyncRepository<User>, UserRepository>();
    }

    internal static void ConfigureOptions(this IServiceCollection services)
    {
        services.Configure<FormOptions>(options =>
        {
            options.ValueLengthLimit = int.MaxValue;
            options.MultipartBodyLengthLimit = long.MaxValue;
            options.MultipartHeadersLengthLimit = int.MaxValue;
        });

        services.Configure<IISServerOptions>(options =>
        {
            options.MaxRequestBodySize = long.MaxValue;
        });
    }

    internal static void AddAuthorization(this IServiceCollection services, IConfiguration config)
    {
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options => {
                    options.TokenValidationParameters = new()
                    {
                        ValidateIssuer = true,
                        ValidIssuer = config["JWT:Issuer"],

                        ValidateAudience = true,
                        ValidAudience = config["JWT:Audience"],
                        ValidateLifetime = true,

                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["JWT:Key"]))
                    };
                });

        services.AddAuthorization(options => options.DefaultPolicy = new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme)
                                                                                    .RequireAuthenticatedUser()
                                                                                    .Build());    }
}