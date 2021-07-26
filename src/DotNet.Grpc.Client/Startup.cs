using DotNet.Grpc.Server.Protos;
using DotNet.Grpc.Server.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System;

namespace DotNet.Grpc.Client
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "DotNet.Grpc.Client", Version = "v1" });
            });

            services.AddCors(options =>
            {
                options.AddPolicy("Full",
                    builder =>
                        builder
                            .AllowAnyOrigin()
                            .AllowAnyMethod()
                            .AllowAnyHeader());
            });

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            // gRPC IoC
            services.AddScoped<IGreeterGrpcService, GreeterGrpcService>();
            services.AddGrpcClient<Greeter.GreeterClient>(options =>
            {
                options.Address = new Uri(Configuration["gRPCServerUrl"]);
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "DotNet.Grpc.Client v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors("Full");

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}