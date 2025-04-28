using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Notification.Models;
using Notification.Models.BasicAuth;
using static Notification.Models.BasicAuth.BasicAuthServices;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;
using System.IO;
using Microsoft.AspNetCore.Mvc.Versioning;

using FluentMigrator.Runner;
using System.Reflection;
using MySql.Data.MySqlClient;

namespace Notification
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<IBasicAuthService, BasicAuthService>();

            var db_host = Environment.GetEnvironmentVariable("DB_HOST");
            var db_port = Environment.GetEnvironmentVariable("DB_PORT");
            var db_name = Environment.GetEnvironmentVariable("DB_NAME");
            var db_user = Environment.GetEnvironmentVariable("DB_USER");
            var db_password = Environment.GetEnvironmentVariable("DB_PASS");

           



            var conn = "Server=" + db_host + ";Port=" + db_port + ";Database=" + db_name + ";Uid=" + db_user + ";Pwd=" + db_password + ";Convert Zero Datetime=True";
            //conn = "Server=127.0.0.1;Port=3306;Database=KONNECT_TPP;User=root;Password=Andrei_123!;"; 

            // error of No service for type 'MySql.Data.MySqlClient.MySqlConnection' has been registered.
            // MySqlConnection is not registered in the DI
            services.AddTransient<MySqlConnection>(_ =>
            {
                var conn = "Server=" + db_host + ";Port=" + db_port + ";Database=" + db_name + ";User=" + db_user + ";Password=" + db_password + ";";
                return new MySqlConnection(conn);
            });

            Console.WriteLine("conn: " + conn);

            services.Add(new ServiceDescriptor(typeof(DBContext), new DBContext(conn)));
            services.AddHttpContextAccessor();


            var connectionString = Configuration.GetConnectionString("DefaultConnection");

            // Register FluentMigrator
            services.AddFluentMigratorCore()
             .ConfigureRunner(rb => rb
                 .AddMySql5()
                 .WithGlobalConnectionString(conn)
                 .ScanIn(Assembly.GetExecutingAssembly()).For.Migrations());




            services.AddControllers();

            BasicAuthServices.conn = conn;
            services.AddAuthentication("BasicAuthentication")
                .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("BasicAuthentication", null);

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc(name: "v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "Notification", Version = "v1" });
                c.AddSecurityDefinition("basic", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "basic",
                    In = ParameterLocation.Header,
                    Description = "Basic Authorization header using the Bearer scheme."
                });

                //var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                //var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                //c.IncludeXmlComments(xmlPath);

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                          new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "basic"
                                }
                            },
                            new string[] {}
                    }
                });
                
                c.OperationFilter<CustomHeaderSwaggerAttribute>();
            });

            services.AddMvc().AddJsonOptions(o =>
            {
                o.JsonSerializerOptions.PropertyNamingPolicy = null;
                o.JsonSerializerOptions.DictionaryKeyPolicy = null;

            });

            //FOR VERSIONING
            services.AddApiVersioning(options =>
            {
                options.ReportApiVersions = true;
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.ApiVersionReader = new UrlSegmentApiVersionReader();
            });

            services.AddVersionedApiExplorer(setup =>
            {
                setup.GroupNameFormat = "'v'VV";
                setup.SubstituteApiVersionInUrl = true;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Notification");
            });


            app.UseHttpsRedirection();

            app.UseRouting();

            using (var scope = app.ApplicationServices.CreateScope())
            {
                var dbConnection = scope.ServiceProvider.GetRequiredService<MySqlConnection>();
                try
                {
                    dbConnection.Open();
                    Console.WriteLine("Database connection successful!!!");
                    dbConnection.Close();
                }
                catch (Exception error)
                {
                    Console.WriteLine($"X Database connection failed: {error.Message}");
                }

                try
                {
                    var migrator = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
                    //migrator.MigrateDown(0);// remove all
                    migrator.MigrateUp();
                    Console.WriteLine("Migrations applied successfully!");
                }
                catch (Exception error)
                {
                    Console.WriteLine($"X Migration failed: {error.Message}");
                }

            }


            app.UseAuthentication(); // this is use for basic authentication, you need this particular line for basic auth

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        public class CustomHeaderSwaggerAttribute : IOperationFilter
        {

            public void Apply(OpenApiOperation operation, OperationFilterContext context)
            {
                if (operation.Parameters == null)
                    operation.Parameters = new List<OpenApiParameter>();

                operation.Parameters.Add(new OpenApiParameter
                {
                    Name = "APIKEY",
                    In = ParameterLocation.Header,
                    Required = false
                });
            }
        }

    }
}
