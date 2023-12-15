using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http.Features;
using Swashbuckle.Swagger.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using StoreAPI.Helpers;
using System.Text;
using Owin;
using StoreAPI.SocketUtils;
using StoreAPI.Chat;

namespace StoreAPI
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

        public IHostingEnvironment Environment { get; }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container
        public void ConfigureServices(IServiceCollection services)
        {

           
            string swaggerCommentXmlPath = string.Empty;
            if (Environment.IsDevelopment()) //while development
                swaggerCommentXmlPath = $@"{Environment.ContentRootPath}\bin\Debug\net452\STOREAPI.xml";
            else //while production
                swaggerCommentXmlPath = $@"{Environment.ContentRootPath}\STOREAPI.xml";

            services.AddScoped<OracleDbContext>((s) => new OracleDbContext(Configuration.GetSection("OracleConnection").Value));
            services.AddApplicationInsightsTelemetry(Configuration);

            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials());
            });
            services.AddMvc();
            services.AddApiVersioning(o => {
                o.AssumeDefaultVersionWhenUnspecified = true;
                o.DefaultApiVersion = new ApiVersion(1, 0);
            });

            services.AddSwaggerGen();

            services.Configure<FormOptions>(options =>
            {
                options.MultipartBodyLengthLimit = 50000000;
            });


            services.ConfigureSwaggerGen(options =>
            {
                //options.SingleApiVersion(new Info
                //{
                //    Version = "v1",
                //    Title = "®STORE API",
                //    Description = "STORE API ©DOUBLE",
                //    TermsOfService = "None",
                //    Contact = new Contact() { Name = "Davkharbayar", Email = "mdavkharbayar@gmail.com", Url = "www.datacare.mn" }

                //}

               // );
                options.IncludeXmlComments(swaggerCommentXmlPath);
                options.DescribeAllEnumsAsStrings();
            });

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
                options.AddPolicy("BIApiUser",
                                  policy => policy.RequireClaim("BICharacter", "IAmMapi"));

            });

            services.AddWebSocketManager();
           
           

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IServiceProvider serviceProvider)
        {
            app.UseWebSockets();
            app.MapWebSocketManager("/ws", serviceProvider.GetService<ChatMessageHandler>());
            app.MapWebSocketManager("/test", serviceProvider.GetService<TestMessageHandler>());


            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();
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
                ClockSkew = TimeSpan.Zero
            };

            app.UseJwtBearerAuthentication(new JwtBearerOptions
            {
                AutomaticAuthenticate = true,
                AutomaticChallenge = true,
                TokenValidationParameters = tokenValidationParameters
            });
            loggerFactory.AddFile(Configuration.GetSection("LogFilePath").Value + "//Logs/STOREApi-{Date}.txt");
            app.UseStaticFiles();
            app.UseCors("CorsPolicy");
            app.UseMvc();
            app.UseSwagger();
            app.UseSwaggerUi();


        }
    }
}
