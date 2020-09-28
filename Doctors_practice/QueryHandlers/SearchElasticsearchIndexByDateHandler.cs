using Doctors_practice.BusinessLayer;
using Doctors_practice.Models.Appointment;
using Doctors_practice.Models.Doctor;
using Doctors_practice.Models.Patient;
using Doctors_practice.Querys;
using MediatR;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Doctors_practice.QueryHandlers
{
    public class SearchElasticsearchIndexByDateHandler : IRequestHandler<SearchElasticsearchIndexByDateQuery, List<object>>
    {
        private IElasticClient _elasticClient;
        private IAppointmentRepository _appointmentRepository;
        private IDoctorRepository _doctorRepository;
        private IPatientRepository _patientRepository;

        public SearchElasticsearchIndexByDateHandler(IElasticClient elasticClient,IAppointmentRepository appointmentRepository, IDoctorRepository doctorRepository, IPatientRepository patientRepository)
        {
            _elasticClient = elasticClient;
            _appointmentRepository = appointmentRepository;
            _doctorRepository = doctorRepository;
            _patientRepository = patientRepository;
        }
        public async Task<List<object>> Handle(SearchElasticsearchIndexByDateQuery request, CancellationToken cancellationToken)
        {
            //var searchResponse = await _elasticClient.SearchAsync<AppointmentDTO>(s => s
            //    .AllIndices()
            //    .Query(q => q
            //        .Match(m => m
            //            .Field(f => f.Date)
            //            .Query(request.SearchValue)
            //        )
            //    ));

            //var searchResponse = await _elasticClient.SearchAsync<AppointmentDTO>(s => s
            //   .AllIndices()
            //   .Query(q => q
            //       .Bool(b => b
            //           .Filter(f => f
            //               .DateRange(dr => dr
            //                    .Field(f => f.Date)
            //                    .GreaterThanOrEquals(request.SearchValue)
            //                    .LessThanOrEquals(request.SearchValue)
            //               )
            //           )
            //       )
            //   ));
            var searchResponse = await _elasticClient.MultiSearchAsync("_all" ,ms => ms
                    .Search<AppointmentDTO>("appointments", s => s
                        .Index("appointments")
                        .Query(q => q
                            .Bool(b => b
                                .Filter(f => f
                                    .DateRange(dr => dr
                                        .Field(f => f.Date)
                                        .GreaterThanOrEquals(request.SearchValue)
                                        .LessThanOrEquals(request.SearchValue)
                                    )
                                )
                            )
                        )
                    )
                    .Search<ElasticPerson>("people", s => s
                        .AllIndices()
                        .Query(q => q
                            .Match(m => m
                                    .Field(f => f.Birthdate)
                                    .Query(request.SearchValue)
                                )
                        )
                    )
               );
            var appointmentResponses = searchResponse.GetResponse<AppointmentDTO>("appointments");
            var peopleResponses = searchResponse.GetResponse<ElasticPerson>("people");
            List<ElasticsearchResponse> objectIdTypePairs = new List<ElasticsearchResponse>();
            
            foreach (var hit in appointmentResponses.Hits)
            {
                objectIdTypePairs.Add(new ElasticsearchResponse(Convert.ToInt32(hit.Id), hit.Index));
            }
            foreach (var hit in peopleResponses.Hits)
            {
                objectIdTypePairs.Add(new ElasticsearchResponse(Convert.ToInt32(hit.Id), hit.Index));
            }

            List<int> searchResponseAppointmentIds = new List<int>();
            List<int> searchResponseDoctorIds = new List<int>();
            List<int> searchResponsePatientIds = new List<int>();

            foreach (var item in objectIdTypePairs)
            {
                if (item.ObjectType == "appointments")
                {
                    searchResponseAppointmentIds.Add(item.Id);
                }
                else if (item.ObjectType == "doctors")
                {
                    searchResponseDoctorIds.Add(item.Id);
                }
                else if (item.ObjectType == "patients")
                {
                    searchResponsePatientIds.Add(item.Id);
                }
            }

            List<object> searchResults = new List<object>();
            foreach (int id in searchResponseAppointmentIds)
            {
                searchResults.Add(_appointmentRepository.GetAppointment(id));
            }
            foreach (int id in searchResponseDoctorIds)
            {
                searchResults.Add(_doctorRepository.GetDoctor(id));
            }
            foreach (int id in searchResponsePatientIds)
            {
                searchResults.Add(_patientRepository.GetPatients(id));
            }

            return searchResults;
        }
    }
}
