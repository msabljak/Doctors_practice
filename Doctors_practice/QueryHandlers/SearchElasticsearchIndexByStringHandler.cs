using Doctors_practice.BusinessLayer;
using Doctors_practice.Models;
using Doctors_practice.Models.Doctor;
using Doctors_practice.Models.Patient;
using Doctors_practice.Querys;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Doctors_practice.QueryHandlers
{
    public class SearchElasticsearchIndexByStringHandler : IRequestHandler<SearchElasticsearchIndexByStringQuery, List<object>>
    {
        private IElasticClient _elasticClient;
        private IDoctorRepository _doctorRepository;
        private IPatientRepository _patientRepository;

        public SearchElasticsearchIndexByStringHandler(IElasticClient elasticClient, IDoctorRepository doctorRepository, IPatientRepository patientRepository)
        {
            _elasticClient = elasticClient;
            _doctorRepository = doctorRepository;
            _patientRepository = patientRepository;
        }
        public async Task<List<dynamic>> Handle(SearchElasticsearchIndexByStringQuery request, CancellationToken cancellationToken)
        {
            var searchResponse = await _elasticClient.SearchAsync<dynamic>(s => s
                .AllIndices()
                .Query(q => q
                    .Bool(b => b
                        .Must(m => m
                                .QueryString(qs => qs
                                    .Query(request.SearchValue)
                            )
                        )
                    )
                ));

            //IReadOnlyCollection<object> documents = searchResponse.Documents;
            //IReadOnlyCollection<Nest.IHit<object>> hits = searchResponse.Hits;
            //Dictionary<int, string> searchObjects = new Dictionary<int, string>();

            List<ElasticsearchResponse> objectIdTypePairs = new List<ElasticsearchResponse>();
            foreach (var hit in searchResponse.Hits)
            {
                //searchObjects.Add(Convert.ToInt32(hit.Id), hit.Index);
                objectIdTypePairs.Add(new ElasticsearchResponse(Convert.ToInt32(hit.Id), hit.Index));
            }

            List<int> searchResponseDoctorIds = new List<int>();
            List<int> searchResponsePatientIds = new List<int>();

            foreach (var item in objectIdTypePairs)
            {
                if (item.ObjectType == "doctors")
                {
                    searchResponseDoctorIds.Add(item.Id);
                }
                else if (item.ObjectType == "patients")
                {
                    searchResponsePatientIds.Add(item.Id);
                }
            }

            List<object> searchResults = new List<object>();
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
