using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WSGClienteCM.Connection;
using WSGClienteCM.Repository;
using WSGClienteCM.Services;
using WSGClienteCM.Utils;
using AutoMapper;
using WSGClienteCM.Profiles;

namespace WSGClienteCM
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
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2).AddControllersAsServices();
            services.AddCors(options =>
            {
                options.AddPolicy("AllowFromAll",
                    builder => builder
                    .WithMethods("GET", "POST", "PUT", "DELETE")
                    .AllowAnyOrigin()
                    .AllowAnyHeader());
            });
            var mappingConfig = new MapperConfiguration(mc => {
                mc.AddProfile(new ClientProfile());
                mc.ValidateInlineMaps = false;
            });
            IMapper mapper = mappingConfig.CreateMapper();
            services.AddSingleton(mapper);
            services.AddOptions();
            var appSettingsSection = Configuration.GetSection("AppSettings");
            services.Configure<AppSettings>(appSettingsSection);
            services.AddScoped<IConnectionBase, ConnectionBase>();
            services.AddScoped<ICargaMasivaService, CargaMasivaService>();
            services.AddScoped<ICargaMasivaRepository, CargaMasivaRepository>();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "Carga Masiva", Version = "v1" });
            }
              );
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

            app.UseHttpsRedirection();
            app.UseCors("AllowFromAll");
            app.UseMvc();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                //c.RoutePrefix = "swagger";
                c.SwaggerEndpoint("../swagger/v1/swagger.json", "Carga Masiva");
            });
        }
    }
}
