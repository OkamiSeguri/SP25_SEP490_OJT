using BusinessObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public interface IOJTProgramRepository
    {
        Task<IEnumerable<OJTProgram>> GetOJTProgramAll();
        Task<OJTProgram> GetOJTProgramById(int id);
        Task Create(OJTProgram ojtProgram);
        Task Update(OJTProgram ojtProgram);
        Task Delete(int id);
    }
}
