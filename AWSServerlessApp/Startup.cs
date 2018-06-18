using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using AWSServerlessApp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using AWSServerlessApp.Connection;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Net;
using System.Threading.Tasks;
using System;
using Microsoft.AspNetCore.Identity;
using Swashbuckle.AspNetCore.Swagger;
using System.IO;
using Microsoft.AspNetCore.Diagnostics;
using Newtonsoft.Json;
using AWSServerlessApp.CustomModels;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Http;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Newtonsoft.Json.Serialization;
using AWSServerlessApp.LogProvider;

namespace AWSServerlessApp
{
    public class Startup
    {
        public const string AppS3BucketKey = "AppS3Bucket";

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);
           
            builder.AddEnvironmentVariables();
          
            Configuration = builder.Build();
        }

        public static IConfigurationRoot Configuration { get; private set; }

        // This method gets called by the runtime. Use this method to add services to the container
        public void ConfigureServices(IServiceCollection services)
        {

        
            services.Configure<JWTSettings>(Configuration.GetSection("JWTSettings"));
         //   services.Configure<PasswordSettings>(Configuration.GetSection("PasswordSettings"));

            services.AddDbContext<NetCoreIdentitySampleContext>(options =>
            options.UseSqlServer(Configuration.GetConnectionString("DbConnection")));
            //For IdentityCore
            services.AddIdentity<ApplicationUser, ApplicationRole> ()
                .AddEntityFrameworkStores<NetCoreIdentitySampleContext>()
                .AddDefaultTokenProviders(); 
            services.Configure<IdentityOptions>(options =>
            {
                // avoid redirecting REST clients on 401
                options.Cookies.ApplicationCookie.Events = new CookieAuthenticationEvents
                {
                    OnRedirectToLogin = ctx =>
                    {
                        ctx.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                        return Task.FromResult(0);
                    }
                };
            });

            services.AddCors(options => options.AddPolicy("AllowAll", p => p.AllowAnyOrigin()
                                                                            .AllowAnyMethod()
                                                                            .AllowAnyHeader()));

            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear(); // => remove default claims
            services
                .AddAuthentication(options =>
                {
                    options.SignInScheme = JwtBearerDefaults.AuthenticationScheme;

                });
              
            //.AddJwtBearer(cfg =>
            //{
            //    cfg.RequireHttpsMetadata = false;
            //    cfg.SaveToken = true;
            //    cfg.TokenValidationParameters = new TokenValidationParameters
            //    {
            //        ValidIssuer = Configuration["JwtIssuer"],
            //        ValidAudience = Configuration["JwtIssuer"],
            //        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JwtKey"])),
            //        ClockSkew = TimeSpan.Zero // remove delay of token when expire
            //    };
            //});

            services.AddMvcCore(config => {
                config.Filters.Add(typeof(CustomExceptionFilter));
                 })
                .AddApiExplorer()
                .AddAuthorization() // Note - this is on the IMvcBuilder, not the service collection
                .AddJsonFormatters(options => options.ContractResolver = new CamelCasePropertyNamesContractResolver());
            //services.AddMvc(
            //    config => {
            //        config.Filters.Add(typeof(CustomExceptionFilter));
            //    }
         //   );

         //   services.AddMvc();
            //services.AddAuthorization(options =>
            //{
            //    options.AddPolicy("RequireAdminRole", policy => policy.RequireRole("User"));
            //});


            // Pull in any SDK configuration from Configuration object
            services.AddDefaultAWSOptions(Configuration.GetAWSOptions());

            // Add S3 to the ASP.NET Core dependency injection framework.
            services.AddAWSService<Amazon.S3.IAmazonS3>();
            services.AddOptions();

            // Configure ConnectionStrings using config
            services.Configure<ConnectionStrings>(Configuration.GetSection("ConnectionStrings"));
            services.AddSingleton<IConfiguration>(Configuration);

            services.AddIdentity<ApplicationUser, ApplicationRole>(x =>
            {
                x.Password.RequiredLength = Convert.ToInt32(Configuration.GetSection("PasswordSettings:RequiredLength").Value);
                x.Password.RequireUppercase = Convert.ToBoolean(Configuration.GetSection("PasswordSettings:RequireUppercase").Value);
                x.Password.RequireDigit = Convert.ToBoolean(Configuration.GetSection("PasswordSettings:RequireDigit").Value);
                x.Password.RequireNonAlphanumeric = Convert.ToBoolean(Configuration.GetSection("PasswordSettings:RequireNonAlphanumeric").Value);
                x.Password.RequireLowercase = Convert.ToBoolean(Configuration.GetSection("PasswordSettings:RequireLowercase").Value);
            }).AddEntityFrameworkStores<NetCoreIdentitySampleContext>().AddDefaultTokenProviders();

            // Register the Swagger generator, defining one or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info {
                    Title = "Application Made Easy API",
                    Version = "v1",
                    Description = "Application Made Easy API using ASP.NET Core",
                    TermsOfService = "None"
                });
                c.DescribeAllEnumsAsStrings();
                c.DescribeAllParametersInCamelCase();
                c.IncludeXmlComments(string.Format(@"{0}\bin\Debug\netcoreapp1.0\AWSServerlessApp.xml",
                         Directory.GetCurrentDirectory()));
            });
        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline
        public async void Configure(IApplicationBuilder app, IHostingEnvironment env, 
                                    ILoggerFactory loggerFactory, IServiceProvider serviceProvider,
                                    RoleManager<ApplicationRole> _roleManager)
        {

            loggerFactory.AddConsole(Configuration.GetSection("ConsoleLogging"));
            loggerFactory.AddDebug();

            LogLevel dbloglevel=LogLevel.Information;
            string dblevelvalue = Configuration.GetSection("DatabaseLogging:Level").Value.ToString();
            switch (dblevelvalue)
                {
                    case "None": dbloglevel = LogLevel.None; break;
                    case "Critical": dbloglevel = LogLevel.Critical; break;
                    case "Debug": dbloglevel = LogLevel.Debug; break;
                    case "Error": dbloglevel = LogLevel.Error; break;
                    case "Information": dbloglevel = LogLevel.Information; break;
                    case "Trace": dbloglevel = LogLevel.Trace; break;
                    case "Warning": dbloglevel = LogLevel.Warning; break;
            }
            loggerFactory.AddContext(dbloglevel, Configuration.GetConnectionString("DbConnection"));

            LogLevel fileloglevel = LogLevel.Information;
            string filelevelvalue = Configuration.GetSection("FileLogging:Level").Value.ToString();
            switch (filelevelvalue)
            {
                case "None": fileloglevel = LogLevel.None; break;
                case "Critical": fileloglevel = LogLevel.Critical; break;
                case "Debug": fileloglevel = LogLevel.Debug; break;
                case "Error": fileloglevel = LogLevel.Error; break;
                case "Information": fileloglevel = LogLevel.Information; break;
                case "Trace": fileloglevel = LogLevel.Trace; break;
                case "Warning": fileloglevel = LogLevel.Warning; break;
            }
            if(filelevelvalue!="None")
             loggerFactory.AddFile("Logs/myapp-{Date}.txt", fileloglevel);
           

            app.UseJwtBearerAuthentication(new JwtBearerOptions
            {
                AutomaticAuthenticate = true,
                AutomaticChallenge = true,
                TokenValidationParameters = new TokenValidationParameters
                {
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration.GetSection("JWTSettings:SecretKey").Value.ToString())),
                    ValidAudience = Configuration.GetSection("JWTSettings:Audience").Value.ToString(),
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = true,
                    ValidIssuer = Configuration.GetSection("JWTSettings:Issuer").Value.ToString()
                }
            });
            app.UseIdentity();

            app.UseGoogleAuthentication(new GoogleOptions()
            {
                ClientId = Configuration.GetSection("GoogleSettings:ClientId").Value, 
                ClientSecret = Configuration.GetSection("GoogleSettings:ClientSecret").Value,
                Scope = { "email", "openid" },
                SaveTokens = true,
                AccessType = "offline"

            });

            app.UseFacebookAuthentication(new FacebookOptions()
            {
                AppId = Configuration.GetSection("FacebookSettings:AppId").Value,
                AppSecret = Configuration.GetSection("FacebookSettings:AppSecret").Value,
                SaveTokens = true
            });

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });
            app.UseCors("AllowAll");
        
            // Simple error page to avoid a repo dependency.
            app.Use(async (context, next) =>
            {
                try
                {
                    await next();
                }
                catch (Exception ex)
                {
                    if (context.Response.HasStarted)
                    {
                        throw;
                    }
                    context.Response.StatusCode = 500;
                    context.Response.ContentType = "application/json";
                    var json = JToken.FromObject(ex);
                    await context.Response.WriteAsync(json.ToString());
                }
            });
            app.UseMvc();
        

        }

     
    }

    public class AspNetUsersConfiguration
    {
        public AspNetUsersConfiguration(EntityTypeBuilder<AspNetUsers> entity)
        {
            entity.ToTable("AspNetUsers", "dbo");
            entity.HasKey(e => e.Id);
        }
    }
    public class AspNetRolesConfiguration
    {
        public AspNetRolesConfiguration(EntityTypeBuilder<AspNetRoles> entity)
        {
            entity.ToTable("AspNetRoles", "dbo");
            entity.HasKey(e => e.Id);
        }
    }
    public class AspNetUserClaimsConfiguration
    {
        public AspNetUserClaimsConfiguration(EntityTypeBuilder<AspNetUserClaims> entity)
        {
            entity.ToTable("AspNetUserClaims", "dbo");
            entity.HasKey(e => e.Id);
        }
    }
    public class AspNetUserLoginsConfiguration
    {
        public AspNetUserLoginsConfiguration(EntityTypeBuilder<AspNetUserLogins> entity)
        {
            entity.ToTable("AspNetUserLogins", "dbo");
            entity.HasKey(e => e.UserId);
        }
    }
    public class AspNetRoleClaimsConfiguration
    {
        public AspNetRoleClaimsConfiguration(EntityTypeBuilder<AspNetRoleClaims> entity)
        {
            entity.ToTable("AspNetRoleClaims", "dbo");
            entity.HasKey(e => e.Id);
        }
    }
    public class AspNetUserTokensConfiguration
    {
        public AspNetUserTokensConfiguration(EntityTypeBuilder<AspNetUserTokens> entity)
        {
            entity.ToTable("AspNetUserTokens", "dbo");
            entity.HasKey(e => e.UserId);
        }
    }
    public class AspNetUserRolesConfiguration
    {
        public AspNetUserRolesConfiguration(EntityTypeBuilder<AspNetUserRoles> entity)
        {
            entity.ToTable("AspNetUserRoles", "dbo");
            entity.HasKey(e => e.UserId);
        }
    }
    public class AspNetUserPinsConfiguration
    {
        public AspNetUserPinsConfiguration(EntityTypeBuilder<AspNetUserPins> entity)
        {
            entity.ToTable("AspNetUserPins", "dbo");
            entity.HasKey(e => e.Id);
        }
    }
    public class NotificationsConfiguration
    {
        public NotificationsConfiguration(EntityTypeBuilder<Notifications> entity)
        {
            entity.ToTable("Notifications", "dbo");
            entity.HasKey(e => e.Id);
        }
    }
}
