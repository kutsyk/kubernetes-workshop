using System.IO;
using k8s;
using MasterAPI.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;
#pragma warning disable 1591

namespace MasterAPI
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
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info
                {
                    Version = "v1",
                    Title = $"MasterEmulator {Configuration[MasterConstants.Namespace]} API",
                    Description = "BoostHeat Master Emulator API",
                    TermsOfService = "None"
                });
                var filePath = Path.Combine(System.AppContext.BaseDirectory, "MasterAPI.xml");
                c.IncludeXmlComments(filePath);
            });
            services.AddSingleton(Configuration);

            var configFilePath = Configuration[MasterConstants.ConfigFilePath];
            var config = string.IsNullOrEmpty(configFilePath)
                ? KubernetesClientConfiguration.BuildConfigFromConfigFile()
                : KubernetesClientConfiguration.BuildConfigFromConfigFile(new FileInfo(configFilePath));

            services.AddSingleton<IKubernetes>(new Kubernetes(config));
            services.AddScoped<KubernetesService>();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
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
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "MasterEmulator v1");
            });

            app.UseCors(builder => builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
