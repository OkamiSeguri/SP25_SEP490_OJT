using BusinessObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public interface ICurriculumRepository
    {
        Task<IEnumerable<Curriculum>> GetCurriculumAll();
        Task<Curriculum> GetCurriculumById(int id);
        Task Create(Curriculum curriculum);
        Task Update(Curriculum curriculum);
        Task Delete(int id);
    }
}
