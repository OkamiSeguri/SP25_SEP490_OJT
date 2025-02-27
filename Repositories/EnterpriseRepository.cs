using BusinessObject;
using DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public class EnterpriseRepository : IEnterpriseRepository
    {
        public async Task<IEnumerable<Enterprise>> GetEnterpriseAll()
        {
            return await EnterpriseDAO.Instance.GetEnterpriseAll();
        }

        public async Task<Enterprise> GetEnterpriseById(int id)
        {
            return await EnterpriseDAO.Instance.GetEnterpriseById(id);
        }

        public async Task Create(Enterprise enterprise)
        {
            await EnterpriseDAO.Instance.Create(enterprise);
        }
        public async Task Update(Enterprise enterprise)
        {
            await EnterpriseDAO.Instance.Update(enterprise);
        }
        public async Task Delete(int id)
        {
            await EnterpriseDAO.Instance.Delete(id);
        }
    }
}
