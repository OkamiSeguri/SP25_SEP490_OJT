using BusinessObject;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public class OJTConditionDAO : SingletonBase<OJTConditionDAO>
    {
        public async Task<Dictionary<string, string>> GetConditionsAsync()
        {
            return await _context.OJTConditions
                .ToDictionaryAsync(c => c.ConditionKey, c => c.ConditionValue);
        }
        public async Task<OJTCondition> GetConditionByKeyAsync(string key)
        {
            return await _context.OJTConditions.FirstOrDefaultAsync(c => c.ConditionKey == key);
        }

        public async Task AddConditionAsync(OJTCondition condition)
        {
            await _context.OJTConditions.AddAsync(condition);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
