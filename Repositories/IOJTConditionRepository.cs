using BusinessObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
