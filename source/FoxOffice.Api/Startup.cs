namespace FoxOffice.Api
{
    using Autofac;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Swashbuckle.AspNetCore.Swagger;
    using Swashbuckle.AspNetCore.SwaggerGen;
    using Swashbuckle.AspNetCore.SwaggerUI;

    public class Startup
    {
        private readonly AppModule _appModule;

        public Startup(IConfiguration config)
            => _appModule = new AppModule(GetSettings(config));

        private static AppSettings GetSettings(IConfiguration config) =>
            new AppSettings(
                config.GetConnectionString("Storage"),
                config["Messaging:Storage:QueueName"],
                config["Domain:Storage:EventStoreTableName"],
                config["ReadModel:CosmosDb:Endpoint"],
                config["ReadModel:CosmosDb:AuthKey"],
                config["ReadModel:CosmosDb:DatabaseId"],
                config["ReadModel:CosmosDb:CollectionId"]);

        public void ConfigureContainer(ContainerBuilder builder)
            => builder.RegisterModule(_appModule);

        public void ConfigureServices(IServiceCollection services)
            => services.AddSwaggerGen(ConfigureSwaggerGen).AddMvc();

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
            app.UseSwagger();
            app.UseSwaggerUI(c => ConfigureSwaggerUI(c));

            app.ApplicationServices.GetService<AppInitializer>().Initialize();
        }

        private static void ConfigureSwaggerGen(SwaggerGenOptions options)
            => options.SwaggerDoc("api", new Info { Title = "Fox Office" });

        private static void ConfigureSwaggerUI(SwaggerUIOptions options)
            => options.SwaggerEndpoint("/swagger/api/swagger.json", "API");
    }
}
