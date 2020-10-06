using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Doctors_practice.Models;
using Doctors_practice.Models.Doctor;
using Doctors_practice.Models.Practice;
using Doctors_practice.Models.Appointment;
using Doctors_practice.Models.Patient;
using MediatR;
using EventStoreImplementation;
using Doctors_practice.BusinessLayer;
using Doctors_practice.Commands;
using System.Reflection;
using Microsoft.AspNetCore.Authorization;
using AuthorisationLibrary;
using Doctors_practice.Extensions;
using StackExchange.Redis;
using Doctors_practice.Services;

namespace Doctors_practice
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
            services.AddAuthentication("Bearer")
                .AddJwtBearer("Bearer", options =>
                {
                    options.Authority = Configuration.GetConnectionString("identity");
                    options.RequireHttpsMetadata = false;
                    options.Audience = "doctorsPracticeApi";
                });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("Reader", policy =>
                {
                    policy.Requirements.Add(new ScopeRequirement("api1.read"));
                });
                options.AddPolicy("Writer", policy =>
                {
                    policy.Requirements.Add(new ScopeRequirement("api1.write"));
                });
            });

            services.AddElasticsearch(Configuration);
            services.AddSingleton<IAuthorizationHandler, ScopeHandler>();
            services.AddTransient<IDummyDB, DummyDB>();
            services.AddTransient<IDummyChargingSystem, DummyChargingSystem>();
            services.AddTransient<ICustomer, Customer>();
            services.AddScoped<IPatientClient, AMQPatientClient>();
            services.AddScoped<IAppointmentRepository, SQLAppointmentRepository>();
            services.AddScoped<IDoctorRepository, SQLDoctorRepository>();
            services.AddScoped<IPracticeRepository, SQLPracticeRepository>();
            services.AddScoped<IPatientRepository, SQLPatientRepository>();
            services.AddScoped<IEventStore, EventStoreImplementation.EventStore>();
            services.AddMediatR(typeof(Startup));
            //services.AddSingleton<ICacheService, InMemoryCacheService>();
            services.AddSingleton<IConnectionMultiplexer>(x =>
                ConnectionMultiplexer.Connect(Configuration.GetConnectionString("redis")));
            services.AddSingleton<ICacheService, RedisCacheService>();
            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            
            app.UseRequestResponseLogging();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }
    }
}
