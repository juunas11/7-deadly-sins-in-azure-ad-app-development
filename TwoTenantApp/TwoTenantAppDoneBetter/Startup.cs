using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using TwoTenantAppDoneBetter.Options;

namespace TwoTenantAppDoneBetter
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


            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            var authSettings = Configuration.GetSection("Auth").Get<AuthSettings>();
            services.AddAuthentication(o =>
            {
                o.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                o.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
                o.DefaultForbidScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
            .AddCookie(o =>
            {
                o.Cookie.Name = "TwoTenantAppDoneBetter.Auth";
            })
            .AddOpenIdConnect(o =>
            {
                o.ClientId = authSettings.ClientId;
                o.CallbackPath = "/signin-aad";
                o.Authority = "https://login.microsoftonline.com/organizations/v2.0";
                o.SignedOutRedirectUri = "/Account/SignedOut";
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    // NOTE: We should not turn issuer validation off
                    // We should instead list the valid issuers
                    // You can find your issuer URI at: https://login.microsoftonline.com/tenant-id-here/v2.0/.well-known/openid-configuration
                    // It's in the "issuer" property
                    NameClaimType = "name",
                    ValidIssuers = new[] // THIS IS IMPORTANT Only accept tokens from these tenants
                    {
                        $"https://login.microsoftonline.com/{authSettings.EmployeeTenantId}/v2.0",
                        $"https://login.microsoftonline.com/{authSettings.PartnerTenantId}/v2.0"
                    }
                };
                o.Events = new OpenIdConnectEvents
                {
                    OnRedirectToIdentityProvider = ctx =>
                    {
                        // Replace the common endpoint with tenant-specific endpoint
                        // Trying to login with an account from another tenant causes an error in Azure AD
                        // NOTE: The flaw with this is that the user can just change the URL!
                        if (ctx.Properties.Parameters.TryGetValue("userType", out var userTypeObj)
                            && userTypeObj is string userType)
                        {
                            // Here we specify prompt="consent" for the sake of demos,
                            // it forces user consent on every login
                            // Yoy may want to remove it to enable SSO
                            if (userType == "employee")
                            {
                                ctx.ProtocolMessage.IssuerAddress = authSettings.EmployeeAuthorizationEndpoint;
                                ctx.ProtocolMessage.SetParameter("prompt", "consent");
                                return Task.CompletedTask;
                            }
                            else if (userType == "partner")
                            {
                                ctx.ProtocolMessage.IssuerAddress = authSettings.PartnerAuthorizationEndpoint;
                                ctx.ProtocolMessage.SetParameter("prompt", "consent");
                                return Task.CompletedTask;
                            }
                        }

                        throw new ArgumentException("Invalid user type specified");
                    },
                    OnTokenValidated = ctx =>
                    {
                        string tid = ctx.Principal.FindFirst("http://schemas.microsoft.com/identity/claims/tenantid").Value;
                        string userType;
                        // Extra check, the exception should never be thrown
                        if (tid == authSettings.EmployeeTenantId)
                        {
                            userType = "employee";
                        }
                        else if (tid == authSettings.PartnerTenantId)
                        {
                            userType = "partner";
                        }
                        else
                        {
                            throw new Exception("Tenant id not allowed");
                        }

                        var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Role, userType)
                        };
                        var appIdentity = new ClaimsIdentity(claims);

                        ctx.Principal.AddIdentity(appIdentity);

                        return Task.CompletedTask;
                    }
                };
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
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
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
