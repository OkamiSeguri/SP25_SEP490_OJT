using BusinessObject;

namespace Repositories
{
    public interface IOJTProgramRepository
    {
        Task<IEnumerable<OJTProgram>> GetOJTProgramAll();
        Task<OJTProgram> GetOJTProgramById(int id);
        Task<OJTProgram> ApproveRequest(int id);
        Task<OJTProgram> RejectRequest(int id);
        Task<IEnumerable<OJTProgram>> ListApproved();
        Task<IEnumerable<OJTProgram>> ListRejected();
        Task<IEnumerable<OJTProgram>> ListPending();
        Task Create(OJTProgram ojtProgram);
        Task Update(OJTProgram ojtProgram);
        Task Delete(int id);
    }
}
