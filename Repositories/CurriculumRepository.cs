using BusinessObject;
using DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public class CurriculumRepository : ICurriculumRepository
    {
        public async Task<IEnumerable<Curriculum>> GetCurriculumAll()
        {
            return await CurriculumDAO.Instance.GetCurriculumAll();
        }

        public async Task<Curriculum> GetCurriculumById(int id)
        {
            return await CurriculumDAO.Instance.GetCurriculumById(id);
        }

        public async Task Create(Curriculum curriculum)
        {
            await CurriculumDAO.Instance.Create(curriculum);
        }
        public async Task Update(Curriculum curriculum)
        {
            await CurriculumDAO.Instance.Update(curriculum);
        }
        public async Task Delete(int id)
        {
            await CurriculumDAO.Instance.Delete(id);
        }
    }
}
