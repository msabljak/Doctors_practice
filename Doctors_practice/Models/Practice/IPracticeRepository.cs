using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Doctors_practice.Models.Practice
{
    public interface IPracticeRepository
    {
        PracticeDTO GetPractices(int id);
        IEnumerable<PracticeDTO> GetAllPractices();
        PracticeDTO Add(PracticeDTO practiceDTO);
        int Update(PracticeDTO practiceDTOChanges, int id);
        int Delete(int id);
    }
}
