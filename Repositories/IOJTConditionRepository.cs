using BusinessObject;

namespace Repositories
{
    public interface IOJTConditionRepository
    {
        Task<IEnumerable<OJTCondition>> GetOJTConditionsAll();

        Task<double> GetMaxDebtRatioAsync();
        Task<bool> ShouldCheckFailedSubjectsAsync();
        Task UpdateConditionAsync(string key, string value);

    }
}
