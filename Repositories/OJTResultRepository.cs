using BusinessObject;
using DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public class OJTResultRepository : IOJTResultRepository
    {
        public async Task<IEnumerable<OJTResult>> GetOJTResultAll()
        {
            return await OJTResultDAO.Instance.GetOJTResultAll();
        }
        public async Task<OJTResult> GetOJTResultById(int id)
        {
            return await OJTResultDAO.Instance.GetOJTResultById(id);
        }
        public async Task Create(OJTResult ojtResult)
        {
            await OJTResultDAO.Instance.Create(ojtResult);
        }
        public async Task Update(OJTResult ojtResult)
        {
            await OJTResultDAO.Instance.Update(ojtResult);
        }
        public async Task Delete(int id)
        {
            await OJTResultDAO.Instance.Delete(id);
        }
    }
}
