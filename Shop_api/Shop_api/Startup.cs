using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using Shop_api.Data;
using Shop_api.Models;

namespace Shop_api
{
    public class Startup
    {
        //private const string SecretKey = "iNivDmHLpUA223sqsfhqGbMRdRj1PVkH"; // todo: get this from somewhere secure
        //private readonly SymmetricSecurityKey _signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(SecretKey));

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<dataContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));
            services.AddIdentity<User, IdentityRole>(opts =>
                    {
                        opts.User.AllowedUserNameCharacters = null;
                    }
                )
                .AddEntityFrameworkStores<dataContext>()
                .AddDefaultTokenProviders();
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear(); // => remove default claims
            services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

                })
                .AddJwtBearer(cfg =>
                {
                    cfg.RequireHttpsMetadata = false;
                    cfg.SaveToken = true;
                    cfg.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidIssuer = Configuration["JwtIssuer"],
                        ValidAudience = Configuration["JwtIssuer"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JwtKey"])),
                        ClockSkew = TimeSpan.Zero // remove delay of token when expire
                    };
                });
            services.AddAuthentication(
                v =>
                {
                    v.DefaultAuthenticateScheme = GoogleDefaults.AuthenticationScheme;
                    v.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
                }).AddGoogle(googleOptions =>
                {
                    googleOptions.ClientId = Configuration["Authentication:Google:ClientId"];
                    googleOptions.ClientSecret = Configuration["Authentication:Google:ClientSecret"];
                });
            //services.AddAuthentication(options =>
            //{
            //    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            //    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            //    options.DefaultChallengeScheme = "GitHub";
            //})
            //.AddCookie()
            //.AddOAuth("GitHub", options =>
            //{
            //    options.ClientId = Configuration["GitHub:ClientId"];
            //    options.ClientSecret = Configuration["GitHub:ClientSecret"];
            //    options.CallbackPath = new PathString("/api/Account/SignInGithub");

            //    options.AuthorizationEndpoint = "https://github.com/login/oauth/authorize";
            //    options.TokenEndpoint = "https://github.com/login/oauth/access_token";
            //    options.UserInformationEndpoint = "https://api.github.com/user";

            //    options.ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "id");
            //    options.ClaimActions.MapJsonKey(ClaimTypes.Name, "name");
            //    options.ClaimActions.MapJsonKey("urn:github:login", "login");
            //    options.ClaimActions.MapJsonKey("urn:github:url", "html_url");
            //    options.ClaimActions.MapJsonKey("urn:github:avatar", "avatar_url");

            //    options.Events = new OAuthEvents
            //    {
            //        OnCreatingTicket = async context =>
            //        {
            //            var request = new HttpRequestMessage(HttpMethod.Get, context.Options.UserInformationEndpoint);
            //            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            //            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", context.AccessToken);

            //            var response = await context.Backchannel.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, context.HttpContext.RequestAborted);
            //            response.EnsureSuccessStatusCode();

            //            var user = JObject.Parse(await response.Content.ReadAsStringAsync());

            //            context.RunClaimActions(user);
            //            return;
            //        }
            //    };
            //});
            services.AddCors(options =>
    options.AddPolicy("AllowMyOrigin",
builder => builder.WithOrigins("https://localhost:44311").AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()
    ));
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseStaticFiles();
            app.UseFileServer();
            app.UseCors("AllowMyOrigin");
            app.UseAuthentication();
            //app.UseOAuthAuthentication(GitHubOptions);

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
