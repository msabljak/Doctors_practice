using Doctors_practice.Models;
using Doctors_practice.Models.Appointment;
using Doctors_practice.Models.Doctor;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Doctors_practice.Extensions
{
    public static class ElasticsearchExtensions
    {
        public static void AddElasticsearch(this IServiceCollection services, IConfiguration configuration)
        {
            var url = configuration.GetConnectionString("elasticsearch");
            var defaultIndex = "test";

            var settings = new ConnectionSettings(new Uri(url))
                .DefaultIndex(defaultIndex)
                .DefaultMappingFor<Patients>(m => m
                    .IndexName("patients")
                    .Ignore(p => p.Secret)
                    .IdProperty(p => p.ID)
                    .PropertyName(p => p.ID, "ID")
                    .PropertyName(p => p.Name, "Name")
                    .PropertyName(p => p.Surname, "Surname")
                    .PropertyName(p => p.Birthdate, "Birthdate")
                    .PropertyName(p => p.Telephone, "Telephone")
                    )
                .DefaultMappingFor<Doctors>(m => m
                    .IndexName("doctors")
                    .Ignore(d => d.Secret)
                    .IdProperty(d => d.ID)
                    .PropertyName(d => d.ID, "ID")
                    .PropertyName(d => d.Name, "Name")
                    .PropertyName(d => d.Surname, "Surname")
                    .PropertyName(d => d.Birthdate, "Birthdate")
                    .PropertyName(d => d.Practice_id, "Practice ID")
                    )
                .DefaultMappingFor<Appointments>(m => m
                    .IndexName("appointments")
                    .Ignore(a => a.Secret)
                    .IdProperty(a => a.ID)
                    .PropertyName(a => a.ID, "ID")
                    .PropertyName(a => a.Doctor_id, "Doctor's ID")
                    .PropertyName(a => a.Patient_id, "Patient's ID")
                    .PropertyName(a => a.Reason, "Reason")
                    .PropertyName(a => a.Date, "Date of appointment")
                );

            var client = new ElasticClient(settings);

            services.AddSingleton<IElasticClient>(client);
        }
    }
}
