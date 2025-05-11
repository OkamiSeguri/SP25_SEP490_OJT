using BusinessObject;
using DataAccess;

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

        public async Task<IEnumerable<OJTResult>> GetByProgramId(int programId)
        {
            return await OJTResultDAO.Instance.GetByProgramId(programId);
        }

        public async Task<IEnumerable<OJTResult>> GetResultsByEnterpriseId(int enterpriseId)
        {
            return await OJTResultDAO.Instance.GetResultsByEnterpriseId(enterpriseId);
        }

        public async Task<IEnumerable<OJTResult>> GetResultsByStatus(string status)
        {
            return await OJTResultDAO.Instance.GetResultsByStatus(status);
        }
        public async Task<int> CountByStatus(string status)
        {
            return await OJTResultDAO.Instance.CountByStatus(status);
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
