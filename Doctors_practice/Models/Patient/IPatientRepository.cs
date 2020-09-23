using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Doctors_practice.Models.Patient
{
    public interface IPatientRepository
    {
        PatientDTO GetPatients(int id);
        IEnumerable<PatientDTO> GetAllPatients();
        PatientDTO Add(PatientDTO patientDTO);
        Task<PatientDTO> AddAsync(PatientDTO patientDTO);
        int Update(PatientDTO patientDTOChanges, int id);
        int Delete(int id);
        Task<int> DeleteAsync(int id);
    }
}
