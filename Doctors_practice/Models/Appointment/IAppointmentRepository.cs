using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Doctors_practice.Models.Appointment
{
    public interface IAppointmentRepository
    {
        AppointmentDTO GetAppointment(int id);
        IEnumerable<AppointmentDTO> GetAllAppointments();
        AppointmentDTO Add(AppointmentDTO appointmentDTO);
        int Update(AppointmentDTO appointmentDTOChanges, int id);
        int Delete(int id);
    }
}
