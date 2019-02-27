using AppWithRoles.Authorization;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AppWithRoles
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        private IConfiguration Configuration { get; }
        
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddMvc(o =>
            {
                o.Filters.Add(new AuthorizeFilter());
            }).SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddAuthentication(o =>
            {
                o.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                o.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
                o.DefaultForbidScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            }).AddOpenIdConnect(o =>
            {
                o.Authority = Configuration["Auth:Authority"];
                o.ClientId = Configuration["Auth:ClientId"];
                o.CallbackPath = "/signin-oidc";
                o.SignedOutRedirectUri = "/Account/SignedOut";
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    NameClaimType = "name"
                };
                o.Events = new OpenIdConnectEvents
                {
                    OnRedirectToIdentityProvider = ctx =>
                    {
                        // Here we specify prompt="consent" for the sake of demos,
                        // it forces user consent on every login
                        // Yoy may want to remove it to enable SSO
                        ctx.ProtocolMessage.SetParameter("prompt", "consent");
                        return Task.CompletedTask;
                    }
                };
            }).AddCookie(o =>
            {
                o.Cookie.Name = "AppWithRoles.Auth";
                o.AccessDeniedPath = "/Account/AccessDenied";
            });

            services.AddAuthorization(o =>
            {
                o.AddPolicy(Policies.AdminOnly, p =>
                {
                    // Require role
                    p.RequireClaim(ClaimTypes.Role, Configuration["Auth:AdminRoleValue"]);
                });
            });
        }
        
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
