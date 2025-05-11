using BusinessObject;
using DataAccess;

namespace Repositories
{
    public class OJTFeedbackRepository : IOJTFeedbackRepository
    {
        public async Task<IEnumerable<OJTFeedback>> GetOJTFeedbackAll()
        {
            return await OJTFeedbackDAO.Instance.GetOJTFeedbackAll();
        }

        public async Task<OJTFeedback> GetOJTFeedbackById(int id)
        {
            return await OJTFeedbackDAO.Instance.GetOJTFeedbackById(id);
        }

        public async Task<IEnumerable<OJTFeedback>> GetFeedbacksByOJTId(int ojtId)
        {
            return await OJTFeedbackDAO.Instance.GetFeedbacksByOJTId(ojtId);
        }

        public async Task<IEnumerable<OJTFeedback>> GetFeedbacksByProgramId(int programId)
        {
            return await OJTFeedbackDAO.Instance.GetFeedbacksByProgramId(programId);
        }

        public async Task<IEnumerable<OJTFeedback>> GetFeedbacksByEnterpriseId(int enterpriseId)
        {
            return await OJTFeedbackDAO.Instance.GetFeedbacksByEnterpriseId(enterpriseId);
        }

        public async Task<bool> FeedbackExists(int ojtId, int programId)
        {
            return await OJTFeedbackDAO.Instance.FeedbackExists(ojtId, programId);
        }

        public async Task Create(OJTFeedback ojtFeedback)
        {
            await OJTFeedbackDAO.Instance.Create(ojtFeedback);
        }

        public async Task Update(OJTFeedback ojtFeedback)
        {
            await OJTFeedbackDAO.Instance.Update(ojtFeedback);
        }

        public async Task Delete(int id)
        {
            await OJTFeedbackDAO.Instance.Delete(id);
        }
    }
}
