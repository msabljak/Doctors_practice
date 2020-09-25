using Doctors_practice.BusinessLayer;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Doctors_practice.Querys
{
    public class SearchElasticsearchIndexByStringQuery : IRequest<List<object>>
    {
        public string SearchValue;
        public SearchElasticsearchIndexByStringQuery(string searchValue)
        {
            SearchValue = searchValue;
        }
    }
}
