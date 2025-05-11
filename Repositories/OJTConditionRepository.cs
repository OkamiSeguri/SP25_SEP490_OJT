using BusinessObject;
using DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public class OJTConditionRepository : IOJTConditionRepository
    {
        public async Task<double> GetMaxDebtRatioAsync()
        {
            var conditions = await OJTConditionDAO.Instance.GetConditionsAsync();
            return double.Parse(conditions["MaxDebtRatio"]);
        }

        public async Task<bool> ShouldCheckFailedSubjectsAsync()
        {
            var conditions = await OJTConditionDAO.Instance.GetConditionsAsync();
            return bool.Parse(conditions["CheckFailedSubjects"]);
        }

        public async Task UpdateConditionAsync(string key, string value)
        {
            var condition = await OJTConditionDAO.Instance.GetConditionByKeyAsync(key);
            if (condition != null)
            {
                condition.ConditionValue = value;
            }
            else
            {
                await OJTConditionDAO.Instance.AddConditionAsync(new OJTCondition
                {
                    ConditionKey = key,
                    ConditionValue = value
                });
            }
            await OJTConditionDAO.Instance.SaveChangesAsync();
        }
    }
}
