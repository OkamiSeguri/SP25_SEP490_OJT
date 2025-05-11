using BusinessObject;

namespace Repositories
{
    public interface IOJTFeedbackRepository
    {
        Task<IEnumerable<OJTFeedback>> GetOJTFeedbackAll();
        Task<OJTFeedback> GetOJTFeedbackById(int id);
        Task<IEnumerable<OJTFeedback>> GetFeedbacksByOJTId(int ojtId);
        Task<IEnumerable<OJTFeedback>> GetFeedbacksByProgramId(int programId);
        Task<IEnumerable<OJTFeedback>> GetFeedbacksByEnterpriseId(int enterpriseId);
        Task<bool> FeedbackExists(int ojtId, int programId);
        Task Create(OJTFeedback ojtFeedback);
        Task Update(OJTFeedback ojtFeedback);
        Task Delete(int id);
    }
}
