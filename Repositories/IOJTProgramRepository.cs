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
        Task <IEnumerable<OJTProgram>> ApproveRequest(int id);
        Task<IEnumerable<OJTProgram>> RejectRequest(int id);
        Task<IEnumerable<OJTProgram>> ListApproved();
        Task<IEnumerable<OJTProgram>> ListRejected();
        Task<IEnumerable<OJTProgram>> ListPending();
        Task Create(OJTProgram ojtProgram);
        Task Update(OJTProgram ojtProgram);
        Task Delete(int id);
    }
}
