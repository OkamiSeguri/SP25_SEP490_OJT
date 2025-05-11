using BusinessObject;

namespace Repositories
{
    public interface IOJTRegistrationRepository
    {
        Task<IEnumerable<OJTRegistration>> GetOJTRegistrationAll();
        Task<OJTRegistration> GetOJTRegistrationById(int id);
        Task<IEnumerable<OJTRegistration>> GetOJTRegistrationByStatus(string status);
        Task<IEnumerable<OJTRegistration>> ListApproved();
        Task<IEnumerable<OJTRegistration>> ListRejected();
        Task<IEnumerable<OJTRegistration>> ListPending();
        Task<IEnumerable<OJTRegistration>> GetByEnterpriseId(int enterpriseId);
        Task<IEnumerable<OJTRegistration>> GetByStudentId(int studentId);
        Task<IEnumerable<OJTRegistration>> GetByProgramId(int programId);
        Task<int> CountByEnterpriseId(int enterpriseId);
        Task<int> CountByProgramId(int programId);
        Task<bool> ChangeStatus(int ojtId, string newStatus);
        Task<string> GetCurrentStatusByStudentId(int studentId);
        Task Create(OJTRegistration ojtRegistration);
        Task Update(OJTRegistration ojtRegistration);
        Task Delete(int id);
    }
}
