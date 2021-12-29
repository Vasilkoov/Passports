using FilesStorage.Extensions;
using FilesStorage.FilesHandlers;
using FilesStorage.FilesHandlers.Interfaces;
using FilesStorage.Settings;
using FilesStorage.Settings.Interfaces;
using FilesStorage.Updaters;
using FilesStorage.Updaters.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace FilesStorage
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
            services.AddAutoMapper(typeof(Startup));
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "PassportsService", Version = "v1" });
            });

            var extraConf = new ExtraAppSettings();
            Configuration.Bind(nameof(ExtraAppSettings), extraConf);
            services.AddSingleton<IExtraAppSettings>(extraConf);

            services.AddScoped<IUpdater, Updater>();
            services.AddSingleton<IFileProcessor, FileProcessor>();

            //Добавляем репорзиторий в зависимости от конфига
            services.AddRepository(Configuration);

            //добавляем ежедневные обновления
            services.AddQuartz(Configuration, extraConf.CronExpForDailyUpdate);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "PassportsService v1"));

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
