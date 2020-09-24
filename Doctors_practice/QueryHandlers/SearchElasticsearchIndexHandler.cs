using Doctors_practice.Models;
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
    public class SearchElasticsearchIndexHandler : IRequestHandler<SearchElasticsearchIndexQuery, IActionResult>
    {
        private IElasticClient _elasticClient;

        public SearchElasticsearchIndexHandler(IElasticClient elasticClient)
        {
            _elasticClient = elasticClient;
        }
        public async Task<IActionResult> Handle(SearchElasticsearchIndexQuery request, CancellationToken cancellationToken)
        {
            var searchResponse = await _elasticClient.SearchAsync<PatientDTO>(s => s
                .AllIndices()
                .Query(q => q
                    .Match(m => m
                        .Field(f => f.Name)
                        .Query(request.SearchValue)
                    )
                ));

            var patients = searchResponse.Documents;

            return new StatusCodeResult(200);
        }
    }
}
