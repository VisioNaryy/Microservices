using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using PlatformService.AsyncDataServices;
using PlatformService.Data;
using PlatformService.SyncDataServices.Grpc;
using PlatformService.SyncDataServices.HTTP;
using System;

namespace PlatformService
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        private readonly IWebHostEnvironment _env;

        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            _env = env;
        }     

        public void ConfigureServices(IServiceCollection services)
        {
            if (_env.IsProduction())
            {
                Console.WriteLine("--> Using SqlServer Db");
                services.AddDbContext<AppDbContext>(x =>
                {
                    x.UseSqlServer(Configuration.GetConnectionString("PlatformsConn"));
                });
            }
            else
            {
                Console.WriteLine("--> Using InMem Db");
                services.AddDbContext<AppDbContext>(x => x.UseInMemoryDatabase("InMem"));
            }

            services.AddScoped<IPlatformRepo, PlatformRepo>();
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            services.AddHttpClient<ICommandDataClient, HttpCommandDataClient>();
            services.AddSingleton<IMessageBusClient, MessageBusClient>();
            services.AddGrpc();

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "PlatformService", Version = "v1" });
            });

            Console.WriteLine($"--> CommandService Endpoint: {Configuration["CommandServiceUrl"]}");

            //services.AddCors(options =>
            //{
            //    options.AddPolicy("AllowOrigin",
            //        builder => builder
            //        //.WithOrigins("http://localhost:4200")
            //        .AllowAnyOrigin()
            //        .AllowAnyMethod()
            //        .AllowAnyHeader());
            //});
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "PlatformService v1"));
            }

            app.UseRouting();

            //app.UseCors("AllowOrigin");

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapGrpcService<GrpcPlatformService>();

                endpoints.MapGet("/protos/platforms.proto", async context => 
                {
                    await context.Response.WriteAsync(System.IO.File.ReadAllText("Protos/platforms.proto"));
                });
            });

            PrepDb.PrepPopulation(app, env.IsProduction());
        }
    }
}