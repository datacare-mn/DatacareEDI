using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Swashbuckle.Swagger.Model;
using EDIWEBAPI.Context;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using EDIWEBAPI.Utils;
using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Collections;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Authorization;
using EDIWEBAPI.Attributes;

namespace EDIWEBAPI
{
    public class Startup
    {
        private const string SecretKey = "SecretKeyEDI123456789";
        private readonly SymmetricSecurityKey _signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(SecretKey));
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            if (env.IsEnvironment("Development"))
            {
                // This will push telemetry data through Application Insights pipeline faster, allowing you to view results immediately.
                builder.AddApplicationInsightsSettings(developerMode: true);
            }

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
            Environment = env;
        }

        public IConfigurationRoot Configuration { get; }


        public IHostingEnvironment Environment { get; }

     

        // This method gets called by the runtime. Use this method to add services to the container
        public void ConfigureServices(IServiceCollection services)
        {
            string swaggerCommentXmlPath = string.Empty;
            if (Environment.IsDevelopment()) //while development
                swaggerCommentXmlPath = $@"{Environment.ContentRootPath}\bin\Debug\net452\EDIWEBAPI.xml";
            else //while production
                swaggerCommentXmlPath = $@"{Environment.ContentRootPath}\EDIWEBAPI.xml";


            // Add framework services.
            services.AddApplicationInsightsTelemetry(Configuration);

            services.AddScoped<OracleDbContext>((s) => new OracleDbContext(Configuration.GetSection("OracleConnection").Value));

            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials());
            });
            services.AddScoped<LogFilter>();
            services.AddScoped<LicenseAttribute>();
            services.AddMvc();

            services.AddMvc(options =>
            {
                // All endpoints need authentication
                options.Filters.Add(new AuthorizeFilter(new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build()));
            });
      





            services.AddSwaggerGen();

            services.Configure<FormOptions>(options =>
            {
                options.MultipartBodyLengthLimit = 50000000;
              
            });

            services.ConfigureSwaggerGen(options =>
            {
                options.SingleApiVersion(new Info
                {
                    Version ="v1",// Configuration.GetSection("Version").Value,
                    Title = "®WWW.URTO.MN",
                    Description = "REST API ©DOUBLE",
                    TermsOfService = "None",
                    Contact = new Contact() { Name = "DataCare LLC", Email = "info@datacare.mn", Url = "www.datacare.mn" }
                }
                );
                options.OperationFilter<FileUploadOperation>();
                options.IncludeXmlComments(swaggerCommentXmlPath);

                options.DescribeAllEnumsAsStrings();
            });


            // Configure JwtIssuerOptions
            var jwtAppSettingOptions = Configuration.GetSection(nameof(JwtIssuerOptions));

            // Configure JwtIssuerOptions
            services.Configure<JwtIssuerOptions>(options =>
            {
                options.Issuer = jwtAppSettingOptions[nameof(JwtIssuerOptions.Issuer)];
                options.Audience = jwtAppSettingOptions[nameof(JwtIssuerOptions.Audience)];
                options.SigningCredentials = new SigningCredentials(_signingKey, SecurityAlgorithms.HmacSha256);
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("EdiApiUser",
                                  policy => policy.RequireClaim("EdiCharacter", "IAmMapi"));
                options.AddPolicy("StoreApiUser",
                                  policy => policy.RequireClaim("StoreApiCharacter", "IAmStoreapi"));
                options.AddPolicy("BizApiUser",
                  policy => policy.RequireClaim("BizApiCharacter", "IAmBizapi"));

            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();
            loggerFactory.AddFile(Configuration.GetSection("LogFilePath").Value + $"//{DateTime.Today.ToString("yyyy-MM-dd")}//EdiAPI.txt");
            // CUSTOM LOG
            ApplicationLogging.LoggerFactory = loggerFactory;

            var jwtAppSettingOptions = Configuration.GetSection(nameof(JwtIssuerOptions));
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = jwtAppSettingOptions[nameof(JwtIssuerOptions.Issuer)],

                ValidateAudience = true,
                ValidAudience = jwtAppSettingOptions[nameof(JwtIssuerOptions.Audience)],

                ValidateIssuerSigningKey = true,
                IssuerSigningKey = _signingKey,

                RequireExpirationTime = true,
                ValidateLifetime = true,

                ClockSkew = TimeSpan.Zero,
                LifetimeValidator = CustomLifetimeValidator
            };

            app.UseJwtBearerAuthentication(new JwtBearerOptions
            {
                AutomaticAuthenticate = true,
                AutomaticChallenge = true,
                TokenValidationParameters = tokenValidationParameters
            });


            //string path = Configuration.GetSection("LogFilePath").Value + $"//{DateTime.Today.ToString("yyyy-MM-dd")}";
            //if (!Directory.Exists(path))
            //{
            //    DirectoryInfo di = Directory.CreateDirectory(path);
            //}

            // Try to create the directory.
            

            app.UseCors("CorsPolicy");
            //app.UseApplicationInsightsRequestTelemetry();

            //app.UseApplicationInsightsExceptionTelemetry();
            app.UseMvc();
            app.UseStaticFiles();

            if (Convert.ToBoolean(Configuration.GetSection("SwaggerDevelopmentMode").Value))
            {
                app.UseSwagger();
                app.UseSwaggerUi();
            }
            app.UseStaticFiles(new StaticFileOptions()
            {
                FileProvider = new PhysicalFileProvider(
                    Path.Combine(Directory.GetCurrentDirectory(), @"uploads")),
                RequestPath = new PathString("/uploads")
            });

        }

        private bool CustomLifetimeValidator(DateTime? notBefore, DateTime? expires, SecurityToken token, TokenValidationParameters @params)
        {
            return expires != null ? expires > DateTime.UtcNow : false;
        }
    }
}
