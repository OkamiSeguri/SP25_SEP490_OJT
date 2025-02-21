using FOMSDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FOMSService
{
    public interface ICurriculumService
    {
        Task<IEnumerable<CurriculumDTO>> GetCurriculumAll();
        Task<CurriculumDTO> GetCurriculumById(int id);
        Task Create(CurriculumDTO curriculumDTO);
        Task Update(CurriculumDTO curriculumDTO);
        Task Delete(int id);

    }
}
