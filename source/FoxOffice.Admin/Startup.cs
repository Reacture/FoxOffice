namespace FoxOffice.Admin
{
    using System;
    using System.Net.Http;
    using FoxOffice.Admin.Services;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            var apiService = new ApiClient(
                new HttpClient(),
                new Uri(Configuration["Api:Endpoint"]));

            services.AddSingleton<ISendCreateTheaterCommandService>(apiService);
            services.AddSingleton<IGetAllTheatersService>(apiService);

            services.AddSingleton<ISendCreateMovieCommandService>(apiService);
            services.AddSingleton<ISendAddScreeningCommandService>(apiService);
            services.AddSingleton<IGetAllMoviesService>(apiService);
            services.AddSingleton<IFindMovieService>(apiService);

            services.AddSingleton<IResourceAwaiter>(apiService);

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
