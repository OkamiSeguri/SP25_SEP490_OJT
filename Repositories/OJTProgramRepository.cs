using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObject;
using DataAccess;

namespace Repositories
{
    public class OJTProgramRepository : IOJTProgramRepository
    {
        public async Task<IEnumerable<OJTProgram>> GetOJTProgramAll()
        {
            return await OJTProgramDAO.Instance.GetOJTProgramAll();
        }
        public async Task<OJTProgram> GetOJTProgramById(int id)
        {
            return await OJTProgramDAO.Instance.GetOJTProgramById(id);
        }
        public async Task Create(OJTProgram ojtProgram)
        {
            await OJTProgramDAO.Instance.Create(ojtProgram);
        }
        public async Task Update(OJTProgram ojtProgram)
        {
            await OJTProgramDAO.Instance.Update(ojtProgram);
        }
        public async Task Delete(int id)
        {
            await OJTProgramDAO.Instance.Delete(id);
        }
    }
}
