using BusinessObject;
using Microsoft.EntityFrameworkCore;

namespace DataAccess
{
    public class OJTResultDAO : SingletonBase<OJTResultDAO>
    {
        public async Task<IEnumerable<OJTResult>> GetOJTResultAll()
        {
            return await _context.OJTResults.ToListAsync();
        }
        public async Task<OJTResult> GetOJTResultById(int id)
        {
            return await _context.OJTResults.FirstOrDefaultAsync(c => c.OJTId == id);
        }

        public async Task<IEnumerable<OJTResult>> GetByProgramId(int programId)
        {
            return await _context.OJTResults
                .Where(c => c.ProgramId == programId)
                .ToListAsync();
        }

        public async Task<IEnumerable<OJTResult>> GetResultsByEnterpriseId(int enterpriseId)
        {
            return await _context.OJTResults
                .Where(r => r.EnterpriseId == enterpriseId)
                .ToListAsync();
        }

        public async Task<IEnumerable<OJTResult>> GetResultsByStatus(string status)
        {
            return await _context.OJTResults
                .Where(r => r.Status == status)
                .ToListAsync();
        }
        public async Task<int> CountByStatus(string status)
        {
            return await _context.OJTResults.CountAsync(r => r.Status == status);
        }

        public async Task Create(OJTResult ojtResult)
        {
            await _context.OJTResults.AddAsync(ojtResult);
            await _context.SaveChangesAsync();
        }
        public async Task Update(OJTResult ojtResult)
        {
            var existingItem = await GetOJTResultById(ojtResult.OJTId);
            if (existingItem != null)
            {
                _context.Entry(existingItem).CurrentValues.SetValues(ojtResult);
            }
            await _context.SaveChangesAsync();
        }
        public async Task Delete(int id)
        {
            var ojtResult = await GetOJTResultById(id);
            if (ojtResult != null)
            {
                _context.OJTResults.Remove(ojtResult);
                await _context.SaveChangesAsync();
            }
        }
    }
}
