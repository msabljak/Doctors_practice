using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        [Route("elasticsearch/search/{searchValue}")]
        public async Task<IActionResult> SearchElasticsearchIndex(string searchValue)
        {
            var response = await _mediator.Send(new SearchElasticsearchIndexQuery(searchValue));
            return response;
        }
    }
}
