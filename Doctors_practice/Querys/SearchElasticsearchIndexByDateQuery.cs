using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Doctors_practice.Querys
{
    public class SearchElasticsearchIndexByDateQuery : IRequest<List<object>>
    {
        public string SearchValue;

        public SearchElasticsearchIndexByDateQuery(string searchValue)
        {
            SearchValue = searchValue;
        }
    }
}
