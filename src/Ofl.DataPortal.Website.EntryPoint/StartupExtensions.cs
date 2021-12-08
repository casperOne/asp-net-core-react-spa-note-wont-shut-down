using System;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authorization;

namespace Ofl.DataPortal.Website.EntryPoint
{
    public static class StartupExtensions
    {
        public static AuthenticationBuilder AddAuthentication(this IServiceCollection serviceCollection)
        {
            // Ease-of-use
            var sc = serviceCollection;

            // Add authentication.
            return sc.AddAuthentication(options => {
                // Add JWT as default.
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            });
        }

        public static IServiceCollection AddAuthorization(this IServiceCollection serviceCollection)
        {
            // Ease-of-use
            var sc = serviceCollection;

            // Add authentication.
            sc.AddAuthorization(o => {
                // Create a policy builder.
                var builder = new AuthorizationPolicyBuilder(
                    JwtBearerDefaults.AuthenticationScheme
                );

                // Require an authenticated user.
                builder = builder.RequireAuthenticatedUser();

                // Set the default policy.
                o.DefaultPolicy = builder.Build();
            });

            // Return the service collection.
            return sc;
        }

        public static IApplicationBuilder ForwardHttpsFromContainer(this IApplicationBuilder app)
        {
            // Validate parameters.
            if (app == null) throw new ArgumentNullException(nameof(app));

            // Set up routing of HTTPS from the docker container, as per:
            // https://stackoverflow.com/a/51153544/50776
            var forwardOptions = new ForwardedHeadersOptions {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto,
                RequireHeaderSymmetry = false
            };

            // Need to clear known networks and proxies.
            forwardOptions.KnownNetworks.Clear();
            forwardOptions.KnownProxies.Clear();

            // Use forwarded headers.
            return app.UseForwardedHeaders(forwardOptions);
        }

        public static IMvcBuilder AddOflMvc(
            this IServiceCollection serviceCollection
        )
        {
            // Validate parameters.
            var sc = serviceCollection ?? throw new ArgumentNullException(nameof(serviceCollection));

            // Set up services.
            return sc
                // Add controllers only.
                .AddControllers(options => {
                    // Add HTTPS only.
                    options.Filters.Add(new RequireHttpsAttribute());
                })
                // Set compatibility version.
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0);
        }
    }
}
