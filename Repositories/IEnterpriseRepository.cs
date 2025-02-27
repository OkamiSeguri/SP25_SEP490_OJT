using BusinessObject;
using DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public interface IEnterpriseRepository
    {
        Task<IEnumerable<Enterprise>> GetEnterpriseAll();
        Task<Enterprise> GetEnterpriseById(int id);
        Task Create(Enterprise enterprise);
        Task Update(Enterprise enterprise);
        Task Delete(int id);
    }
}
