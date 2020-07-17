using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Doctors_practice.Models.Doctor
{
    public interface IDoctorRepository
    {
        DoctorDTO GetDoctor(int id);
        IEnumerable<DoctorDTO> GetAllDoctors();
        DoctorDTO Add(DoctorDTO doctorDTO);
        int Update(DoctorDTO doctorDTOChanges, int id);
        int Delete(int id);
    }
}
