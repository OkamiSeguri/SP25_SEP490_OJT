using BusinessObject;

namespace Repositories
{
    public interface IOJTResultRepository
    {
        Task<IEnumerable<OJTResult>> GetOJTResultAll();
        Task<OJTResult> GetOJTResultById(int id);
        Task<IEnumerable<OJTResult>> GetByProgramId(int programId);
        Task<IEnumerable<OJTResult>> GetResultsByEnterpriseId(int enterpriseId);
        Task<IEnumerable<OJTResult>> GetResultsByStatus(string status);
        Task<int> CountByStatus(string status);
        Task Create(OJTResult ojtResult);
        Task Update(OJTResult ojtResult);
        Task Delete(int id);
    }
}
