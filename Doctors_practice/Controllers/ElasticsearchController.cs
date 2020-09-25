using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Doctors_practice.BusinessLayer;
using Doctors_practice.Querys;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nest;

namespace Doctors_practice.Controllers
{
    [ApiController]
    public class ElasticsearchController : ControllerBase
    {
        private IMediator _mediator;

        public ElasticsearchController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [Route("elasticsearch/reindex")]
        public async Task<IActionResult> UpdateElasticsearchIndex()
        {
            var response = await _mediator.Send(new UpdateElasticsearchIndexQuery());
            return response;
        }

        [HttpGet]
        [Route("elasticsearch/search/string/{searchValue}")]
        public async Task<List<object>> SearchElasticsearchIndexByString(string searchValue)
        {
            var response = await _mediator.Send(new SearchElasticsearchIndexByStringQuery(searchValue));
            return response;
        }

        [HttpGet]
        [Route("elasticsearch/search/date/{dateValue}")]
        public async Task<List<object>> SearchElasticsearchIndexByDate(DateTime dateValue)
        {
            string dateAsString = dateValue.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss");
            var response = await _mediator.Send(new SearchElasticsearchIndexByDateQuery(dateAsString));
            return response;
        }
    }
}
