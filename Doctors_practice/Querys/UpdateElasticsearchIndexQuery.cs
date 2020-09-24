using MediatR;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Doctors_practice.Querys
{
    public class UpdateElasticsearchIndexQuery : IRequest<IActionResult>
    {
        public UpdateElasticsearchIndexQuery()
        {

        }
    }
}
