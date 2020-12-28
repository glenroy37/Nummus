using System;
using MatBlazor;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Nummus.Areas.Identity;
using Nummus.Data;
using Nummus.Service;
using Nummus.Helper;

namespace Nummus {
    public class Startup {
        public Startup(IConfiguration configuration) {
            _configuration = configuration;
        }

        private IConfiguration _configuration;

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services) {
            services.AddDbContext<NummusDbContext>(options =>
                options.UseMySql(
                    _configuration.GetConnectionString("DefaultConnection"),
                    new MariaDbServerVersion(new Version(10, 5))));
            services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<NummusDbContext>();
            services.AddRazorPages();
            services.AddServerSideBlazor();
            services.AddMatBlazor();
            services.AddScoped<AuthenticationStateProvider, RevalidatingIdentityAuthenticationStateProvider<IdentityUser>>();
            services.AddDatabaseDeveloperPageExceptionFilter();

            //Own Services
            services.AddSingleton<FormatService>();
            services.AddScoped<IdentityHelper>();
            services.AddScoped<BrowserHelper>();
            services.AddScoped<NummusUserService>();
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IBookingLineService, BookingLineService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IAccountStatementService, AccountStatementService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
                app.UseMigrationsEndPoint();
            } else {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints => {
                endpoints.MapControllers();
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
}
