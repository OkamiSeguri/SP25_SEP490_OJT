using BusinessObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public interface IOJTResultRepository
    {
        Task<IEnumerable<OJTResult>> GetOJTResultAll();
        Task<OJTResult> GetOJTResultById(int id);
        Task Create(OJTResult ojtResult);
        Task Update(OJTResult ojtResult);
        Task Delete(int id);
    }
}
