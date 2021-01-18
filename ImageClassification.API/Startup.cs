using ImageClassification.API.Configurations;
using ImageClassification.API.Extensions;
using ImageClassification.Shared.DataModels;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.ML;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ImageClassification.API
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
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.Configure<MLModelOptions>(Configuration.GetSection(MLModelOptions.MLModel));
            
            services.AddCustomServices();

            services.AddControllers()
                    .AddNewtonsoftJson(options =>
                    {
                        options.SerializerSettings.Converters.Add(new StringEnumConverter());
                        options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                    });
            services.AddRazorPages();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "ImageClassification.API", Version = "v1" });
            });
            services.AddSwaggerGenNewtonsoftSupport();

            // Register the PredictionEnginePool as a service in the IoC container for DI.
            services.AddPredictionEnginePool<InMemoryImageData, ImagePrediction>()
                    .FromFile(Configuration["MLModel:MLModelFilePath"]);

            services.WarmUpPredictionEnginePool(Configuration["MLModel:WarmupImagePath"]);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "ImageClassification.API v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
