using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Doctors_practice.Commands
{
    public class ChargePatientCommand : IRequest<bool>
    {
        private int _id;

        public ChargePatientCommand(int id)
        {
            _id = id;
        }
    }
}
