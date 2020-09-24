using MediatR;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Doctors_practice.Querys
{
    public class SearchElasticsearchIndexQuery : IRequest<IActionResult>
    {
        public string SearchValue;
        public SearchElasticsearchIndexQuery(string searchValue)
        {
            SearchValue = searchValue;
        }
    }
}
